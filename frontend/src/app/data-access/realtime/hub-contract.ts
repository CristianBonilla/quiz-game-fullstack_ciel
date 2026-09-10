export const HUB_METHODS = {
  joinGame: 'JoinGameAsync',
  leaveGame: 'LeaveGameAsync',
  submitAnswer: 'SubmitAnswerAsync',
  withdraw: 'WithdrawAsync'
} as const;

export const HUB_EVENTS = {
  gameStarted: 'GameStartedAsync',
  roundStarted: 'RoundStartedAsync',
  answerEvaluated: 'AnswerEvaluatedAsync',
  roundAdvanced: 'RoundAdvancedAsync',
  prizeAccumulated: 'PrizeAccumulatedAsync',
  gameEnded: 'GameEndedAsync',
  timeRemaining: 'TimeRemainingAsync'
} as const;
