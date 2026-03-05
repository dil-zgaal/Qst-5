import { Patchable, PatchableNullable, PatchableArray } from './delta';

export type QuestionType = 'Message' | 'Text' | 'Number' | 'Email';

export interface QuestionBase {
  id: string;
  title: string;
  description?: string | null;
  subQuestions: Question[];
  questionType: QuestionType;
}

export interface MessageQuestion extends QuestionBase {
  questionType: 'Message';
}

export interface TextQuestion extends QuestionBase {
  questionType: 'Text';
}

export interface NumberQuestion extends QuestionBase {
  questionType: 'Number';
}

export interface EmailQuestion extends QuestionBase {
  questionType: 'Email';
}

export type Question = MessageQuestion | TextQuestion | NumberQuestion | EmailQuestion;

export class QuestionDelta {
  id: string;
  title: Patchable<string>;
  description: PatchableNullable<string>;
  subQuestions?: PatchableArray<QuestionDelta>;

  constructor(id: string) {
    this.id = id;
    this.title = Patchable.notGiven<string>();
    this.description = PatchableNullable.notGiven<string>();
  }

  apply(question: Question): void {
    if (question.id !== this.id) {
      throw new Error('Delta Id does not match Question Id');
    }

    this.title.apply(question, (q, v) => {
      q.title = v;
    });
    this.description.apply(question, (q, v) => {
      q.description = v;
    });

    if (this.subQuestions) {
      applyQuestionDeltaPatchableArray(question.subQuestions, this.subQuestions);
    }
  }
}

export interface MessageQuestionDelta extends QuestionDelta {
  questionType: 'Message';
}

export interface TextQuestionDelta extends QuestionDelta {
  questionType: 'Text';
}

export interface NumberQuestionDelta extends QuestionDelta {
  questionType: 'Number';
}

export interface EmailQuestionDelta extends QuestionDelta {
  questionType: 'Email';
}

export function applyQuestionDeltaPatchableArray(
  list: Question[],
  patch: PatchableArray<QuestionDelta>
): void {
  switch (patch.operation) {
    case 'Replace':
    case 'Add':
    case 'AddRange':
      throw new Error('Add/Replace operations not supported for QuestionDelta patches. Use full Question objects.');

    case 'Remove':
      if (patch.index != null && patch.index >= 0 && patch.index < list.length) {
        list.splice(patch.index, 1);
      }
      break;

    case 'RemoveById':
      if (patch.itemId != null) {
        const index = list.findIndex((q) => q.id === patch.itemId);
        if (index >= 0) {
          list.splice(index, 1);
        }
      }
      break;

    case 'RemoveRange':
      if (
        patch.index != null &&
        patch.count != null &&
        patch.index >= 0 &&
        patch.index < list.length
      ) {
        const count = Math.min(patch.count, list.length - patch.index);
        list.splice(patch.index, count);
      }
      break;

    case 'Move':
      if (
        patch.index != null &&
        patch.toIndex != null &&
        patch.index >= 0 &&
        patch.index < list.length &&
        patch.toIndex >= 0 &&
        patch.toIndex < list.length
      ) {
        const [item] = list.splice(patch.index, 1);
        list.splice(patch.toIndex, 0, item);
      }
      break;

    case 'MoveById':
      if (patch.itemId != null && patch.toIndex != null) {
        const index = list.findIndex((q) => q.id === patch.itemId);
        if (index >= 0) {
          const [item] = list.splice(index, 1);
          const toIndex = Math.min(patch.toIndex, list.length);
          list.splice(toIndex, 0, item);
        }
      }
      break;

    case 'Clear':
      list.splice(0, list.length);
      break;
  }

  // After structural operations, apply any deltas to existing questions
  if (patch.item != null) {
    const existing = list.find((q) => q.id === patch.item!.id);
    if (existing) {
      patch.item.apply(existing);
    }
  }

  if (patch.items != null) {
    for (const delta of patch.items) {
      const existing = list.find((q) => q.id === delta.id);
      if (existing) {
        delta.apply(existing);
      }
    }
  }
}
