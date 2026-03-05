import { Question, QuestionDelta, applyQuestionDeltaPatchableArray } from './question';
import { Patchable, PatchableArray } from './delta';

export interface Questionnaire {
  id: string;
  title: string;
  description?: string | null;
  content: Question[];
  createdAt: string;
}

export class QuestionnaireDelta {
  id: string;
  title: Patchable<string>;
  description: Patchable<string | null>;
  content?: PatchableArray<QuestionDelta>;
  createdAt: Patchable<string>;

  constructor(id: string) {
    this.id = id;
    this.title = Patchable.notGiven<string>();
    this.description = Patchable.notGiven<string | null>();
    this.createdAt = Patchable.notGiven<string>();
  }

  apply(questionnaire: Questionnaire): void {
    if (questionnaire.id !== this.id) {
      throw new Error('Delta Id does not match Questionnaire Id');
    }

    // Apply questionnaire-level property patches
    this.title.apply(questionnaire, (q, v) => {
      if (v != null) q.title = v;
    });
    this.description.apply(questionnaire, (q, v) => {
      q.description = v;
    });
    this.createdAt.apply(questionnaire, () => {
      /* createdAt is readonly, cannot be changed */
    });

    // Apply content array patch
    if (this.content) {
      applyQuestionDeltaPatchableArray(questionnaire.content, this.content);
    }
  }

  static merge(id: string, ...deltas: QuestionnaireDelta[]): QuestionnaireDelta {
    if (deltas.some((d) => d.id !== id)) {
      throw new Error('All deltas must have the same Id');
    }

    const merged = new QuestionnaireDelta(id);

    // Merge property patches (later patches override earlier ones)
    for (const delta of deltas) {
      if (!delta.title.isNotGiven) {
        merged.title = delta.title;
      }

      if (!delta.description.isNotGiven) {
        merged.description = delta.description;
      }

      if (!delta.createdAt.isNotGiven) {
        merged.createdAt = delta.createdAt;
      }

      // For Content, only keep the last one (arrays are replaced, not merged individually)
      if (delta.content) {
        merged.content = delta.content;
      }
    }

    return merged;
  }
}
