# Eclipse Origins Modern Architecture

## Purpose
This document defines module boundaries and dependency direction for the modernized .NET codebase.

## Module Boundaries

### Shared
**Owns:** domain contracts, protocol contracts, and cross-runtime abstractions.

**Contains**
- DTOs and enums shared by client/server.
- Protocol schema definitions and generated message contracts.
- Common primitives (IDs, result types, time abstraction interfaces).

**Does not contain**
- MonoGame rendering/UI concerns.
- Socket hosting or persistence adapters.
- Any gameplay authority implementation.

---

### Client
**Owns:** presentation and player interaction.

**Contains**
- MonoGame bootstrap and game loop integration.
- Input mapping, HUD composition, camera/view state.
- Network session client and protocol message handling for display.

**Does not contain**
- Authoritative simulation or trust decisions.
- Persistence write logic for canonical world state.
- References to server implementation projects.

---

### Server
**Owns:** authoritative gameplay and operations.

**Contains**
- Socket endpoints, session lifecycle, and message dispatch.
- Authoritative simulation/tick processing.
- Persistence orchestration, observability, and administrative controls.

**Does not contain**
- UI/MonoGame dependencies.
- References to client implementation projects.

## Dependency Direction
The dependency graph must remain acyclic and directed inward toward Shared:

1. `Client/*` -> `Shared/*`
2. `Server/*` -> `Shared/*`
3. `Shared/*` -> no dependency on runtime modules

### Forbidden Edges
- `Client/*` -> `Server/*`
- `Server/*` -> `Client/*`
- `Shared/*` -> `Client/*` or `Server/*`

## Layered Runtime View
- **Client process:** visual frame/render loop + input capture + protocol consumption.
- **Server process:** authoritative tick loop + rules validation + persistence coordination.
- **Shared package(s):** wire contracts and common domain vocabulary used by both processes.

## Architecture Decision Rules
- Any new cross-boundary type starts in Shared first.
- Any protocol shape change requires update in `docs/PROTOCOL.md`.
- Any new module must declare owner (Shared/Client/Server) and dependency direction before merge.
