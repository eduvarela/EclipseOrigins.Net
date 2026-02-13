CREATE TABLE IF NOT EXISTS accounts (
    id UUID PRIMARY KEY,
    username TEXT NOT NULL UNIQUE,
    password_algorithm TEXT NOT NULL,
    password_iterations INTEGER NOT NULL,
    password_salt TEXT NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_accounts_username ON accounts (username);
