import { Question, QuestionDelta } from './question';
import { Patchable, PatchableArray } from './delta';

export interface Questionnaire {
  id: string;
  title: string;
  description?: string | null;
  content: Question[];
  createdAt: string;
}

export interface QuestionnaireDelta {
  id: string;
  title?: Patchable<string>;
  description?: Patchable<string | null>;
  content?: PatchableArray<QuestionDelta>;
  createdAt?: Patchable<string>;
}
