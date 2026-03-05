export type Patchable<T> = T | null | undefined;

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
