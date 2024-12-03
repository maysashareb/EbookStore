-- Create the Categories table
CREATE TABLE [dbo].[Categories] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Image] NVARCHAR(255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create the Books table
CREATE TABLE [dbo].[Books] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [BorrowPrice] DECIMAL(18, 2) NOT NULL,
    [CategoryId] INT NOT NULL,
    [ImageUrl] NVARCHAR(255) NULL,
    [Quantity] INT NOT NULL DEFAULT 0,
    [IsBorrowable] BIT NOT NULL DEFAULT 1,
    [IsDiscounted] BIT NOT NULL DEFAULT 0,
    [DiscountEndDate] DATETIME NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id]) ON DELETE CASCADE
);

-- Create the BookFormats table
CREATE TABLE [dbo].[BookFormats] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [BookId] INT NOT NULL,
    [Format] NVARCHAR(50) NOT NULL, -- e.g., EPUB, MOBI, PDF
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books]([Id]) ON DELETE CASCADE
);



-- Insert data into Categories table
INSERT INTO [dbo].[Categories] (Name, Image)
VALUES 
    ('Fiction', 'Content/advanture.jpg'),
    ('Science', 'Content/romance.jpg'),
    ('History', 'Content/fantasy.jpg'),
    ('Mystery', 'Content/kids.jpg');

-- Insert data into Books table
INSERT INTO [dbo].[Books] (Title, Description, Price, BorrowPrice, CategoryId, ImageUrl, Quantity, IsBorrowable, IsDiscounted, DiscountEndDate)
VALUES 
    ('thrilling adventure', 'Get your heart racing with our action-packed novels!', 9.99, 5.99, 1, 'images/advanture.jpg', 10, 1, 1, DATEADD(DAY, 7, GETDATE())),
    ('nikkis big weekend', 'Dive into love stories that will sweep you off your feet!', 14.99, 9.99, 2, 'Content/nikkis big weekend.jpg', 5, 1, 0, NULL),
    ('two sides to every murder', 'Solve the puzzles and uncover secrets in our gripping mysteries!', 19.99, 14.99, 3, 'images/two sides.jpg', 3, 1, 1, DATEADD(DAY, 5, GETDATE())),
    ('Fantasy Realms', 'Escape to magical worlds filled with dragons and wizards!', 12.99, 7.99, 4, '~/Content/howls.jpg', 8, 0, 0, NULL);

-- Insert data into BookFormats table
INSERT INTO [dbo].[BookFormats] (BookId, Format)
VALUES 
    (1, 'EPUB'),
    (1, 'PDF'),
    (1, 'MOBI'),
    (1, 'F2B'),
    (2, 'PDF'),
    (2, 'EPUB'),
    (2, 'MOBI'),
    (2, 'F2B'),
    (3, 'MOBI'),
    (3, 'PDF'),
    (3, 'MOBI'),
    (3, 'F2B'),
    (4, 'EPUB'),
    (4, 'PDF'),
    (4, 'MOBI'),
    (4, 'F2B');



-- Verify data
SELECT * FROM [dbo].[Categories];
SELECT * FROM [dbo].[Books];
SELECT * FROM [dbo].[BookFormats];

