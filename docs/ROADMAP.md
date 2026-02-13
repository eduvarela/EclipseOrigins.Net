# Modernization Roadmap (P0-P4)

This roadmap translates the project brief into phased deliverables for a .NET LTS, MonoGame, socket/protobuf architecture.

## P0 - Foundation & Standards

### Deliverables
- Repository standards artifacts (`.editorconfig`, `Directory.Build.props`, architecture/protocol/roadmap docs).
- Solution baseline compiling on .NET LTS.
- Initial project boundaries: Shared, Client, Server.

### Quality Gates
- `dotnet build` succeeds for all projects.
- No boundary violations in project references (Client/Server only depend on Shared).
- Core technical decisions documented and reviewed.

## P1 - Transport & Handshake Vertical Slice

### Deliverables
- Socket listener and client connector operational.
- Protobuf contract definitions for handshake, ping, and simple authenticated session start.
- End-to-end ping/pong and version negotiation path.

### Quality Gates
- Automated integration test for connect -> handshake -> ping.
- Protocol mismatch path returns explicit error and graceful disconnect.
- Basic connection metrics/logging available.

## P2 - Authoritative Gameplay Core

### Deliverables
- Server-side authoritative tick loop and player session state.
- Client receives world snapshot/delta updates and renders a minimal scene in MonoGame.
- Command flow for movement and at least one interaction.

### Quality Gates
- Deterministic simulation tests for core movement rules.
- Soak test proving server stability under concurrent sessions target from brief.
- No client-side authority for gameplay-critical state.

## P3 - Content Systems & Persistence

### Deliverables
- Inventory/combat/chat baseline systems over protocol contracts.
- Persistence integration for accounts/characters/world progress.
- Basic admin/ops controls (kick, broadcast, maintenance mode).

### Quality Gates
- Migration-safe persistence schema with rollback plan.
- Recovery test for server restart without progress loss.
- Security checks for auth/session/token handling complete.

## P4 - Production Readiness & Expansion

### Deliverables
- Deployment pipeline, environment configuration strategy, and observability dashboards.
- Performance tuning for target CCU, packet budgets, and tick latency goals from brief.
- Backward-compatible protocol evolution process validated in release workflow.

### Quality Gates
- Load/perf results meet brief SLO targets (latency, uptime, memory/CPU envelope).
- Runbook coverage for incident response and rolling upgrade.
- Release candidate sign-off across engineering + design stakeholders.

## Ongoing Exit Criteria Across All Phases
- Documentation updated in lockstep with architecture/protocol changes.
- Cross-module dependency rules remain enforced.
- Every phase ships with measurable test evidence and explicit risk register updates.
