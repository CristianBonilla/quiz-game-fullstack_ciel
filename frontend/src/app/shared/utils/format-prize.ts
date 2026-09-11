const PRIZE_FORMATTER = new Intl.NumberFormat('es-CO', {
  style: 'currency',
  currency: 'COP',
  maximumFractionDigits: 0
});

export function formatPrize(amount: number): string {
  return PRIZE_FORMATTER.format(amount);
}
