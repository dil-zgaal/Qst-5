import { Question } from '../model/question';

export interface CreateQuestionnaireRequest {
  title: string;
  description?: string | null;
  content?: Question[];
}

export interface UpdateQuestionnaireRequest {
  title: string;
  description?: string | null;
}
