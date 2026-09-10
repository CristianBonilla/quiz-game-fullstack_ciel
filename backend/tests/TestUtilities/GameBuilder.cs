using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.TestUtilities;

public sealed class GameBuilder
{
    private string _playerName = "Alice";
    private int _targetRound = 1;
    private readonly DateTime _now = TestClock.FixedUtcNow;

    private bool _withOpenRound;
    private Guid? _openQuestionId;
    private Guid? _openCorrectAnswerId;
    private decimal _openPrize = 100m;
    private DateTime? _openDeadline;

    public static GameBuilder AGame() => new();

    public GameBuilder WithPlayer(string name)
    {
        _playerName = name;
        return this;
    }

    public GameBuilder InRound(int round)
    {
        _targetRound = round;
        return this;
    }

    public GameBuilder WithOpenRound(
        Guid? questionId = null,
        Guid? correctAnswerId = null,
        decimal prize = 100m,
        DateTime? deadline = null)
    {
        _withOpenRound = true;
        _openQuestionId = questionId;
        _openCorrectAnswerId = correctAnswerId;
        _openPrize = prize;
        _openDeadline = deadline;
        return this;
    }

    public Game Build()
    {
        GameSettings settings = GameSettings.Default;
        PlayerName playerName = PlayerName.Create(_playerName).Value;
        Game game = Game.Create(Guid.NewGuid(), playerName, settings, _now).Value;

        for (int round = 1; round < _targetRound; round++)
        {
            Prize prize = settings.PrizeForRound(round).Value;
            Question question = QuestionBuilder.AQuestion().Build();

            game.AssignQuestion(question, prize, _now, _now.AddSeconds(30));
            game.Answer(question.CorrectAnswerId, _now);
        }

        if (_withOpenRound)
        {
            Question question = QuestionBuilder.AQuestion()
                .WithId(_openQuestionId ?? Guid.NewGuid())
                .WithCorrectAnswerAt(0, _openCorrectAnswerId)
                .Build();

            game.AssignQuestion(
                question,
                Prize.Create(_openPrize).Value,
                _now,
                _openDeadline ?? _now.AddSeconds(30));
        }

        return game;
    }
}
