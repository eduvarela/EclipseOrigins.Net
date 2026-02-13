# Network Protocol Standard

## Scope
This document defines the application protocol used between the MonoGame client and socket-based .NET server.

## Message Categories
All protobuf contracts must be categorized for routing, validation, and compatibility checks:

1. **Session & Handshake**
   - connect, version negotiation, authentication, keepalive/ping.
2. **Command (Client -> Server)**
   - player intent (movement, interaction, inventory actions, chat send).
3. **State Snapshot/Delta (Server -> Client)**
   - authoritative world state, entity updates, map/region streaming.
4. **Event/Broadcast (Server -> Client)**
   - combat outcomes, loot events, social/system notifications.
5. **Control & Diagnostics (Both ways, restricted)**
   - protocol errors, throttling notices, admin/ops messages.

## Framing Assumptions
- Transport is reliable sockets (TCP or equivalent reliable stream).
- Each protobuf payload is sent as a **length-prefixed frame**:
  - 4-byte unsigned length prefix (network byte order), followed by raw protobuf bytes.
- Receivers must support partial reads and reassembly before decode.
- Maximum accepted frame size must be explicitly bounded server-side to mitigate abuse.

## Versioning Policy
- Protocol uses semantic version tuple: `MAJOR.MINOR.PATCH`.
- **MAJOR** increments for incompatible wire changes.
- **MINOR** increments for backward-compatible field additions/new messages.
- **PATCH** increments for non-wire clarifications (docs/validation semantics) only.

### Protobuf Schema Rules
- Never reuse removed field numbers.
- Reserve field numbers/names when deprecating.
- Additive-only changes in a MAJOR line (new optional fields/messages).
- Required fields are disallowed for gameplay messages; use optional with validation rules.

## Compatibility Strategy
- Server advertises supported protocol range at handshake (`minSupported`, `maxSupported`).
- Client sends its protocol version before login.
- Connection is accepted only when client version is within server range.
- During active development, server may support **N and N-1 MINOR** versions in same MAJOR.
- If version mismatch occurs, server returns explicit incompatibility response and closes gracefully.

## Validation & Safety
- All incoming command messages are validated against authoritative world rules.
- Unknown protobuf fields must be ignored (forward compatibility) unless security policy blocks the message category.
- Server enforces per-category rate limits and disconnect thresholds for malformed frames.

## Evolution Workflow
1. Update protobuf contract in `Shared/Protocol`.
2. Record compatibility impact and version bump decision in this document.
3. Regenerate code and update both client/server handlers.
4. Add regression tests for mixed-version handshake behavior.
