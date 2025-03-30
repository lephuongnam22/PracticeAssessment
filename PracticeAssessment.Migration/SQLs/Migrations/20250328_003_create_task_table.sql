CREATE TABLE [dbo].[Tasks] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Title] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Status] INT NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NULL,
    [CreatedBy] int NOT NULL,
    [ModifiedBy] int NULL,
);

-- Create an index for the Status column
CREATE INDEX IX_Tasks_Status ON [dbo].[Tasks]([Status]);

-- Create an index for the DueDate column
CREATE INDEX IX_Tasks_DueDate ON [dbo].[Tasks]([DueDate]);