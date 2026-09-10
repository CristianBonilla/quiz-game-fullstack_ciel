/** Bounded FIFO of seen identities: the hub is at-least-once, so replays must not reach the store. */
export class EventDeduplicator {
  private readonly seen = new Set<string>();
  private readonly order: string[] = [];

  constructor(private readonly capacity = 256) {}

  shouldProcess(identity: string): boolean {
    if (this.seen.has(identity)) {
      return false;
    }

    this.seen.add(identity);
    this.order.push(identity);

    if (this.order.length > this.capacity) {
      const evicted = this.order.shift();
      if (evicted !== undefined) {
        this.seen.delete(evicted);
      }
    }

    return true;
  }

  clear(): void {
    this.seen.clear();
    this.order.length = 0;
  }
}
