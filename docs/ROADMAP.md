# Eclipse Origins Modern - Roadmap

## P0 Discovery + Contracts
- Baseline .NET solution and projects.
- Shared coding conventions and architecture docs.
- Protocol v1 protobuf definitions.
- TCP frame codec + tests.

## P1 Vertical Slice
- TCP server/session pipeline.
- Handshake/login flow.
- Authoritative movement with snapshots.
- MonoGame client scenes + online movement.

## P2 Gameplay Core
- ECS foundations.
- Persistence with PostgreSQL + Dapper.
- NPCs and basic interaction.

## P3 Networking Hardening
- Rate limiting and abuse detection.
- Snapshot deltas and interpolation.

## P4 Productionization
- CI workflows, artifacts.
- Docker compose/observability.
- Mobile build pipeline docs.
