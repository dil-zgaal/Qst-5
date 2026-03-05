# Questionnaire API Tests

Playwright-based API tests for the Questionnaire API.

## Setup

```bash
pnpm install
```

## Running Tests

```bash
# Run all tests
pnpm test

# Run with UI
pnpm test:ui

# Debug mode
pnpm test:debug
```

## Configuration

Set `API_BASE_URL` environment variable to test against a different endpoint:

```bash
API_BASE_URL=https://api.example.com pnpm test
```
