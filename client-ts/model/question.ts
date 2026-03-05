import { Patchable, PatchableArray } from './delta';

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

export interface QuestionDeltaBase {
  id: string;
  title?: Patchable<string>;
  description?: Patchable<string | null>;
  subQuestions?: PatchableArray<QuestionDelta>;
  questionType: QuestionType;
}

export interface MessageQuestionDelta extends QuestionDeltaBase {
  questionType: 'Message';
}

export interface TextQuestionDelta extends QuestionDeltaBase {
  questionType: 'Text';
}

export interface NumberQuestionDelta extends QuestionDeltaBase {
  questionType: 'Number';
}

export interface EmailQuestionDelta extends QuestionDeltaBase {
  questionType: 'Email';
}

export type QuestionDelta = MessageQuestionDelta | TextQuestionDelta | NumberQuestionDelta | EmailQuestionDelta;
