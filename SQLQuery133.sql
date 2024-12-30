CREATE TABLE Ratings (
    Id INT PRIMARY KEY IDENTITY,
    BookId INT FOREIGN KEY REFERENCES Books(Id),
    Value INT CHECK (Value >= 1 AND Value <= 5)
);
