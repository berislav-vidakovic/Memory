DROP TABLE IF EXISTS Health;
DROP TABLE IF EXISTS refresh_tokens;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS localization;

CREATE TABLE Health (Id INT IDENTITY(1,1) PRIMARY KEY, Msg NVARCHAR(50));


CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    password_hash VARCHAR(255) NOT NULL,
    login VARCHAR(100) NOT NULL UNIQUE,
    full_name VARCHAR(255) NULL,
    isonline BIT NOT NULL DEFAULT 0
);


CREATE TABLE refresh_tokens (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    token VARCHAR(255) NOT NULL,
    expires_at DATETIME2 NOT NULL,
    CONSTRAINT fk_user FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE
);


CREATE TABLE localization (
    id INT IDENTITY(1,1) PRIMARY KEY,
    paramkey VARCHAR(100) NOT NULL,
    paramvalue VARCHAR(100) NOT NULL,
    lang VARCHAR(2) NOT NULL,
    CONSTRAINT uk_param_lang UNIQUE (paramkey, lang)
);

