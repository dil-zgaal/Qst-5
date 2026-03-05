import { Question } from './question';
import { Patchable, PatchableNullable } from './delta';

export interface Questionnaire {
  id: string;
  version: number;
  createdAt: string;
  updatedAt?: string;
  title: string;
  description?: string | null;
  content: Question[];
}

export class QuestionnaireDelta {
  id: string;
  fromVersion: number;
  toVersion: number;
  updatedAt: string;
  title: Patchable<string>;
  description: PatchableNullable<string>;

  constructor(id: string, fromVersion: number, toVersion: number, updatedAt: string) {
    this.id = id;
    this.fromVersion = fromVersion;
    this.toVersion = toVersion;
    this.updatedAt = updatedAt;
    this.title = Patchable.notGiven<string>();
    this.description = PatchableNullable.notGiven<string>();
  }

  apply(questionnaire: Questionnaire): void {
    if (questionnaire.id !== this.id) {
      throw new Error('Delta Id does not match Questionnaire Id');
    }

    if (questionnaire.version !== this.fromVersion) {
      throw new Error(
        `Questionnaire version ${questionnaire.version} does not match delta from version ${this.fromVersion}`
      );
    }

    // Apply version
    questionnaire.version = this.toVersion;
    questionnaire.updatedAt = this.updatedAt;

    // Apply questionnaire-level property patches
    this.title.apply(questionnaire, (q, v) => {
      q.title = v;
    });
    this.description.apply(questionnaire, (q, v) => {
      q.description = v;
    });
  }

  applyDelta(delta: QuestionnaireDelta): void {
    if (delta.id !== this.id) {
      throw new Error('Delta Id does not match Questionnaire Id');
    }

    if (this.toVersion !== delta.fromVersion) {
      throw new Error(
        `Delta from version ${delta.fromVersion} does not match to version ${this.toVersion}`
      );
    }

    this.toVersion = delta.toVersion;
    this.updatedAt = delta.updatedAt;

    // Apply patches using the Apply method on Patchable types
    if (!delta.title.isNotGiven) {
      this.title = delta.title;
    }

    if (!delta.description.isNotGiven) {
      this.description = delta.description;
    }
  }
}
