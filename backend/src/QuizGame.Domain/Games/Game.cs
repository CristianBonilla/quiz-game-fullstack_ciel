using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.Games.Events;
using QuizGame.Domain.Games.Rules;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.Games;

public sealed class Game : AggregateRoot<Guid>
{
    private readonly List<Round> _rounds = [];

    private Game(Guid id, PlayerName playerName, GameSettings settings, RoundNumber firstRound, DateTime startedAtUtc)
        : base(id)
    {
        PlayerName = playerName;
        Settings = settings;
        CurrentRound = firstRound;
        AccumulatedPrize = Prize.Zero;
        StartedAtUtc = startedAtUtc;
        Status = GameStatus.NotStarted;
    }

    private Game()
    {
        PlayerName = null!;
        Settings = null!;
        CurrentRound = null!;
        AccumulatedPrize = null!;
    }

    public PlayerName PlayerName { get; private set; }

    public GameSettings Settings { get; private set; }

    public GameStatus Status { get; private set; }

    public RoundNumber CurrentRound { get; private set; }

    public Prize AccumulatedPrize { get; private set; }

    public DateTime StartedAtUtc { get; private set; }

    public DateTime? EndedAtUtc { get; private set; }

    public IReadOnlyCollection<Round> Rounds => _rounds.AsReadOnly();

    public IReadOnlyCollection<Guid> AskedQuestionIds =>
        _rounds.ConvertAll(round => round.QuestionId).AsReadOnly();

    public static Result<Game> Create(
        Guid id,
        PlayerName playerName,
        GameSettings settings,
        DateTime startedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Result<RoundNumber> firstRound = RoundNumber.Create(RoundNumber.First, settings.TotalRounds);
        if (firstRound.IsFailure)
        {
            return Failure<Game>(firstRound.Error);
        }

        Game game = new(id, playerName, settings, firstRound.Value, startedAtUtc);

        Result transition = game.TransitionTo(GameStatus.InProgress);
        if (transition.IsFailure)
        {
            return Failure<Game>(transition.Error);
        }

        game.RaiseDomainEvent(new GameStartedDomainEvent(
            id,
            playerName.Value,
            settings.TotalRounds,
            startedAtUtc));

        return game;
    }

    public bool CanTransitionTo(GameStatus target) => GameStateMachine.CanTransitionTo(Status, target);

    public Result AssignQuestion(
        Question question,
        Prize prizeAtStake,
        DateTime startedAtUtc,
        DateTime deadlineUtc)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(prizeAtStake);

        if (Status != GameStatus.InProgress)
        {
            return Result.Failure(GameErrors.NotInProgress);
        }

        if (!question.IsActive)
        {
            return Result.Failure(GameErrors.QuestionNotActive);
        }

        if (OpenRound is not null)
        {
            return Result.Failure(GameErrors.RoundAlreadyOpen);
        }

        if (_rounds.Exists(round => round.QuestionId == question.Id))
        {
            return Result.Failure(GameErrors.QuestionAlreadyAsked);
        }

        Round newRound = Round.Open(
            Guid.NewGuid(),
            Id,
            CurrentRound,
            question.Id,
            question.CorrectAnswerId,
            prizeAtStake,
            deadlineUtc);

        _rounds.Add(newRound);

        RaiseDomainEvent(new RoundStartedDomainEvent(
            Id,
            CurrentRound.Value,
            question.Id,
            prizeAtStake.Amount,
            deadlineUtc,
            startedAtUtc));

