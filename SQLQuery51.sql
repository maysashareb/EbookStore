-- Add the Author column
ALTER TABLE Books
ADD Author NVARCHAR(MAX) NULL;

-- Add the Publisher column
ALTER TABLE Books
ADD Publisher NVARCHAR(MAX) NULL;


