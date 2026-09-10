const PRIZE_FORMATTER = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  maximumFractionDigits: 0
});

export function formatPrize(amount: number): string {
  return PRIZE_FORMATTER.format(amount);
}
