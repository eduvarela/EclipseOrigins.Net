# Eclipse Origins Modern - Protocol v1

## Transport framing

TCP frame format:

- `uint32 length` (body length in bytes)
- `uint16 msgType`
- `payload` (protobuf encoded)

## Versioning

- Handshake includes `protocol_version` and `feature_flags`.
- Server can reject incompatible clients with a message.
- Backward-compatible additions should use optional/new fields only.

## Message groups

- Auth/session: handshake, login, enter-world.
- World simulation: movement input, authoritative snapshots.

## v1 baseline messages

- `HandshakeRequest` / `HandshakeResponse`
- `LoginRequest` / `LoginResponse`
- `EnterWorldRequest` / `EnterWorldResponse`
- `ClientInputMove`
- `ServerSnapshot`
