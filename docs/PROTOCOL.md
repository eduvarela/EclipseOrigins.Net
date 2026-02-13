# Network Protocol Policy

## Scope
This document defines protocol versioning, message categories, and compatibility strategy for client/server communication.

## Versioning Policy
Protocol versions use `MAJOR.MINOR.PATCH` semantics.

- **MAJOR**: breaking wire compatibility changes.
  - Examples: field renumbering, removed message types without fallback, framing changes.
- **MINOR**: additive backward-compatible changes.
  - Examples: new optional fields, new message types, additional enum values with safe default handling.
- **PATCH**: non-wire behavior clarification or bugfix notes.
  - Examples: stricter validation wording, docs corrections, operational guidance.

### Schema Rules
- Never reuse removed field numbers.
- Reserve field numbers and names when deprecating.
- Prefer optional/additive evolution within the same major line.
- Avoid required fields for gameplay traffic; enforce requirements via validation rules.

## Message Categories
All protocol messages must be explicitly classified:

1. **Handshake & Session**
   - Version negotiation, authentication bootstrap, keepalive.
2. **Client Commands**
   - Player intent (movement, inventory actions, interaction requests).
3. **World State**
   - Snapshots, deltas, and entity updates from server to client.
4. **Events & Broadcasts**
   - Combat outcomes, chat/system notifications, region/global events.
5. **Control & Diagnostics**
   - Throttling notices, protocol errors, admin/ops signaling.

Each category must define:
- Direction (`C->S`, `S->C`, or bidirectional).
- Rate/size limits.
- Validation and authorization expectations.

## Compatibility Strategy

### Negotiation
- Client sends `clientProtocolVersion` during initial handshake.
- Server responds with capability range: `minSupportedVersion` and `maxSupportedVersion`.
- Session proceeds only when client version falls within server range.

### Support Window
- Active releases should support `current MINOR` and `current MINOR - 1` within the same major when feasible.
- MAJOR mismatch is a hard reject.

### Mismatch Behavior
On incompatibility, server must:
1. Return explicit version mismatch response code.
2. Include supported range metadata.
3. Close connection gracefully.

## Framing and Safety
- Transport is reliable stream sockets.
- Payload framing is length-prefixed (`uint32` network byte order + protobuf payload).
- Receivers must support partial reads/reassembly.
- Server must enforce max frame size, malformed frame thresholds, and per-category rate limits.

## Change Workflow
1. Update shared protocol schema/contracts.
2. Classify the change (MAJOR/MINOR/PATCH) and record rationale here.
3. Regenerate/update both client and server handlers.
4. Add/refresh compatibility tests (handshake, mixed-version behavior, fallback paths).
