import { computed, DestroyRef, effect, inject } from '@angular/core';
import { GAME_GATEWAY, GAME_REPOSITORY } from '@core/config/injection-tokens';
import { ConnectionState } from '@core/realtime/connection-state';
import { SignalrConnectionService } from '@core/realtime/signalr-connection.service';
import { AnswerEvaluation } from '@domain/models/answer-evaluation';
import { AppError, toAppError } from '@domain/models/app-error';
import { Game } from '@domain/models/game';
import { GameEvent } from '@domain/models/game-event';
import { GameSettings } from '@domain/models/game-settings';
import { GameStatus } from '@domain/enums/game-status';
import {
  canWithdraw as canPlayerWithdraw,
  isFinalRound as isFinalRoundRule,
  isTerminalStatus,
  prizeIfCorrect as prizeIfCorrectRule,
  progressPercent as progressPercentRule
} from '@domain/rules/game-rules';
import { patchState, signalStore, withComputed, withHooks, withMethods, withState } from '@ngrx/signals';

export type GameStoreStatus = 'idle' | 'loading' | 'ready' | 'error';

export interface GameState {
  readonly game: Game | null;
  readonly settings: GameSettings | null;
  readonly selectedAnswerId: string | null;
  readonly lastEvaluation: AnswerEvaluation | null;
  readonly timeRemaining: number | null;
  readonly connectionState: ConnectionState;
  readonly status: GameStoreStatus;
  readonly error: AppError | null;
}

const initialState: GameState = {
  game: null,
  settings: null,
  selectedAnswerId: null,
  lastEvaluation: null,
  timeRemaining: null,
  connectionState: 'disconnected',
  status: 'idle',
  error: null
};

/**
 * Root scoped: a game survives navigation between /setup, /game/play and /results, and the exit
 * guard needs to reach it from outside the feature's route subtree.
 */
