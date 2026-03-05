import { test, expect } from '@playwright/test';
import { Patchable } from '@client/model/delta';
import { QuestionnaireDelta } from '@client/model/questionnaire';
import { QuestionDelta } from '@client/model/question';
import type { Questionnaire } from '@client/model/questionnaire';
import type { TextQuestion } from '@client/model/question';

test.describe('QuestionnaireDelta', () => {
  test('apply - updates title', () => {
    const questionnaire: Questionnaire = {
      id: 'q1',
      title: 'Original Title',
      description: 'Original description',
      content: [],
      createdAt: '2024-01-01T00:00:00Z',
    };

    const delta = new QuestionnaireDelta('q1');
    delta.title = Patchable.set('New Title');

    delta.apply(questionnaire);

    expect(questionnaire.title).toBe('New Title');
    expect(questionnaire.description).toBe('Original description');
  });

  test('apply - clears description', () => {
    const questionnaire: Questionnaire = {
      id: 'q1',
      title: 'Title',
      description: 'Some description',
      content: [],
      createdAt: '2024-01-01T00:00:00Z',
    };

    const delta = new QuestionnaireDelta('q1');
    delta.description = Patchable.clear<string | null>();

    delta.apply(questionnaire);

    expect(questionnaire.title).toBe('Title');
    expect(questionnaire.description).toBeNull();
  });

  test('apply - updates multiple properties', () => {
    const questionnaire: Questionnaire = {
      id: 'q1',
      title: 'Original',
      description: 'Old',
      content: [],
      createdAt: '2024-01-01T00:00:00Z',
    };

    const delta = new QuestionnaireDelta('q1');
    delta.title = Patchable.set('Updated Title');
    delta.description = Patchable.set('Updated Description');

    delta.apply(questionnaire);

    expect(questionnaire.title).toBe('Updated Title');
    expect(questionnaire.description).toBe('Updated Description');
  });

  test('apply - throws error for mismatched id', () => {
    const questionnaire: Questionnaire = {
      id: 'q1',
      title: 'Title',
      content: [],
      createdAt: '2024-01-01T00:00:00Z',
    };

    const delta = new QuestionnaireDelta('q2');
    delta.title = Patchable.set('New Title');

    expect(() => delta.apply(questionnaire)).toThrow('Delta Id does not match Questionnaire Id');
  });

  test('merge - combines multiple deltas', () => {
    const delta1 = new QuestionnaireDelta('q1');
    delta1.title = Patchable.set('First Title');

    const delta2 = new QuestionnaireDelta('q1');
    delta2.description = Patchable.set('Second Description');

    const delta3 = new QuestionnaireDelta('q1');
    delta3.title = Patchable.set('Third Title');

    const merged = QuestionnaireDelta.merge('q1', delta1, delta2, delta3);

    expect(merged.id).toBe('q1');
    expect(merged.title.value).toBe('Third Title');
    expect(merged.description.value).toBe('Second Description');
  });

  test('merge - later deltas override earlier ones', () => {
    const delta1 = new QuestionnaireDelta('q1');
    delta1.title = Patchable.set('First');
    delta1.description = Patchable.set('First Desc');

    const delta2 = new QuestionnaireDelta('q1');
    delta2.title = Patchable.set('Second');

    const merged = QuestionnaireDelta.merge('q1', delta1, delta2);

    expect(merged.title.value).toBe('Second');
    expect(merged.description.value).toBe('First Desc');
  });

  test('merge - throws error for mismatched ids', () => {
    const delta1 = new QuestionnaireDelta('q1');
    const delta2 = new QuestionnaireDelta('q2');

    expect(() => QuestionnaireDelta.merge('q1', delta1, delta2)).toThrow(
      'All deltas must have the same Id'
    );
  });
});

test.describe('QuestionDelta', () => {
  test('apply - updates question title', () => {
    const question: TextQuestion = {
      id: 'q1',
      title: 'Original Question',
      description: 'Original',
      subQuestions: [],
      questionType: 'Text',
    };

    const delta = new QuestionDelta('q1');
    delta.title = Patchable.set('Updated Question');

    delta.apply(question);

    expect(question.title).toBe('Updated Question');
    expect(question.description).toBe('Original');
  });

  test('apply - clears question description', () => {
    const question: TextQuestion = {
      id: 'q1',
      title: 'Question',
      description: 'Has description',
      subQuestions: [],
      questionType: 'Text',
    };

    const delta = new QuestionDelta('q1');
    delta.description = Patchable.clear<string | null>();

    delta.apply(question);

    expect(question.title).toBe('Question');
    expect(question.description).toBeNull();
  });

  test('apply - throws error for mismatched id', () => {
    const question: TextQuestion = {
      id: 'q1',
      title: 'Question',
      subQuestions: [],
      questionType: 'Text',
    };

    const delta = new QuestionDelta('q2');

    expect(() => delta.apply(question)).toThrow('Delta Id does not match Question Id');
  });
});
