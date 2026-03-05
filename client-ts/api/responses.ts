import { Questionnaire, QuestionnaireDelta } from '../model/questionnaire';

export interface QuestionnaireResponse {
  questionnaire: Questionnaire;
}

export interface QuestionnaireUpdatedResponse {
  delta: QuestionnaireDelta;
}

export interface QuestionnaireListItemResponse {
  id: string;
  title: string;
  description?: string | null;
  createdAt: string;
}

export interface QuestionnaireListResponse {
  questionnaires: QuestionnaireListItemResponse[];
}
