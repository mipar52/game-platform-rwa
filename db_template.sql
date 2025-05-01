-- GAME PLATFORM DB

create database GamePlatformRWA

use GamePlatformRWA

go

-- USER
CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL
);

-- GAME TYPE
CREATE TABLE GameType (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- GAME GENRE
CREATE TABLE Genre (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- GAME
CREATE TABLE Game (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000),
    ReleaseDate DATE,
    GameUrl NVARCHAR(255),
    GameTypeId INT NOT NULL,
    FOREIGN KEY (GameTypeId) REFERENCES GameType(Id)
);

-- GAME + GAME GENRE table
CREATE TABLE GameGenre (
    GameId INT NOT NULL,
    GenreId INT NOT NULL,
    PRIMARY KEY (GameId, GenreId),
    FOREIGN KEY (GameId) REFERENCES Game(Id) ON DELETE CASCADE,
    FOREIGN KEY (GenreId) REFERENCES Genre(Id) ON DELETE CASCADE
);

-- USER + GAME review
CREATE TABLE Review (
    UserId INT NOT NULL,
    GameId INT NOT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 10),
    ReviewText NVARCHAR(2000),
    Approved BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, GameId),
    FOREIGN KEY (UserId) REFERENCES [User](Id) ON DELETE CASCADE,
    FOREIGN KEY (GameId) REFERENCES Game(Id) ON DELETE CASCADE
);



--- data generation ---
INSERT INTO [User] VALUES (1, N'gamer01', N'gamer01@example.com', N'hash1');
INSERT INTO [User] VALUES (2, N'gamer02', N'gamer02@example.com', N'hash2');
INSERT INTO [User] VALUES (3, N'admin', N'admin@example.com', N'adminhash');

INSERT INTO GameType VALUES (1, N'Singleplayer');
INSERT INTO GameType VALUES (2, N'Multiplayer');
INSERT INTO GameType VALUES (3, N'Co-op');

INSERT INTO Genre VALUES (1, N'Action');
INSERT INTO Genre VALUES (2, N'Adventure');
INSERT INTO Genre VALUES (3, N'RPG');
INSERT INTO Genre VALUES (4, N'Strategy');
INSERT INTO Genre VALUES (5, N'Simulation');

INSERT INTO Game VALUES (1, N'The Witcher 3', N'Story-rich RPG', N'2015-05-19', N'http://example.com/witcher3', 1);
INSERT INTO Game VALUES (2, N'League of Legends', N'Competitive MOBA game', N'2009-10-27', N'http://example.com/lol', 2);
INSERT INTO Game VALUES (3, N'Stardew Valley', N'Relaxing farming sim', N'2016-02-26', N'http://example.com/stardew', 3);

INSERT INTO GameGenre VALUES (1, 1);
INSERT INTO GameGenre VALUES (1, 3);
INSERT INTO GameGenre VALUES (2, 1);
INSERT INTO GameGenre VALUES (2, 4);
INSERT INTO GameGenre VALUES (3, 5);

INSERT INTO Review VALUES (1, 1, 10, N'Amazing story and characters.', 1, '2025-04-21 13:00:04');
INSERT INTO Review VALUES (2, 2, 7, N'Fun with friends, but toxic community.', 0, '2025-04-26 13:00:04');
INSERT INTO Review VALUES (1, 3, 9, N'Very relaxing and enjoyable gameplay.', 1, '2025-04-29 13:00:04');