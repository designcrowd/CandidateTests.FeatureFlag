CREATE TABLE IF NOT EXISTS Flags (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    Key             TEXT    NOT NULL UNIQUE,
    Name            TEXT    NOT NULL,
    Enabled         INTEGER NOT NULL,
    RolloutPercent  INTEGER NOT NULL,
    CreatedAt       TEXT    NOT NULL,
    UpdatedAt       TEXT    NOT NULL
);
