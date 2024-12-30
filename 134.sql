-- Add the Price column to the CartItems table
ALTER TABLE CartItems
ADD Price DECIMAL(18, 2) NOT NULL DEFAULT 0.00;
