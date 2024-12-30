UPDATE Books
SET 
    EpubUrl = REPLACE(EpubUrl, '/NewPath/Formats/', '/Content/Formats/'),
    Fb2Url = REPLACE(Fb2Url, '/NewPath/Formats/', '/Content/Formats/'),
    MobiUrl = REPLACE(MobiUrl, '/NewPath/Formats/', '/Content/Formats/'),
    PdfUrl = REPLACE(PdfUrl, '/NewPath/Formats/', '/Content/Formats/');
