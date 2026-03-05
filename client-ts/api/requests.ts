import { Question } from '../model/question';
import { UpdateQuestionnaireProperty } from '../model/commands';

export interface CreateQuestionnaireRequest {
  title: string;
  description?: string | null;
  content?: Question[];
}

// For update, we now use the command directly
export type UpdateQuestionnaireRequest = Omit<UpdateQuestionnaireProperty, 'type' | 'id'>;
