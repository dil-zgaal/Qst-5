import { test, expect } from '@playwright/test';
import { QuestionnaireRoute } from '@client/api/questionnaireRoute';
import type { CreateQuestionnaireRequest, UpdateQuestionnaireRequest } from '@client/api/requests';
import type {
  QuestionnaireResponse,
  QuestionnaireUpdatedResponse,
  QuestionnaireListResponse,
} from '@client/api/responses';

const baseURL = process.env.API_BASE_URL || 'http://localhost:5000';
const route = new QuestionnaireRoute(baseURL + '/questionnaires');

test.describe('Questionnaire API', () => {
  test('POST /questionnaires - Create questionnaire', async ({ request }) => {
    const requestData: CreateQuestionnaireRequest = {
      title: 'Test Questionnaire',
      description: 'A test questionnaire',
      content: [],
    };

    const response = await request.post(route.create(), {
      data: requestData,
    });

    expect(response.status()).toBe(201);
    const body: QuestionnaireUpdatedResponse = await response.json();
    expect(body).toHaveProperty('delta');
    expect(body.delta).toHaveProperty('id');
    expect(body.delta.title).toBe('Test Questionnaire');
  });

  test('GET /questionnaires - List all questionnaires', async ({ request }) => {
    const response = await request.get(route.list());

    expect(response.status()).toBe(200);
    const body: QuestionnaireListResponse = await response.json();
    expect(body).toHaveProperty('questionnaires');
    expect(Array.isArray(body.questionnaires)).toBe(true);
  });

  test('GET /questionnaires/{id} - Get questionnaire by id', async ({ request }) => {
    // First create a questionnaire
    const createRequest: CreateQuestionnaireRequest = {
      title: 'Questionnaire to Get',
      description: 'Test description',
      content: [],
    };

    const createResponse = await request.post(route.create(), {
      data: createRequest,
    });

    const createBody: QuestionnaireUpdatedResponse = await createResponse.json();
    const id = createBody.delta.id;

    // Then get it
    const response = await request.get(route.get(id));

    expect(response.status()).toBe(200);
    const body: QuestionnaireResponse = await response.json();
    expect(body).toHaveProperty('questionnaire');
    expect(body.questionnaire.id).toBe(id);
    expect(body.questionnaire.title).toBe('Questionnaire to Get');
    expect(body.questionnaire.description).toBe('Test description');
  });

  test('PATCH /questionnaires/{id} - Update questionnaire', async ({ request }) => {
    // First create a questionnaire
    const createRequest: CreateQuestionnaireRequest = {
      title: 'Original Title',
      description: 'Original description',
      content: [],
    };

    const createResponse = await request.post(route.create(), {
      data: createRequest,
    });

    const createBody: QuestionnaireUpdatedResponse = await createResponse.json();
    const id = createBody.delta.id;

    // Then update it
    const updateRequest: UpdateQuestionnaireRequest = {
      title: 'Updated Title',
      description: 'Updated description',
    };

    const response = await request.patch(route.update(id), {
      data: updateRequest,
    });

    expect(response.status()).toBe(200);
    const body: QuestionnaireUpdatedResponse = await response.json();
    expect(body).toHaveProperty('delta');
    expect(body.delta.title).toBe('Updated Title');
    expect(body.delta.description).toBe('Updated description');
  });

  test('DELETE /questionnaires/{id} - Delete questionnaire', async ({ request }) => {
    // First create a questionnaire
    const createRequest: CreateQuestionnaireRequest = {
      title: 'Questionnaire to Delete',
      content: [],
    };

    const createResponse = await request.post(route.create(), {
      data: createRequest,
    });

    const createBody: QuestionnaireUpdatedResponse = await createResponse.json();
    const id = createBody.delta.id;

    // Then delete it
    const response = await request.delete(route.delete(id));

    expect(response.status()).toBe(204);

    // Verify it's deleted
    const getResponse = await request.get(route.get(id));
    expect(getResponse.status()).toBe(404);
  });

  test('GET /questionnaires/{id} - Returns 404 for non-existent id', async ({ request }) => {
    const response = await request.get(route.get('non-existent-id'));
    expect(response.status()).toBe(404);
  });

  test('POST /questionnaires - Returns 400 for missing title', async ({ request }) => {
    const response = await request.post(route.create(), {
      data: {
        description: 'No title',
        content: [],
      },
    });

    expect(response.status()).toBe(400);
  });
});
