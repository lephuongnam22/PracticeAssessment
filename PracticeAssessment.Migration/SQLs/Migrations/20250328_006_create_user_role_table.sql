--Table for IdentityUserLogin<int>
CREATE TABLE IdentityUserLogins(
    LoginProvider NVARCHAR(450) NOT NULL,
    ProviderKey NVARCHAR(450) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX),
    UserId INT NOT NULL,
    PRIMARY KEY(LoginProvider, ProviderKey),
    FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

--Table for IdentityUserRole<int>
CREATE TABLE [dbo].AspNetUserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE
);

--Table for IdentityUserToken<int>
CREATE TABLE IdentityUserTokens(
    UserId INT NOT NULL,
    LoginProvider NVARCHAR(450) NOT NULL,
    Name NVARCHAR(450) NOT NULL,
    Value NVARCHAR(MAX),
    PRIMARY KEY(UserId, LoginProvider, Name),
    FOREIGN KEY(UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserClaims (
    Id INT NOT NULL IDENTITY(1,1),
    UserId INT NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetRoleClaims (
    Id INT NOT NULL IDENTITY(1,1),
    RoleId INT NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);
