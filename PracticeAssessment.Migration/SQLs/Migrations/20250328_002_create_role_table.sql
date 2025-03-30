CREATE TABLE [dbo].[AspNetRoles] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(256) NULL,
    [NormalizedName] NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL
);