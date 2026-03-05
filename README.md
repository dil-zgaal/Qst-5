# Questionnaires - Cooperative Editing System

A .NET 8.0 Minimal API backend with TypeScript client models demonstrating **Command Sourcing** and **Delta-based Cooperative Editing**.

## Overview

This system enables multiple clients to collaboratively edit questionnaires through:

- **Ordered Command Storage**: All updates stored as sequential, versioned commands
- **Lock-based Processing**: Commands processed exactly once with distributed locking
- **Delta Responses**: Clients receive incremental changes instead of full state
- **Version-based Concurrency**: Optimistic locking using version numbers
- **Patchable Pattern**: Type-safe partial updates distinguishing "not changed" from "set" from "clear"

## Architecture

### Command Sourcing Model

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │ UpdateCommand (title, description)
       │ fromVersion: 5
       ▼
┌─────────────────────────────────────────┐
│           Command Storage               │
│  ┌─────┬─────┬─────┬─────┬─────┬─────┐ │
│  │ v1  │ v2  │ v3  │ v4  │ v5  │ v6  │ │
│  ├─────┼─────┼─────┼─────┼─────┼─────┤ │
│  │Cmd  │Cmd  │Cmd  │Cmd  │Cmd  │Cmd  │ │
│  │Delta│Delta│Delta│Delta│Delta│ ?   │ │
│  │Lock │Lock │Lock │Lock │Lock │Lock │ │
│  └─────┴─────┴─────┴─────┴─────┴─────┘ │
└─────────────────────────────────────────┘
       │
       │ Command Processing (async)
       ▼
┌─────────────────┐
│ Command Handler │ ──► Generate Delta
└─────────────────┘
       │
       │ QuestionnaireDelta (v5→v6)
       ▼
┌─────────────────┐
│ Store Effect    │ ──► Release Lock
└─────────────────┘
       │
       │ Aggregated Delta (v5→v6)
       ▼
┌─────────────┐
│   Client    │ ──► Apply to local state
└─────────────┘
```

## Cooperative Editing Flow

### Scenario: Two clients editing simultaneously

```
Client A (v5)                    Server                    Client B (v5)
    │                               │                           │
    │ PATCH /questionnaires/abc     │                           │
    │ version=5                     │                           │
    │ {title: "New Title"}          │                           │
    ├──────────────────────────────►│                           │
    │                               │ Store Command → v6        │
    │                               │ Lock Command v6           │
    │                               │ Process Command           │
    │                               │ Generate Delta (v5→v6)    │
    │                               │ Store Effect              │
    │ ◄─────────────────────────────┤                           │
    │ Delta {                       │                           │
    │   fromVersion: 5,             │                           │
    │   toVersion: 6,               │                           │
    │   title: Set("New Title")     │                           │
    │ }                             │                           │
    │                               │                           │
    │                               │     PATCH /questionnaires/abc
    │                               │     version=5              │
    │                               │     {description: "..."}   │
    │                               │◄───────────────────────────┤
    │                               │ Store Command → v7         │
    │                               │ Process Command            │
    │                               │ Generate Delta (v6→v7)     │
    │                               ├───────────────────────────►│
    │                               │           Delta {          │
    │                               │   fromVersion: 6,          │
    │                               │   toVersion: 7,            │
    │                               │   description: Set("...")  │
    │                               │ }                          │
    │                               │                           │
    │ GET /questionnaires/abc?from=5│                           │
    ├──────────────────────────────►│                           │
    │ ◄─────────────────────────────┤                           │
    │ Aggregated Delta {            │                           │
    │   fromVersion: 5,             │                           │
    │   toVersion: 7,               │                           │
    │   title: Set("New Title"),    │                           │
    │   description: Set("...")     │                           │
    │ }                             │                           │
    │                               │                           │
