UPDATE Books
SET 
    EpubUrl = REPLACE(EpubUrl, '/Content/Formats/', '/NewPath/Formats/'),
    Fb2Url = REPLACE(Fb2Url, '/Content/Formats/', '/NewPath/Formats/'),
    MobiUrl = REPLACE(MobiUrl, '/Content/Formats/', '/NewPath/Formats/'),
    PdfUrl = REPLACE(PdfUrl, '/Content/Formats/', '/NewPath/Formats/');
