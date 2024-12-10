ALTER TABLE Books
ADD EpubUrl NVARCHAR(MAX),
    Fb2Url NVARCHAR(MAX), 
    MobiUrl NVARCHAR(MAX),
    PdfUrl NVARCHAR(MAX),
    Publisheyear NVARCHAR(255), 

    AvailableCopies INT NOT NULL DEFAULT 0;