```

### Key Points

1. **Client sends command with current version** - Optimistic concurrency control
2. **Server assigns next sequential version** - Commands ordered globally
3. **Async processing** - Command stored immediately, processed asynchronously
4. **Delta returned** - Only changes, not full state
5. **Client A polls/subscribes** - Gets aggregated delta (v5→v7) including both changes
6. **Automatic conflict resolution** - Last-write-wins per field via Patchable pattern

## Data Flow Details

### Command Storage Structure

```
QuestionnaireStream {
  commands: [
    CommandData {
      commandJson: '{"type":"updateProperty","title":"..."}',
      effectJson: '{"id":"...","fromVersion":0,"toVersion":1,...}',
      lockId: null,
      lockExpiration: null
    },
    CommandData {
      commandJson: '{"type":"updateProperty","title":"..."}',
      effectJson: null,  // ← Not processed yet
      lockId: "guid-123",
      lockExpiration: "2024-03-05T10:15:00Z"
    }
  ]
}

Version = Array Index + 1
```

### Lock Mechanism Flow

```
StartNextCommand()
    │
    ├─► Find first command without effect
    │
    ├─► Check lock status
    │   ├─► No lock? Acquire new lock
    │   └─► Has lock?
    │       ├─► Expired? Reuse lock
    │       └─► Valid? Skip to next
    │
    └─► Return (version, command, lockId)

CompleteCommand(version, delta, lockId)
    │
    ├─► Validate lockId matches
    │
    ├─► Check lock not expired
    │
    ├─► Store effect (delta)
    │
    └─► Release lock
```

## Patchable Pattern

### Two Distinct Types

**`Patchable<T>` - For non-nullable fields**
- States: `NotGiven`, `Set`
- Cannot be cleared (no null state)
- Used for: `Title`

**`PatchableNullable<T>` - For nullable fields**
- States: `NotGiven`, `Clear`, `Set`
- Can be set to null explicitly
- Used for: `Description`

### Example

```csharp
// .NET
var delta = new QuestionnaireDelta {
    Title = Patchable<string>.Set("New Title"),  // Will update
    Description = PatchableNullable<string>.NotGiven()  // Will not update
};

questionnaire.Apply(delta);
// Result: Title changed, Description unchanged
```

```typescript
// TypeScript
const delta = new QuestionnaireDelta(id, 5, 6, new Date().toISOString());
delta.title = Patchable.set("New Title");
delta.description = PatchableNullable.notGiven();

delta.apply(questionnaire);
// Result: Title changed, Description unchanged
```

### Why Patchable?

Without Patchable, you cannot distinguish:
- `{ title: undefined }` - Don't change title
- `{ title: "New Value" }` - Set title to "New Value"
- `{ description: null }` - Clear description (set to null)
- `{ description: undefined }` - Don't change description

Patchable makes these distinctions explicit and type-safe.

## API Endpoints

### Create Questionnaire
```http
POST /questionnaires
Content-Type: application/json

{
  "title": "Customer Survey",
  "description": "Annual satisfaction survey",
  "content": []
}

Response: 201 Created
{
  "delta": {
    "id": "abc-123",
    "fromVersion": 0,
    "toVersion": 0,
    "updatedAt": "2024-03-05T10:00:00Z",
    "title": { "isSet": true, "value": "Customer Survey" },
    "description": { "isSet": true, "value": "Annual satisfaction survey" }
  }
}
```

### Update Questionnaire
```http
PATCH /questionnaires/abc-123?version=5
Content-Type: application/json

{
  "title": "Updated Survey",
  "description": null
}

Response: 200 OK
{
  "delta": {
    "id": "abc-123",
    "fromVersion": 5,
    "toVersion": 6,
    "updatedAt": "2024-03-05T10:05:00Z",
    "title": { "isSet": true, "value": "Updated Survey" },
    "description": { "isClear": true }
  }
}
```

### Get Questionnaire
```http
GET /questionnaires/abc-123

