namespace QuizGame.Api.Hubs;

public sealed record SubmitAnswerRequest(Guid GameId, Guid AnswerId, Guid RequestId);

public sealed record WithdrawRequest(Guid GameId);
