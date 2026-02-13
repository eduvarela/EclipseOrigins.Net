# Eclipse Origins Modern - Architecture

## Module boundaries

- `Shared.Abstractions`: cross-cutting primitives (network framing, ECS foundations, common contracts).
- `Shared.Protocol`: protobuf contracts and generated message types.
- `GameClient`: app bootstrap and scene orchestration.
- `GameServer`: host/bootstrap process.
- `GameServer.Core`: domain/gameplay logic.
- `GameServer.Net`: socket transport, session pipeline, dispatch.
- `GameServer.Persistence`: PostgreSQL access and repositories.

## Dependency direction

- Client/Server depend on Shared projects.
- `GameServer` composes `GameServer.Core`, `GameServer.Net`, `GameServer.Persistence`.
- `GameServer.Core` must not depend on transport concerns.
- `GameServer.Net` should depend on abstractions/interfaces from core.

## Target runtime

- .NET LTS baseline.
- Cross-platform support: Windows/Linux/macOS for server+desktop client.
