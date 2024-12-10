-- First, create a new column with the desired name
ALTER TABLE Books ADD AvailableCopiesBorrow INT NOT NULL DEFAULT 0;

-- Copy data from the old column to the new column
UPDATE Books SET AvailableCopiesBorrow = AvailableCopies;

-- Drop the old column
ALTER TABLE Books DROP COLUMN AvailableCopies;
