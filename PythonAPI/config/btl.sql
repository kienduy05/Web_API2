CREATE DATABASE BTL_API;
GO

USE BTL_API;
GO

CREATE TABLE Customer (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(100) NULL,
    CustomerGender NVARCHAR(10) NULL,
    CustomerPhone VARCHAR(15) NULL,
    CustomerEmail VARCHAR(100) NULL,
    CustomerAddress NVARCHAR(200) NULL
);
GO

CREATE TABLE Author (
    AuthorID INT IDENTITY(1,1) PRIMARY KEY,
    AuthorName NVARCHAR(100) NULL
);
GO

CREATE TABLE BookCategory (
    BookCategoryID INT IDENTITY(1,1) PRIMARY KEY,
    BookCategoryName NVARCHAR(100) NULL
);
GO

CREATE TABLE Publisher (
    PublisherID INT IDENTITY(1,1) PRIMARY KEY,
    PublisherName NVARCHAR(150) NULL,
    PublisherAddress NVARCHAR(200) NULL,
    PublisherPhone VARCHAR(15) NULL
);
GO

CREATE TABLE Book (
    BookID INT IDENTITY(1,1) PRIMARY KEY,
    BookName NVARCHAR(200) NULL,
    BookDescription NVARCHAR(500) NULL,
    BookCategoryID INT NULL,
    BookAuthorID INT NULL,
    BookPublisherID INT NULL,
    BookQuantity INT NULL,
    BookPrice DECIMAL(10,2) NULL,
    BookStatus INT NULL,
    BookImage VARCHAR(255) NULL
);
GO

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NULL,
    OrderCreatedDate DATETIME NULL,
    ReceiverName NVARCHAR(100) NULL,
    ReceiverPhone VARCHAR(15) NULL,
    ReceiverAddress NVARCHAR(200) NULL,
    OrderTotalAmount DECIMAL(12,2) NULL,
    OrderStatus INT NULL
);
GO

CREATE TABLE OrderDetail (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NULL,
    BookID INT NULL,
    Quantity INT NULL,
    UnitPrice DECIMAL(10,2) NULL
);
GO

CREATE TABLE Account (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NULL UNIQUE,
    [Password] VARCHAR(50) NULL,
    AccountType INT NULL,
    AccountDisplayName NVARCHAR(100) NULL,
    CustomerID INT NULL
);
GO

ALTER TABLE Account
ADD CONSTRAINT FK_Account_Customer
FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID);
GO

ALTER TABLE Book
ADD CONSTRAINT FK_Book_Author
FOREIGN KEY (BookAuthorID) REFERENCES Author(AuthorID);
GO

ALTER TABLE Book
ADD CONSTRAINT FK_Book_Category
FOREIGN KEY (BookCategoryID) REFERENCES BookCategory(BookCategoryID);
GO

ALTER TABLE Book
ADD CONSTRAINT FK_Book_Publisher
FOREIGN KEY (BookPublisherID) REFERENCES Publisher(PublisherID);
GO

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Customer
FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID);
GO

ALTER TABLE OrderDetail
ADD CONSTRAINT FK_OrderDetail_Order
FOREIGN KEY (OrderID) REFERENCES Orders(OrderID);
GO

ALTER TABLE OrderDetail
ADD CONSTRAINT FK_OrderDetail_Book
FOREIGN KEY (BookID) REFERENCES Book(BookID);
GO

-- Customer
INSERT INTO Customer (CustomerName, CustomerGender, CustomerPhone, CustomerEmail, CustomerAddress) VALUES
(N'Nguyễn Văn A', N'Nam', '09011111112', 'nguyenvana@gmail.com', N'Hà Nội'),
(N'Trần Thị B', N'Nữ', '0902222222', 'b@gmail.com', N'Hồ Chí Minh'),
(N'Lê Văn C', N'Nam', '0903333333', 'c@gmail.com', N'Đà Nẵng'),
(N'Nguyễn Thị B', NULL, '0912345671', 'nguyenthib@gmail.com', N'Hà Nội');
GO

-- Author
INSERT INTO Author (AuthorName) VALUES
(N'Nguyễn Nhật Ánh'),
(N'Robert C. Martin'),
(N'Stephen Hawking'),
(N'Nguyên Hồng');
GO

-- BookCategory
INSERT INTO BookCategory (BookCategoryName) VALUES
(N'Tiểu thuyết'),
(N'Công nghệ'),
(N'Khoa học');
GO

-- Publisher
INSERT INTO Publisher (PublisherName, PublisherAddress, PublisherPhone) VALUES
(N'NXB Trẻ', N'Hồ Chí Minh', '0281111111'),
(N'NXB Kim Đồng', N'Hà Nội', '0282222222'),
(N'NXB Giáo Dục', N'Hà Nội', '0283333333');
GO

