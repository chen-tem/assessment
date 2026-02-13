-- Database schema for Information table
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_NAME = 'Information'
)
BEGIN
    CREATE TABLE Information (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Date DATETIME NOT NULL,
        UserId INT NOT NULL,
        Name NVARCHAR(100),
        Email NVARCHAR(100)
    );
END
