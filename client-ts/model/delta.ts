/**
 * Patchable for non-nullable values - only has NotGiven and Set states
 */
export class Patchable<T> {
  private constructor(
    private readonly _value: T | undefined,
    private readonly _isSet: boolean
  ) {}

  static notGiven<T>(): Patchable<T> {
    return new Patchable<T>(undefined, false);
  }

  static set<T>(value: T): Patchable<T> {
    return new Patchable<T>(value, true);
  }

  get isNotGiven(): boolean {
    return !this._isSet;
  }

  get isSet(): boolean {
    return this._isSet;
  }

  get value(): T {
    if (!this._isSet || this._value === undefined) {
      throw new Error('Patchable has no value');
    }
    return this._value;
  }

  apply<TTarget>(target: TTarget, setter: (target: TTarget, value: T) => void): void {
    if (this._isSet && this._value !== undefined) {
      setter(target, this._value);
    }
  }
}

enum PatchState {
  NotGiven = 'NotGiven',
  Clear = 'Clear',
  Set = 'Set',
}

/**
 * PatchableNullable for nullable values - has NotGiven, Clear, and Set states
 */
export class PatchableNullable<T> {
  private constructor(
    private readonly _value: T | null | undefined,
    private readonly _state: PatchState
  ) {}

  static notGiven<T>(): PatchableNullable<T> {
    return new PatchableNullable<T>(undefined, PatchState.NotGiven);
  }

  static clear<T>(): PatchableNullable<T> {
    return new PatchableNullable<T>(null, PatchState.Clear);
  }

  static set<T>(value: T | null): PatchableNullable<T> {
    if (value === null) {
      return PatchableNullable.clear<T>();
    }
    return new PatchableNullable<T>(value, PatchState.Set);
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

  get value(): T | null {
    return this._state === PatchState.Set ? (this._value as T) : null;
  }

  apply<TTarget>(target: TTarget, setter: (target: TTarget, value: T | null) => void): void {
    if (this._state === PatchState.NotGiven) {
      return;
    }

    if (this._state === PatchState.Clear) {
      setter(target, null);
      return;
    }

    setter(target, this._value as T);
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