-- Book
-- ID mới sẽ là:
-- 1=Mắt Biếc
-- 2=Clean Code
-- 3=Lược Sử Thời Gian
-- 4=Hạ Đỏ
-- 5=Ngồi Khóc Trên Cây
-- 6=Chú bé rắc rối
-- 7=Bỉ Vỏ
-- 8=Những ngày thơ ấu
-- 9=Tôi Là Bêtô
-- 10=Còn Chút Gì Để Nhớ
INSERT INTO Book (BookName, BookDescription, BookCategoryID, BookAuthorID, BookPublisherID, BookQuantity, BookPrice, BookStatus, BookImage) VALUES
(N'Mắt Biếc', N'Mắt Biếc” là bản tình ca buồn của tuổi trẻ, nơi những rung động đầu đời hiện lên trong trẻo mà day dứt. Tình yêu của Ngạn dành cho Hà Lan lặng lẽ, chân thành nhưng mãi không trọn vẹn, như chính những cảm xúc chưa kịp thổ lộ. Câu chuyện không chỉ gợi lại một thời thanh xuân đã qua, mà còn chạm đến nỗi tiếc nuối rất thật – khi con người nhận ra có những điều đẹp đẽ nhất lại không thể giữ lại bên mình mãi mãi.', 1, 1, 1, 50, 90000.00, 1, 'matbiec.jpg'),
(N'Clean Code', N'“Clean Code” của Robert C. Martin là cuốn sách kinh điển dành cho lập trình viên, tập trung vào việc viết mã nguồn rõ ràng, dễ đọc và dễ bảo trì. Tác phẩm không chỉ đưa ra các nguyên tắc và quy tắc thực tiễn, mà còn phân tích những ví dụ cụ thể để giúp người đọc hiểu thế nào là “code sạch”. Qua đó, lập trình viên có thể nâng cao tư duy thiết kế, giảm lỗi và tạo ra những sản phẩm phần mềm chất lượng, bền vững theo thời gian.', 2, 2, 2, 29, 250000.00, 1, 'cleancode.png'),
(N'Lược Sử Thời Gian', N'“Lược Sử Thời Gian” của Stephen Hawking là một trong những tác phẩm khoa học phổ thông nổi tiếng, mở ra cánh cửa giúp người đọc tiếp cận những bí ẩn lớn nhất của vũ trụ. Cuốn sách giải thích các khái niệm phức tạp như hố đen, Big Bang, thời gian và không gian theo cách dễ hiểu nhưng vẫn đầy chiều sâu. Không chỉ là hành trình khám phá khoa học, tác phẩm còn khơi gợi sự tò mò và niềm kinh ngạc trước quy luật vận hành của vũ trụ, khiến người đọc suy ngẫm về nguồn gốc và tương lai của chính mình.', 3, 3, 3, 20, 200000.00, 1, 'luocsuthoigian.webp'),
(N'Hạ Đỏ (Tái bản 2022)', N'“Hạ Đỏ” là bức tranh dịu dàng về tuổi trẻ, nơi những ngày hè trôi qua chậm rãi giữa tiếng ve và ánh nắng vàng ươm. Ở đó, tình bạn hồn nhiên, những rung động đầu đời và cả những ước mơ nhỏ bé đan xen, tạo nên một miền ký ức khó quên. Câu chuyện không chỉ gợi lại cảm giác trong trẻo của một thời đã qua, mà còn chạm đến nỗi buồn rất nhẹ khi con người dần lớn lên, buộc phải rời xa những điều quen thuộc. “Hạ Đỏ” vì thế mang một vẻ đẹp lặng lẽ, khiến người đọc vừa mỉm cười, vừa tiếc nuối.', 1, 1, 1, 50, 80500.00, 1, 'hado.webp'),
(N'Ngồi Khóc Trên Cây (Tái Bản 2022)', N'“Ngồi Khóc Trên Cây” (Tái Bản 2022) là câu chuyện nhẹ nhàng nhưng đầy cảm xúc về tuổi thơ và những rung động đầu đời trong sáng. Qua hành trình của Đông và Rùa, tác phẩm tái hiện một miền quê yên bình, nơi tình bạn, sự ngây thơ và những bí mật nhỏ bé dần hé lộ. Không chỉ là ký ức đẹp, cuốn sách còn chạm đến những tổn thương và mất mát đầu tiên, khiến người đọc lặng đi trong cảm xúc vừa dịu dàng, vừa man mác buồn của một thời đã xa.', 1, 1, 1, 99, 130000.00, 1, 'ngoikhoctrencay.webp'),
(N'Chú bé rắc rối (Tái bán 2022)', N'“Chú bé rắc rối” là câu chuyện nhẹ nhàng nhưng sâu sắc về một cậu bé tinh nghịch, luôn khiến người lớn đau đầu vì những trò quậy phá không ngừng. Tuy nhiên, ẩn sau vẻ ngoài bướng bỉnh ấy là một trái tim nhạy cảm, khao khát được thấu hiểu và yêu thương. Qua từng tình huống dở khóc dở cười, cuốn sách gửi gắm thông điệp ý nghĩa về gia đình, sự kiên nhẫn và cách người lớn nên lắng nghe trẻ nhỏ. Đây là tác phẩm phù hợp cho cả thiếu nhi lẫn phụ huynh.', 1, 1, 1, 200, 32000.00, 1, 'chuberacroi.webp'),
(N'Bỉ Vỏ', N'“Bỉ vỏ” là tiểu thuyết hiện thực đặc sắc của Nguyên Hồng, khắc họa cuộc đời đầy bi kịch của Tám Bính – một cô gái nghèo bị đẩy vào con đường lưu lạc, sa ngã giữa xã hội đầy bất công. Từ một người phụ nữ hiền lành, cô dần trở thành kẻ sống ngoài vòng pháp luật, mang trong mình nỗi đau, sự giằng xé và khát khao được làm lại cuộc đời. Tác phẩm phản ánh sâu sắc số phận con người dưới đáy xã hội, đồng thời lên án những định kiến tàn nhẫn và bất công đã chà đạp lên nhân phẩm con người.', 1, 4, 2, 100, 45000.00, 1, 'bivo.webp'),
(N'Những ngày thơ ấu', N'“Những ngày thơ ấu” là tác phẩm tự truyện tiêu biểu của Nguyên Hồng, tái hiện chân thực tuổi thơ nhiều đau khổ của chính tác giả. Qua góc nhìn của cậu bé Hồng, người đọc cảm nhận rõ nỗi cô đơn, tủi cực khi thiếu vắng tình mẹ và phải sống trong sự ghẻ lạnh của họ hàng. Tuy vậy, tình yêu thương mẹ sâu sắc vẫn luôn là điểm tựa tinh thần giúp cậu vượt qua mọi bất hạnh. t.', 1, 4, 1, 100, 50000.00, 1, 'nhungngaythoau.webp'),
(N'Tôi Là Bêtô (Tái Bản 2023)', N'Truyện Tôi là Bêtô là sáng tác mới nhất của nhà văn Nguyễn Nhật Ánh được viết theo phong cách hoàn toàn khác so với những tác phẩm trước đây của ông. Những mẩu chuyện, hay những phát hiện của chú chó Bêtô đầy thú vị, vừa hài hước, vừa chiêm nghiệm một cách nhẹ nhàng “vô vàn những điều thú vị mà cuộc sống cố tình giấu kín ở ngóc ngách nào đó trong tâm hồn của mỗi chúng ta”.', 1, 1, 1, 200, 90000.00, 1, 'toilabeto.jpg'),
(N'Còn Chút Gì Để Nhớ (Tái Bản 2022)', N'Đó là những kỷ niệm thời đi học của Chương, lúc mới bước chân vào Sài Gòn và làm quen với cuộc sống đô thị.
Cuộc sống đầy biến động đã xô dạt mỗi người mỗi nơi, nhưng trải qua hàng mấy chục năm, những kỷ niệm ấy vẫn luôn níu kéo Chương về với một thời để nhớ.', 1, 1, 1, 50, 36500.00, 1, 'conchutgidenho.webp');
GO

-- Orders
-- CustomerID cũ 11 được đổi thành 4
INSERT INTO Orders (CustomerID, OrderCreatedDate, ReceiverName, ReceiverPhone, ReceiverAddress, OrderTotalAmount, OrderStatus) VALUES
(1, '2026-03-17T09:27:58.383', N'Nguyễn Văn A', '0901111111', N'Hà Nội', 180000.00, 2),
(2, '2026-03-17T09:27:58.383', N'Trần Thị B', '0902222222', N'Hồ Chí Minh', 250000.00, 1),
(1, '2026-03-17T09:27:58.383', N'Nguyễn Văn A', '0901111111', N'Đà Nẵng', 200000.00, 2),
(1, '2026-03-26T14:52:23.000', N'duy a', '0902255123', N'XuanDinh', 4640000.00, 2),
(1, '2026-03-26T18:59:09.000', N'Nguyen Duy Kien', '0961418033', N'Xuan Dinh', 430000.00, 2);
GO

-- OrderDetail
-- Map BookID cũ -> mới:
-- 1->1, 2->2, 3->3, 5->5
INSERT INTO OrderDetail (OrderID, BookID, Quantity, UnitPrice) VALUES
(1, 1, 2, 90000.00),
(2, 2, 1, 250000.00),
(3, 3, 1, 200000.00),
(4, 1, 51, 90000.00),
(5, 5, 1, 130000.00),
(5, 2, 1, 250000.00);
GO

-- Account
-- CustomerID cũ 11 đổi thành 4
INSERT INTO Account (Username, [Password], AccountType, AccountDisplayName, CustomerID) VALUES
('admin', '123', 0, N'Admin', NULL),
('nhanvien', '123', 1, N'Nhân viên A', NULL),
('customer', '123456', 2, N'Khách A', 1),
('customer2', '123456', 2, N'Nguyễn Thị B', 4);
GO