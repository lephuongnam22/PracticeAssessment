CREATE TABLE [dbo].[Notifications] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Message] NVARCHAR(256) NOT NULL,
    [CreatedAt] DateTime NOT NULL,
    [TaskId] INT NOT NULL,
    [UserId] INT NULL,
    HasNotified BIT NOT NULL DEFAULT 0,
    FOREIGN KEY ([TaskId]) REFERENCES[dbo].[Tasks]([Id]) ON DELETE NO ACTION,
    FOREIGN KEY ([UserId]) REFERENCES[dbo].[Users]([Id]) ON DELETE NO ACTION,
);