Response: 200 OK
{
  "questionnaire": {
    "id": "abc-123",
    "version": 6,
    "title": "Updated Survey",
    "description": null,
    "content": [],
    "createdAt": "2024-03-05T10:00:00Z",
    "updatedAt": "2024-03-05T10:05:00Z"
  }
}
```

### Get Aggregated Delta (Sync)
```http
GET /questionnaires/abc-123/delta?from=5&to=10

Response: 200 OK
{
  "delta": {
    "id": "abc-123",
    "fromVersion": 5,
    "toVersion": 10,
    "updatedAt": "2024-03-05T10:10:00Z",
    "title": { "isSet": true, "value": "Final Title" },
    "description": { "isSet": true, "value": "Final Description" }
  }
}
```

## Project Structure

```
/backend
  /Core
    /Core.Model
      /Delta
        Patchable.cs              # Patchable<T> and PatchableNullable<T>
        PatchableArray.cs         # Array delta operations

  /Questionnaires
    /Questionnaires.Contract
      /Models
        Questionnaire.cs          # Domain model
        QuestionnaireDelta.cs     # Delta model
        /Commands
          UpdateCommand.cs
          UpdateQuestionnaireProperty.cs

      /Repositories
        IQuestionnaireRepository.cs
        IQuestionnaireCommands.cs # Command storage interface

    /Questionnaires.Domain
      /Repositories
        InMemoryQuestionnaireCommands.cs  # Command storage implementation
        InMemoryQuestionnaireRepository.cs

      /Services
        QuestionnaireUpdateService.cs  # Orchestrates command processing

      /Commands
        UpdateQuestionnairePropertyHandler.cs  # Command → Delta

      /Requests
        CreateQuestionnaire.cs
        UpdateQuestionnaire.cs
        GetQuestionnaire.cs
        ListQuestionnaires.cs
        QuestionnaireResponses.cs

/client-ts
  /model
    delta.ts                      # Patchable and PatchableNullable classes
    questionnaire.ts              # Questionnaire and QuestionnaireDelta
    commands.ts                   # UpdateCommand types
    question.ts                   # Question types

  /api
    requests.ts                   # Request DTOs
    responses.ts                  # Response DTOs
    questionnaireRoute.ts         # URL builders
```

## Concurrency Model

### Per-Questionnaire Locking
- Each questionnaire has independent command stream
- Commands within questionnaire processed sequentially
- Commands across questionnaires processed in parallel

### Lock Timeout & Recovery
- Locks expire after 5 minutes (configurable)
- Expired locks can be reacquired by another processor
- Prevents deadlock from crashed processors

### Idempotency
- Commands processed exactly once
- Effect stored atomically with lock release
- Duplicate processing prevented by lock validation

## Benefits

1. **Conflict-Free**: Sequential processing eliminates write conflicts
2. **Efficient Sync**: Clients only receive changes since their version
3. **Audit Trail**: Full command history preserved
4. **Offline Support**: Clients can queue commands and sync later
5. **Type Safety**: Patchable pattern prevents null/undefined ambiguity
6. **Scalability**: Per-questionnaire locking allows parallel processing
7. **Resilience**: Lock timeouts handle processor failures

## Technologies

- **.NET 8.0** - Backend API
- **Minimal APIs** - Lightweight routing
- **C# 12** - Required properties, pattern matching
- **TypeScript** - Type-safe client models
- **JSON Serialization** - Command/Delta storage
- **In-Memory Storage** - ConcurrentDictionary with fine-grained locking

## Future Enhancements

- [ ] Database persistence (PostgreSQL, SQL Server)
- [ ] WebSocket/SSE for real-time delta streaming
- [ ] Snapshots for efficient reads (Event Sourcing optimization)
- [ ] Conflict resolution strategies beyond last-write-wins
- [ ] Command validation and business rules
- [ ] PatchableArray for collections (questions, sub-questions)
- [ ] Multi-tenant isolation
- [ ] Command replay for debugging/testing

## License

MIT
