enum PatchState {
  NotGiven = 'NotGiven',
  Clear = 'Clear',
  Set = 'Set',
}

export class Patchable<T> {
  private constructor(
    private readonly _value: T | null | undefined,
    private readonly _state: PatchState
  ) {}

  static notGiven<T>(): Patchable<T> {
    return new Patchable<T>(undefined, PatchState.NotGiven);
  }

  static clear<T>(): Patchable<T> {
    return new Patchable<T>(null, PatchState.Clear);
  }

  static set<T>(value: T): Patchable<T> {
    return new Patchable<T>(value, PatchState.Set);
  }

  get isNotGiven(): boolean {
    return this._state === PatchState.NotGiven;
  }

  get isClear(): boolean {
    return this._state === PatchState.Clear;
  }

  get isSet(): boolean {
    return this._state === PatchState.Set;
  }

  get hasValue(): boolean {
    return this._state === PatchState.Set;
  }

  get value(): T | null | undefined {
    return this._state === PatchState.Set ? this._value : undefined;
  }

  apply<TTarget>(target: TTarget, setter: (target: TTarget, value: T | null | undefined) => void): void {
    if (this._state === PatchState.NotGiven) {
      return;
    }

    if (this._state === PatchState.Clear) {
      setter(target, null);
      return;
    }

    setter(target, this._value);
  }
}

export type PatchableArrayOperation =
  | 'Replace'
  | 'Add'
  | 'AddRange'
  | 'Remove'
  | 'RemoveById'
  | 'RemoveRange'
  | 'Move'
  | 'MoveById'
  | 'Clear';

export interface PatchableArray<T> {
  operation: PatchableArrayOperation;
  index?: number | null;
  item?: T | null;
  items?: T[] | null;
  itemId?: string | null;
  count?: number | null;
  toIndex?: number | null;
}