        return Success();
    }

    public Result Answer(Guid answerId, DateTime answeredAtUtc)
    {
        if (Status != GameStatus.InProgress)
        {
            return Result.Failure(GameErrors.NotInProgress);
        }

        Round? round = _rounds.Find(candidate => candidate.Number == CurrentRound);
        if (round is null)
        {
            return Result.Failure(GameErrors.NoOpenRound);
        }

        if (round.IsAnswered)
        {
            return Result.Failure(GameErrors.RoundAlreadyAnswered);
        }

        if (GameRules.HasExpired(round.DeadlineUtc, answeredAtUtc))
        {
            return Result.Failure(GameErrors.RoundExpired);
        }

        RoundOutcome outcome = GameRules.EvaluateAnswer(answerId, round.CorrectAnswerId);
        round.RegisterAnswer(answerId, outcome, answeredAtUtc);

        RaiseDomainEvent(new AnswerSubmittedDomainEvent(
            Id,
            round.Number.Value,
            round.QuestionId,
            answerId,
            outcome == RoundOutcome.Correct,
            answeredAtUtc));

        Prize previousAccumulated = AccumulatedPrize;
        AccumulatedPrize = GameRules.AccumulateFor(outcome, AccumulatedPrize, round.PrizeAtStake);

        return outcome == RoundOutcome.Correct
            ? RegisterCorrectAnswer(round, answeredAtUtc)
            : RegisterIncorrectAnswer(round, previousAccumulated, answeredAtUtc);
    }

    public Result Withdraw(DateTime withdrawnAtUtc)
    {
        if (Status != GameStatus.InProgress)
        {
            return Result.Failure(GameErrors.NotInProgress);
        }

        Round? round = _rounds.Find(candidate => candidate.Number == CurrentRound);
        if (round is null)
        {
            return Result.Failure(GameErrors.NoOpenRound);
        }

        if (round.IsAnswered)
        {
            return Result.Failure(GameErrors.CannotWithdrawAfterAnswering);
        }

        Result transition = TransitionTo(GameStatus.Withdrawn);
        if (transition.IsFailure)
        {
            return transition;
        }

        EndedAtUtc = withdrawnAtUtc;

        RaiseDomainEvent(new GameWithdrawnDomainEvent(
            Id,
            PlayerName.Value,
            CurrentRound.Value,
            AccumulatedPrize.Amount,
            withdrawnAtUtc));

        return Success();
    }

    public Result ForceEnd(string? reason, DateTime endedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure(GameErrors.EmptyForcedEndReason);
        }

        if (Status != GameStatus.InProgress)
        {
            return Result.Failure(GameErrors.NotInProgress);
        }

        Result transition = TransitionTo(GameStatus.ForcedEnd);
        if (transition.IsFailure)
        {
            return transition;
        }

        AccumulatedPrize = Prize.Zero;
        EndedAtUtc = endedAtUtc;

        RaiseDomainEvent(new GameForciblyEndedDomainEvent(
            Id,
            PlayerName.Value,
            CurrentRound.Value,
            reason.Trim(),
            endedAtUtc));

        return Success();
    }

    private Round? OpenRound => _rounds.Find(round => !round.IsAnswered);

    private Result RegisterCorrectAnswer(Round round, DateTime answeredAtUtc)
    {
        RaiseDomainEvent(new PrizeAccumulatedDomainEvent(
            Id,
            round.Number.Value,
            round.PrizeAtStake.Amount,
            AccumulatedPrize.Amount,
            answeredAtUtc));

        if (round.Number.IsFinal)
        {
            Result won = TransitionTo(GameStatus.Won);
            if (won.IsFailure)
            {
                return won;
            }

            EndedAtUtc = answeredAtUtc;

            RaiseDomainEvent(new GameWonDomainEvent(
                Id,
                PlayerName.Value,
                AccumulatedPrize.Amount,
                answeredAtUtc));

            return Success();
        }

        Result<RoundNumber> next = CurrentRound.Next();
        if (next.IsFailure)
        {
            return Result.Failure(next.Error);
        }

        int previousRoundNumber = CurrentRound.Value;
        CurrentRound = next.Value;

        RaiseDomainEvent(new RoundAdvancedDomainEvent(
            Id,
            previousRoundNumber,
            CurrentRound.Value,
            AccumulatedPrize.Amount,
            answeredAtUtc));

        return Success();
    }

    private Result RegisterIncorrectAnswer(Round round, Prize forfeitedPrize, DateTime answeredAtUtc)
    {
        Result transition = TransitionTo(GameStatus.Lost);
        if (transition.IsFailure)
        {
            return transition;
        }

        EndedAtUtc = answeredAtUtc;

        RaiseDomainEvent(new GameLostDomainEvent(
            Id,
            PlayerName.Value,
            round.Number.Value,
            forfeitedPrize.Amount,
            answeredAtUtc));

        return Success();
    }

    private Result TransitionTo(GameStatus target)
    {
        if (!GameStateMachine.CanTransitionTo(Status, target))
        {
            return Result.Failure(GameErrors.InvalidTransition(Status, target));
        }

        Status = target;

        return Success();
    }
}
