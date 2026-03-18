CREATE DATABASE BTL_API
GO

USE BTL_API
GO


/* =============================
TABLE: Customer
============================= */

CREATE TABLE Customer(
    CustomerID INT IDENTITY PRIMARY KEY,
    CustomerName NVARCHAR(100),
    CustomerGender NVARCHAR(10),
    CustomerPhone VARCHAR(15),
    CustomerEmail VARCHAR(100),
    CustomerAddress NVARCHAR(200)
)

INSERT INTO Customer VALUES
(N'Nguyễn Văn A',N'Nam','0901111111','a@gmail.com',N'Hà Nội'),
(N'Trần Thị B',N'Nữ','0902222222','b@gmail.com',N'Hồ Chí Minh'),
(N'Lê Văn C',N'Nam','0903333333','c@gmail.com',N'Đà Nẵng')


/* =============================
TABLE: Account
============================= */

CREATE TABLE Account(
    AccountID INT IDENTITY PRIMARY KEY,
    Username VARCHAR(50) UNIQUE,
    Password VARCHAR(50),
    AccountType INT,
    AccountDisplayName NVARCHAR(100),
    CustomerID INT NULL,
    FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID)
)

INSERT INTO Account VALUES
('admin','123',0,N'Admin',NULL),
('nhanvien','123',1,N'Nhân viên A',NULL),
('customer','123',2,N'Khách A',1)


/* =============================
TABLE: BookCategory
============================= */

CREATE TABLE BookCategory(
    BookCategoryID INT IDENTITY PRIMARY KEY,
    BookCategoryName NVARCHAR(100)
)

INSERT INTO BookCategory VALUES
(N'Tiểu thuyết'),
(N'Công nghệ'),
(N'Khoa học')


/* =============================
TABLE: Author
============================= */

CREATE TABLE Author(
    AuthorID INT IDENTITY PRIMARY KEY,
    AuthorName NVARCHAR(100)
)

INSERT INTO Author VALUES
(N'Nguyễn Nhật Ánh'),
(N'Robert C. Martin'),
(N'Stephen Hawking')


/* =============================
TABLE: Publisher
============================= */

CREATE TABLE Publisher(
    PublisherID INT IDENTITY PRIMARY KEY,
    PublisherName NVARCHAR(150),
    PublisherAddress NVARCHAR(200),
    PublisherPhone VARCHAR(15)
)

INSERT INTO Publisher VALUES
(N'NXB Trẻ',N'Hồ Chí Minh','0281111111'),
(N'NXB Kim Đồng',N'Hà Nội','0282222222'),
(N'NXB Giáo Dục',N'Hà Nội','0283333333')


/* =============================
TABLE: Book
============================= */

CREATE TABLE Book(
    BookID INT IDENTITY PRIMARY KEY,
    BookName NVARCHAR(200),
    BookDescription NVARCHAR(500),
    BookCategoryID INT,
    BookAuthorID INT,
    BookPublisherID INT,
    BookQuantity INT,
    BookPrice DECIMAL(10,2),
    BookStatus INT,
    FOREIGN KEY(BookCategoryID) REFERENCES BookCategory(BookCategoryID),
    FOREIGN KEY(BookAuthorID) REFERENCES Author(AuthorID),
    FOREIGN KEY(BookPublisherID) REFERENCES Publisher(PublisherID)
)

INSERT INTO Book VALUES
(N'Mắt Biếc',N'Tiểu thuyết nổi tiếng của Nguyễn Nhật Ánh',1,1,1,50,90000,1),
(N'Clean Code',N'Sách lập trình nổi tiếng về viết code sạch',2,2,2,30,250000,1),
(N'Lược Sử Thời Gian',N'Sách khoa học về vũ trụ của Stephen Hawking',3,3,3,20,200000,1)


/* =============================
TABLE: Orders
============================= */

CREATE TABLE Orders(
    OrderID INT IDENTITY PRIMARY KEY,
    CustomerID INT,
    OrderCreatedDate DATETIME,
    ReceiverName NVARCHAR(100),
    ReceiverPhone VARCHAR(15),
    ReceiverAddress NVARCHAR(200),
    OrderTotalAmount DECIMAL(12,2),
    OrderStatus INT,
    FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID)
)

INSERT INTO Orders VALUES
(1,GETDATE(),N'Nguyễn Văn A','0901111111',N'Hà Nội',180000,0),
(2,GETDATE(),N'Trần Thị B','0902222222',N'Hồ Chí Minh',250000,1),
(1,GETDATE(),N'Nguyễn Văn A','0901111111',N'Đà Nẵng',200000,2)


/* =============================
TABLE: OrderDetail
============================= */

CREATE TABLE OrderDetail(
    OrderDetailID INT IDENTITY PRIMARY KEY,
    OrderID INT,
    BookID INT,
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    FOREIGN KEY(OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY(BookID) REFERENCES Book(BookID)
)

INSERT INTO OrderDetail (OrderID,BookID,Quantity,UnitPrice) VALUES
(1,1,2,90000),
(2,2,1,250000),
(3,3,1,200000)


ALTER TABLE Book
ADD BookImage VARCHAR(255)
UPDATE Book
SET BookImage = 'mat_biec.png'
WHERE BookID = 1

UPDATE Book
SET BookImage = 'clean_code.png'
WHERE BookID = 2

UPDATE Book
SET BookImage = 'luoc_su_thoi_gian.png'
WHERE BookID = 3