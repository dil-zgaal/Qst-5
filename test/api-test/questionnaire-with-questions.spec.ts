import { test, expect } from '@playwright/test';
import { QuestionnaireRoute } from '@client/api/questionnaireRoute';
import type { CreateQuestionnaireRequest } from '@client/api/requests';
import type {
  QuestionnaireResponse,
  QuestionnaireUpdatedResponse,
} from '@client/api/responses';
import type { MessageQuestion, TextQuestion, NumberQuestion, EmailQuestion } from '@client/model/question';

const baseURL = process.env.API_BASE_URL || 'http://localhost:5000';
const route = new QuestionnaireRoute(baseURL + '/questionnaires');

test.describe('Questionnaire with Questions', () => {
  test('Create questionnaire with questions', async ({ request }) => {
    const createRequest: CreateQuestionnaireRequest = {
      title: 'Survey',
      description: 'Customer satisfaction survey',
      content: [
        {
          id: 'q1',
          title: 'Welcome Message',
          questionType: 'Message',
          subQuestions: [],
        } as MessageQuestion,
        {
          id: 'q2',
          title: 'Your Name',
          description: 'Please enter your full name',
          questionType: 'Text',
          subQuestions: [],
        } as TextQuestion,
        {
          id: 'q3',
          title: 'Your Age',
          questionType: 'Number',
          subQuestions: [],
        } as NumberQuestion,
        {
          id: 'q4',
          title: 'Contact Email',
          questionType: 'Email',
          subQuestions: [],
        } as EmailQuestion,
      ],
    };

    const response = await request.post(route.create(), {
      data: createRequest,
    });

    expect(response.status()).toBe(201);
    const body: QuestionnaireUpdatedResponse = await response.json();
    expect(body.delta).toBeDefined();

    const id = body.delta.id;

    // Verify the created questionnaire
    const getResponse = await request.get(route.get(id));
    expect(getResponse.status()).toBe(200);

    const getBody: QuestionnaireResponse = await getResponse.json();
    expect(getBody.questionnaire.content).toHaveLength(4);
    expect(getBody.questionnaire.content[0].questionType).toBe('Message');
    expect(getBody.questionnaire.content[1].questionType).toBe('Text');
    expect(getBody.questionnaire.content[2].questionType).toBe('Number');
    expect(getBody.questionnaire.content[3].questionType).toBe('Email');
  });

  test('Create questionnaire with nested subquestions', async ({ request }) => {
    const createRequest: CreateQuestionnaireRequest = {
      title: 'Nested Survey',
      content: [
        {
          id: 'q1',
          title: 'Do you have a job?',
          questionType: 'Text',
          subQuestions: [
            {
              id: 'q1-1',
              title: 'What is your job title?',
              questionType: 'Text',
              subQuestions: [],
            } as TextQuestion,
            {
              id: 'q1-2',
              title: 'Years of experience',
              questionType: 'Number',
              subQuestions: [],
            } as NumberQuestion,
          ],
        } as TextQuestion,
      ],
    };

    const response = await request.post(route.create(), {
      data: createRequest,
    });

    expect(response.status()).toBe(201);
    const body: QuestionnaireUpdatedResponse = await response.json();
    const id = body.delta.id;

    // Verify nested structure
    const getResponse = await request.get(route.get(id));
    const getBody: QuestionnaireResponse = await getResponse.json();

    expect(getBody.questionnaire.content).toHaveLength(1);
    expect(getBody.questionnaire.content[0].subQuestions).toHaveLength(2);
    expect(getBody.questionnaire.content[0].subQuestions[0].title).toBe('What is your job title?');
  });
});
