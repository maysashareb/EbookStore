UPDATE Books
SET Description = LEFT(Description, 100)
WHERE LEN(Description) > 100;