export const GameStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed(({ game, settings, selectedAnswerId }) => ({
    currentQuestion: computed(() => game()?.currentQuestion ?? null),
    accumulatedPrize: computed(() => game()?.accumulatedPrize ?? 0),
    prizeIfCorrect: computed(() => {
      const current = game();
      return current === null ? 0 : prizeIfCorrectRule(current);
    }),
    canWithdraw: computed(() => {
      const current = game();
      return current !== null && canPlayerWithdraw(current);
    }),
    isFinalRound: computed(() => {
      const current = game();
      const configuration = settings();
      return current !== null && configuration !== null && isFinalRoundRule(current.currentRound, configuration);
    }),
    progressPercent: computed(() => {
      const current = game();
      const configuration = settings();
      return current === null || configuration === null ? 0 : progressPercentRule(current, configuration);
    }),
    isGameOver: computed(() => {
      const current = game();
      return current !== null && isTerminalStatus(current.status);
    }),
    isAwaitingAnswer: computed(() => {
      const current = game();
      return current !== null && current.status === 'InProgress' && current.currentQuestion !== null && selectedAnswerId() === null;
    })
  })),
  withMethods((store, repository = inject(GAME_REPOSITORY), gateway = inject(GAME_GATEWAY)) => {
    function fail(error: unknown): void {
      patchState(store, { status: 'error', error: toAppError(error) });
    }

    function applyEvent(event: GameEvent): void {
      const current = store.game();
      if (current !== null && event.gameId !== current.id) {
        return;
      }

      switch (event.type) {
        case 'GameStarted':
          patchState(store, { timeRemaining: store.settings()?.questionTimeLimitSeconds ?? 30 });
          break;
        case 'RoundStarted':
          patchState(store, {
            timeRemaining: store.settings()?.questionTimeLimitSeconds ?? 30,
            game:
              current === null
                ? null
                : {
                    ...current,
                    currentRound: event.roundNumber,
                    prizeAtStake: event.prizeAtStake,
                    deadlineUtc: event.deadlineUtc,
                    currentQuestion: {
                      id: event.questionId,
                      categoryId: current.currentQuestion?.categoryId ?? '',
                      text: event.questionText,
                      answers: event.answers
                    }
                  },
            selectedAnswerId: null,
            lastEvaluation: null
          });
          break;
        case 'AnswerEvaluated':
          patchState(store, {
            game: current === null ? null : { ...current, status: event.status, accumulatedPrize: event.accumulatedPrize }
          });
          break;
        case 'RoundAdvanced':
          patchState(store, {
            game:
              current === null
                ? null
                : { ...current, currentRound: event.currentRoundNumber, accumulatedPrize: event.accumulatedPrize }
          });
          break;
        case 'PrizeAccumulated':
          patchState(store, {
            game: current === null ? null : { ...current, accumulatedPrize: event.accumulatedPrize }
          });
          break;
        case 'GameEnded':
          patchState(store, {
            game:
              current === null
                ? null
                : {
                    ...current,
                    status: event.status,
                    accumulatedPrize: event.finalPrize,
                    deadlineUtc: null
                  },
            timeRemaining: 0
          });
          break;
        case 'TimeRemaining':
          if (!store.isGameOver()) {
            patchState(store, { timeRemaining: event.secondsRemaining });
          }
          break;
      }
    }

    async function loadSettings(): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        patchState(store, { settings: await repository.getSettings(), status: 'ready' });
      } catch (error) {
        fail(error);
      }
    }

    async function refreshState(): Promise<void> {
      const gameId = store.game()?.id;
      if (gameId === undefined) {
        return;
      }

      try {
        patchState(store, { game: await repository.getGameState(gameId), status: 'ready', error: null });
      } catch (error) {
        fail(error);
      }
    }

    async function startGame(playerName: string): Promise<void> {
      patchState(store, {
        status: 'loading',
        error: null,
        lastEvaluation: null,
        selectedAnswerId: null,
        timeRemaining: store.settings()?.questionTimeLimitSeconds ?? 30
      });

      try {
        const game = await repository.startGame({ playerName });
        patchState(store, { game, status: 'ready' });
        await gateway.joinGame(game.id);
      } catch (error) {
        fail(error);
      }
    }

    async function joinGame(gameId: string): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        patchState(store, { game: await gateway.joinGame(gameId), status: 'ready' });
      } catch (error) {
        fail(error);
      }
    }

    function selectAnswer(answerId: string): void {
      patchState(store, { selectedAnswerId: answerId });
    }

    async function submitAnswer(answerId: string): Promise<void> {
      const game = store.game();
      if (game === null) {
        return;
      }

      patchState(store, { selectedAnswerId: answerId, status: 'loading', error: null });

      try {
        const evaluation = await gateway.submitAnswer({
          gameId: game.id,
          answerId,
          requestId: crypto.randomUUID()
        });

        const isFinished = evaluation.status !== 'InProgress';
        patchState(store, {
          lastEvaluation: evaluation,
          timeRemaining: isFinished ? 0 : store.timeRemaining(),
          status: 'ready',
          game: {
            ...game,
            status: evaluation.status,
            accumulatedPrize: evaluation.accumulatedPrize,
            currentRound: evaluation.currentRound,
            prizeAtStake: evaluation.prizeAtStake,
            currentQuestion: evaluation.nextQuestion,
            deadlineUtc: evaluation.deadlineUtc
          }
        });
      } catch (error) {
        fail(error);
      }
    }

    async function withdraw(): Promise<void> {
      const game = store.game();
      if (game === null) {
        return;
      }

      patchState(store, { status: 'loading', error: null });

      try {
        const summary = await gateway.withdraw({ gameId: game.id });

        patchState(store, {
          status: 'ready',
          timeRemaining: 0,
          game: {
            ...game,
            status: summary.status,
            accumulatedPrize: summary.finalPrize,
            deadlineUtc: null
          }
        });
      } catch (error) {
        fail(error);
      }
    }

    function reset(): void {
      patchState(store, initialState);
    }

    return { applyEvent, loadSettings, refreshState, startGame, joinGame, selectAnswer, submitAnswer, withdraw, reset };
  }),
  withHooks({
    onInit(store, connection = inject(SignalrConnectionService), gateway = inject(GAME_GATEWAY)) {
      const destroyRef = inject(DestroyRef);
      let previousConnectionState: ConnectionState = connection.connectionState();

      effect(() => {
        const currentConnectionState = connection.connectionState();
        patchState(store, { connectionState: currentConnectionState });

        // A reconnection may land after the round deadline expired, so never assume continuity.
        if (previousConnectionState === 'reconnecting' && currentConnectionState === 'connected') {
          void store.refreshState();
        }

        previousConnectionState = currentConnectionState;
      });

      let previousStatus: GameStatus | null = null;
      effect(() => {
        const game = store.game();
        const currentStatus = game?.status ?? null;
        if (currentStatus !== null && currentStatus !== previousStatus) {
          console.log(`[GameStatus] Transitioned from '${previousStatus ?? 'None'}' to '${currentStatus}' (Game ID: ${game?.id})`);
          previousStatus = currentStatus;
        }
      });

      const unsubscribe = gateway.onEvent((event) => store.applyEvent(event));
      destroyRef.onDestroy(() => unsubscribe());
    }
  })
);
