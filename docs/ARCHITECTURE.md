# Eclipse Origins Modern Architecture

## Goals
- Keep domain and transport contracts shared between client/server while maintaining strict ownership boundaries.
- Support a **.NET LTS** toolchain with a **MonoGame** presentation client and a **socket-based** authoritative server.
- Keep protocol contracts transport-agnostic and generated from **protobuf** schemas.

## Project Modules

### Shared
**Projects:** `src/Shared/Shared.Abstractions`, `src/Shared/Protocol`

**Responsibilities**
- Canonical DTO/message contracts and shared enums/constants.
- Cross-cutting abstractions used by both runtime sides (time provider, ID abstractions, common result types).
- Serialization contract definitions (protobuf message definitions and generated stubs).

**Must not contain**
- Rendering code, MonoGame references, or client UX concerns.
- Server-only infrastructure (socket loop, persistence adapters, hosted services).

---

### Client
**Project:** `src/Client/GameClient`

**Responsibilities**
- MonoGame application bootstrap and frame loop.
- Input mapping, camera/state presentation, HUD/UI composition.
- Client session orchestration (connect/auth/world stream consumption) using shared protocol contracts.

**Must not contain**
- Authoritative simulation logic.
- Direct references to `Server/*` projects.

---

### Server
**Project:** `src/Server/GameServer`

**Responsibilities**
- Socket listener/session lifecycle and message dispatch.
- Authoritative world simulation and gameplay rules.
- Persistence integration and server-side observability.

**Must not contain**
- MonoGame/UI references.
- Direct references to `Client/*` projects.

## Dependency Direction Rules
Allowed references are intentionally one-directional:

1. `Client/*` -> `Shared/*`
2. `Server/*` -> `Shared/*`
3. `Shared/Protocol` -> `Shared.Abstractions` (or sibling Shared contracts)

Forbidden:
- `Shared/*` -> `Client/*` or `Server/*`
- `Client/*` <-> `Server/*` direct project references
- Any runtime project directly depending on generated protocol internals that bypass shared contracts

## Runtime Topology
- **Client process:** MonoGame runtime on .NET LTS, drives render/input at frame cadence.
- **Server process:** .NET LTS background service hosting socket endpoints and authoritative tick loop.
- **Wire:** length-delimited protobuf payloads carried over reliable sockets.

## Architectural Decision Standards
- New cross-boundary data must be introduced in `Shared/Protocol` first.
- Any change that alters network payload shape requires protocol versioning notes in `docs/PROTOCOL.md`.
- Any new project must declare ownership under one of: Shared, Client, Server.
