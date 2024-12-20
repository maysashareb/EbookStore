UPDATE Books
SET AverageRating = 0
WHERE AverageRating IS NULL;

UPDATE Books
SET RatingCount = 0
WHERE RatingCount IS NULL;

ALTER TABLE Books
ALTER COLUMN AverageRating DECIMAL(18, 2) NOT NULL;
ALTER TABLE Books
ALTER COLUMN RatingCount INT NOT NULL;

