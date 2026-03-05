/**
 * Base interface for update commands
 */
export interface UpdateCommand {
  type: string;
  id: string;
}

/**
 * Command to update questionnaire properties
 */
export interface UpdateQuestionnaireProperty extends UpdateCommand {
  type: 'updateProperty';
  title?: string;
  description?: string;
}

/**
 * Type guard to check if a command is UpdateQuestionnaireProperty
 */
export function isUpdateQuestionnaireProperty(
  command: UpdateCommand
): command is UpdateQuestionnaireProperty {
  return command.type === 'updateProperty';
}

/**
 * Factory function to create an UpdateQuestionnaireProperty command
 */
export function createUpdateQuestionnaireProperty(
  id: string,
  title?: string,
  description?: string
): UpdateQuestionnaireProperty {
  return {
    type: 'updateProperty',
    id,
    title,
    description,
  };
}
