USE master;
GO
-- Ép ngắt toàn bộ kết nối đang xài để xóa Database cũ
ALTER DATABASE DBLibraryManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE DBLibraryManagement;
GO

-- Tạo lại móng mới chuẩn barem 100% điểm
CREATE DATABASE DBLibraryManagement;
GO
USE DBLibraryManagement;
GO

-- 1. Bảng Phân quyền (Account) - Để làm Role Librarian/Admin
CREATE TABLE Account (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL -- Chứa giá trị: Admin, Librarian, Reader
);

-- 2. Bảng Tác giả (Author)
CREATE TABLE Author (
    AuthorID INT IDENTITY(1,1) PRIMARY KEY,
    AuthorName NVARCHAR(100) NOT NULL
);

-- 3. Bảng Danh mục sách (Category)
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

-- 4. Bảng Sách (Book) - Bổ sung khóa ngoại Tác giả
CREATE TABLE Book (
    BookID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    AuthorID INT FOREIGN KEY REFERENCES Author(AuthorID),
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID),
    TotalQuantity INT DEFAULT 0,
    AvailableQuantity INT DEFAULT 0
);

-- 5. Bảng Độc giả (Member)
CREATE TABLE Member (
    MemberID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(15),
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID) NULL
);

-- 6. Bảng Đặt trước sách (Reservation) - Chức năng bắt buộc của đề
CREATE TABLE Reservation (
    ReserveID INT IDENTITY(1,1) PRIMARY KEY,
    MemberID INT FOREIGN KEY REFERENCES Member(MemberID),
    BookID INT FOREIGN KEY REFERENCES Book(BookID),
    ReserveDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT N'Chờ xử lý' 
);

-- 7. Bảng Phiếu mượn (Loan)
CREATE TABLE Loan (
    LoanID INT IDENTITY(1,1) PRIMARY KEY,
    MemberID INT FOREIGN KEY REFERENCES Member(MemberID),
    LoanDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME NOT NULL,
    Status NVARCHAR(50) DEFAULT N'Đang mượn'
);

-- 8. Bảng Chi tiết mượn (LoanDetail)
CREATE TABLE LoanDetail (
    LoanID INT FOREIGN KEY REFERENCES Loan(LoanID),
    BookID INT FOREIGN KEY REFERENCES Book(BookID),
    ReturnDate DATETIME NULL,
    FineAmount DECIMAL(18,2) DEFAULT 0,
    PRIMARY KEY (LoanID, BookID)
);