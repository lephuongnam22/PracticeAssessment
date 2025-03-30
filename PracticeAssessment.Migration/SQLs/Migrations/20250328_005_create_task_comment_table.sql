CREATE TABLE[dbo].[TaskComments] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TaskId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Comment] NVARCHAR(MAX) NOT NULL,
    [CommentDate] DATETIME2 NOT NULL,
    FOREIGN KEY ([TaskId]) REFERENCES[dbo].[Tasks]([Id]) ON DELETE NO ACTION,
    FOREIGN KEY ([UserId]) REFERENCES[dbo].[Users]([Id]) ON DELETE NO ACTION
);