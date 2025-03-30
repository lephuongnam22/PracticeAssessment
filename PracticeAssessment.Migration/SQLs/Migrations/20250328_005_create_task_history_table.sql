CREATE TABLE[dbo].[TaskHistory] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TaskId] INT NOT NULL,
    [NewStatus] INT NOT NULL,
    [OldStatus] INT NOT NULL,
    [ChangedDate] DATETIME2 NOT NULL,
    [UserId] int NOT NULL,
    FOREIGN KEY ([TaskId]) REFERENCES[dbo].[Tasks]([Id]) ON DELETE NO ACTION,
    FOREIGN KEY ([UserId]) REFERENCES[dbo].[Users]([Id]) ON DELETE NO ACTION
);