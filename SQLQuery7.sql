-- Update Author, Publisher, and DiscountPrice for 4 specific books
UPDATE Books
SET Author = 'J.K. Rowling', Publisher = 'Bloomsbury', DiscountPrice = 5.43
WHERE Id = 1; -- Replace with the actual ID of the book

UPDATE Books
SET Author = 'George R.R. Martin', Publisher = 'Bantam Books', DiscountPrice = 5.43
WHERE Id = 2; -- Replace with the actual ID of the book

UPDATE Books
SET Author = 'Agatha Christie', Publisher = 'HarperCollins', DiscountPrice = 5.43
WHERE Id = 3; -- Replace with the actual ID of the book

UPDATE Books
SET Author = 'Dan Brown', Publisher = 'Doubleday', DiscountPrice = 5.43
WHERE Id = 4; -- Replace with the actual ID of the book
