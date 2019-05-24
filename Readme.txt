TO CREATE DBCONTEXT
PM> Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=UserDatabase;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models

TO UPDATE DBCONTEXT
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=UserDatabase;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -f

CREATE DATABASE UserDatabase;
GO

USE UserDatabase;
GO

CREATE TABLE Users (
    Id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Username nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    Salt nvarchar(max) NOT NULL,
);
GO
