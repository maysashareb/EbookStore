UPDATE Books
SET CreatedDate = GETDATE()
WHERE CreatedDate IS NULL;
