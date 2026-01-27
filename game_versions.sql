-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
-- Maze: Identity
-- Version history and exhibition build records

CREATE TABLE game_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version TEXT NOT NULL,
    label TEXT NOT NULL,
    release_type TEXT CHECK (
        release_type IN ('internal', 'exhibition', 'public')
    ),
    playable_scenes INTEGER,
    ui_status TEXT,
    notes TEXT,
    release_date TEXT
);

INSERT INTO game_versions (
    version,
    label,
    release_type,
    playable_scenes,
    ui_status,
    notes,
    release_date
) VALUES
(
    '0.2.3',
    'Internal Test Build',
    'internal',
    4,
    'unstable',
    'Early structural validation build',
    '2024-10-01'
),
(
    '0.4.3',
    'Full Scene Internal Test',
    'internal',
    7,
    'testing',
    'First complete scene flow with checkpoint and animation testing',
    '2024-12-15'
),
(
    '1.0.2',
    'Exhibition Stable Build',
    'exhibition',
    7,
    'stable',
    'Prepared for exhibition; known minor animation reload issue',
    '2025-01-20'
);
