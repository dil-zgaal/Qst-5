export class QuestionnaireRoute {
  constructor(private baseUrl: string = '/questionnaires') {}

  list(): string {
    return this.baseUrl;
  }

  get(id: string): string {
    return `${this.baseUrl}/${encodeURIComponent(id)}`;
  }

  create(): string {
    return this.baseUrl;
  }

  update(id: string): string {
    return `${this.baseUrl}/${encodeURIComponent(id)}`;
  }

  delete(id: string): string {
    return `${this.baseUrl}/${encodeURIComponent(id)}`;
  }
}
