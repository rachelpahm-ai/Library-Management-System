CREATE DATABASE DBLibraryManagement;
GO
USE DBLibraryManagement;
GO

-- 1. Bảng Danh mục sách (TRINH phụ trách)
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

-- 2. Bảng Sách (TRINH phụ trách)
CREATE TABLE Book (
    BookID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(100),
    TotalQuantity INT DEFAULT 0,
    AvailableQuantity INT DEFAULT 0, -- Rất quan trọng để Hưng/Tiên check kho
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID)
);

-- 3. Bảng Độc giả (Huy phụ trách)
CREATE TABLE Member (
    MemberID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(15)
);

-- 4. Bảng Phiếu mượn (Hưng & Tiên đụng vào)
CREATE TABLE Loan (
    LoanID INT IDENTITY(1,1) PRIMARY KEY,
    MemberID INT FOREIGN KEY REFERENCES Member(MemberID),
    LoanDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME NOT NULL,
    Status NVARCHAR(50) DEFAULT N'Đang mượn' -- Các trạng thái: Đang mượn, Đã hoàn tất, Quá hạn
);

-- 5. Bảng Chi tiết mượn (Hưng, Tiên, Bảo đụng vào)
CREATE TABLE LoanDetail (
    LoanID INT FOREIGN KEY REFERENCES Loan(LoanID),
    BookID INT FOREIGN KEY REFERENCES Book(BookID),
    ReturnDate DATETIME NULL, -- Null nghĩa là chưa trả
    FineAmount DECIMAL(18,2) DEFAULT 0, -- Tiền phạt, Bảo sẽ xử lý cột này
    PRIMARY KEY (LoanID, BookID)
);