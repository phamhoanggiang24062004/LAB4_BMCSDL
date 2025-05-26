-- LAB4 - BMCSDL

USE MASTER
GO
-- Xóa nếu đã tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QLSVNhom')
BEGIN
    DROP DATABASE QLSVNhom;
END
GO

-- Tạo mới
CREATE DATABASE QLSVNhom;
GO

-- Sử dụng CSDL vừa tạo
USE QLSVNhom;
GO

-- 3. XÓA BẢNG (nếu tồn tại) — THEO THỨ TỰ PHỤ THUỘC
IF OBJECT_ID('BANGDIEM', 'U') IS NOT NULL DROP TABLE BANGDIEM;
IF OBJECT_ID('SINHVIEN', 'U') IS NOT NULL DROP TABLE SINHVIEN;
IF OBJECT_ID('HOCPHAN', 'U') IS NOT NULL DROP TABLE HOCPHAN;
IF OBJECT_ID('LOP', 'U') IS NOT NULL DROP TABLE LOP;
IF OBJECT_ID('NHANVIEN', 'U') IS NOT NULL DROP TABLE NHANVIEN;
GO

CREATE TABLE NHANVIEN (
    MANV VARCHAR(20) PRIMARY KEY,                     -- Mã nhân viên (PK)
    HOTEN NVARCHAR(100) NOT NULL,                     -- Họ tên
    EMAIL VARCHAR(20) NULL,                          -- Email
    LUONG VARBINARY(MAX) NULL,                        -- Lương (RSA mã hóa)
    TENDN NVARCHAR(100) NOT NULL UNIQUE,              -- Tên đăng nhập (duy nhất)
    MATKHAU VARBINARY(MAX) NOT NULL,                  -- Mật khẩu (SHA1)
    PUBKEY NVARCHAR(MAX) NULL                          -- Tên khóa công khai RSA
);

CREATE TABLE LOP (
    MALOP VARCHAR(20) PRIMARY KEY,                    -- Mã lớp (PK)
    TENLOP NVARCHAR(100) NOT NULL,                    -- Tên lớp
    MANV VARCHAR(20) NULL,                            -- Mã giáo viên chủ nhiệm
    FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV)      -- Ràng buộc đến nhân viên
);

CREATE TABLE SINHVIEN (
    MASV VARCHAR(20) PRIMARY KEY,                    -- Mã sinh viên (PK)
    HOTEN NVARCHAR(100) NOT NULL,                     -- Họ tên
    NGAYSINH DATETIME NULL,                           -- Ngày sinh
    DIACHI NVARCHAR(200) NULL,                        -- Địa chỉ
    MALOP VARCHAR(20) NULL,                         -- Mã lớp (tham chiếu)
    TENDN NVARCHAR(100) NOT NULL UNIQUE,              -- Tên đăng nhập (duy nhất)
    MATKHAU VARBINARY(MAX) NOT NULL                   -- Mật khẩu (SHA1 - đã băm)
	FOREIGN KEY (MALOP) REFERENCES LOP(MALOP)      -- Ràng buộc đến lớp
);

CREATE TABLE HOCPHAN (
    MAHP VARCHAR(20) PRIMARY KEY,                     -- Mã học phần (PK)
    TENHP NVARCHAR(100) NOT NULL,                     -- Tên học phần
    SOTC INT NULL                                     -- Số tín chỉ
);

CREATE TABLE BANGDIEM (
    MASV VARCHAR(20),                                 -- Mã sinh viên
    MAHP VARCHAR(20),                                 -- Mã học phần
    DIEMTHI VARBINARY(MAX),                           -- Điểm thi (RSA mã hóa)
    PRIMARY KEY (MASV, MAHP),                         -- Khóa chính kép
    FOREIGN KEY (MASV) REFERENCES SINHVIEN(MASV),     -- Tham chiếu sinh viên
    FOREIGN KEY (MAHP) REFERENCES HOCPHAN(MAHP)       -- Tham chiếu học phần
);

USE QLSVNhom
GO

-- Kiểm tra và xóa stored procedure cu nếu tồn tại
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_INS_PUBLIC_NHANVIEN')
    DROP PROCEDURE SP_INS_PUBLIC_NHANVIEN;
GO

-- i. Thêm mới dữ liệu vào bảng NHANVIEN(LUONG, MATKHAU đã được mã hoá ở ứng dụng)
CREATE PROCEDURE SP_INS_PUBLIC_NHANVIEN
    @MANV     VARCHAR(20),
    @HOTEN    NVARCHAR(100),
    @EMAIL    VARCHAR(20),
    @LUONG	  VARBINARY(MAX),            
    @TENDN    NVARCHAR(100),
    @MK       VARBINARY(MAX),
	@PUB	  NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO NHANVIEN (MANV, HOTEN, EMAIL, LUONG, TENDN, MATKHAU, PUBKEY)
    VALUES (@MANV, @HOTEN, @EMAIL, @LUONG, @TENDN, @MK, @PUB);
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_SEL_PUBLIC_NHANVIEN')
    DROP PROCEDURE SP_SEL_PUBLIC_NHANVIEN;
GO

-- ii. Truy vấn dữ liệu nhân viên(LUONG ở dạng mã hoá)
CREATE PROCEDURE SP_SEL_PUBLIC_NHANVIEN
    @TENDN VARCHAR(20),
    @MK NVARCHAR(20)    
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @MANV VARCHAR(20);
	SELECT @MANV = MANV 
	FROM NHANVIEN
	WHERE TENDN = @TENDN;

    SELECT 
        MANV,
        HOTEN,
        EMAIL,
        LUONG
    FROM 
        NHANVIEN
    WHERE 
        MANV = @TENDN;

END
GO

-- Chèn dữ liệu mẫu vào các bảng:
-- Chèn nhân viên mẫu
/*
EXEC SP_INS_PUBLIC_NHANVIEN 'NV01', N'Nguyễn Văn A', 'a@fit.hcmus.edu.vn',3000000, 'nv01', 'pass123';
EXEC SP_INS_PUBLIC_NHANVIEN 'NV02', N'Trần Thị B', 'b@fit.hcmus.edu.vn', 2800000, 'nv02', 'abc123';
EXEC SP_INS_PUBLIC_NHANVIEN 'NV03', N'Lê Văn C', 'c@fit.hcmus.edu.vn', 3100000, 'nv03', 'xyz123';
EXEC SP_INS_PUBLIC_NHANVIEN 'NV04', N'Phạm Thị D', 'd@fit.hcmus.edu.vn', 3200000, 'nv04','admin123';
EXEC SP_INS_PUBLIC_NHANVIEN 'NV05', N'Hoàng Văn E', 'e@fit.hcmus.edu.vn', 2900000, 'nv05', 'test123';
*/

/*
-- Chèn lớp học mẫu
INSERT INTO LOP (MALOP, TENLOP, MANV)
VALUES 
('L01', N'Cơ sở dữ liệu', 'NV01'),
('L02', N'Mạng máy tính', 'NV02'),
('L03', N'Cấu trúc dữ liệu', 'NV01'),
('L04', N'Lập trình hướng đối tượng', 'NV03'),
('L05', N'An toàn hệ thống', 'NV04'),
('L06', N'Điện toán đám mây', 'NV02'),
('L07', N'Cơ sở AI', 'NV04'),
('L08', N'An toàn máy tính', 'NV05'),
('L09', N'Nhập môn lập trình', 'NV01'),
('L10', N'Cơ sở lập trình', 'NV02');

-- Chèn sinh viên mẫu
INSERT INTO SINHVIEN (MASV, HOTEN, NGAYSINH, DIACHI, MALOP, TENDN, MATKHAU)
VALUES 
('SV01', N'Nguyễn Minh Khoa', '2002-03-01', N'123 Lê Lợi, Q1', 'L01', 'sv01', HASHBYTES('SHA1', 'svpass1')),
('SV02', N'Trần Hoàng Anh', '2001-07-15', N'22 Trần Hưng Đạo, Q5', 'L01', 'sv02', HASHBYTES('SHA1', 'svpass2')),
('SV03', N'Lê Văn Tùng', '2002-12-20', N'88 Nguyễn Trãi, Q1', 'L02', 'sv03', HASHBYTES('SHA1', 'svpass3')),
('SV04', N'Phạm Thị Mai', '2001-04-30', N'10 Bạch Đằng, Q3', 'L03', 'sv04', HASHBYTES('SHA1', 'svpass4')),
('SV05', N'Hoàng Gia Huy', '2000-09-09', N'90 Điện Biên Phủ, Q10', 'L04', 'sv05', HASHBYTES('SHA1', 'svpass5')),
('SV06', N'Nguyễn Thị Hồng', '2002-05-12', N'45 Lý Thường Kiệt, Q10', 'L02', 'sv06', HASHBYTES('SHA1', 'svpass6')),
('SV07', N'Trần Văn Đạt', '2001-11-25', N'78 Cách Mạng Tháng 8, Q3', 'L03', 'sv07', HASHBYTES('SHA1', 'svpass7')),
('SV08', N'Lê Thị Minh Anh', '2002-08-18', N'12 Nguyễn Du, Q1', 'L05', 'sv08', HASHBYTES('SHA1', 'svpass8')),
('SV09', N'Phạm Quốc Bảo', '2001-02-14', N'34 Lê Văn Sỹ, Q3', 'L01', 'sv09', HASHBYTES('SHA1', 'svpass9')),
('SV10', N'Hoàng Thị Lan', '2000-06-30', N'56 Trần Quang Khải, Q5', 'L07', 'sv10', HASHBYTES('SHA1', 'svpass10')),
('SV11', N'Vũ Đình Khôi', '2002-09-05', N'89 Lê Quang Định, Q.Bình Thạnh', 'L05', 'sv11', HASHBYTES('SHA1', 'svpass11')),
('SV12', N'Đặng Thị Ngọc', '2001-04-22', N'21 Hồ Tùng Mậu, Q1', 'L08', 'sv12', HASHBYTES('SHA1', 'svpass12')),
('SV13', N'Bùi Văn Thành', '2000-12-15', N'43 Nguyễn Thị Minh Khai, Q3', 'L09', 'sv13', HASHBYTES('SHA1', 'svpass13')),
('SV14', N'Ngô Thị Hương', '2002-07-08', N'67 Đồng Khởi, Q1', 'L10', 'sv14', HASHBYTES('SHA1', 'svpass14')),
('SV15', N'Đỗ Minh Quân', '2001-01-19', N'99 Nam Kỳ Khởi Nghĩa, Q3', 'L08', 'sv15', HASHBYTES('SHA1', 'svpass15')),
('SV16', N'Nguyễn Thị Thu Hà', '2002-04-05', N'123 Trần Phú, Q5', 'L04', 'sv16', HASHBYTES('SHA1', 'svpass16')),
('SV17', N'Trần Minh Khang', '2001-08-12', N'45 Nguyễn Văn Cừ, Q1', 'L06', 'sv17', HASHBYTES('SHA1', 'svpass17')),
('SV18', N'Lê Thị Bích Ngọc', '2000-11-25', N'78 Lê Đại Hành, Q11', 'L07', 'sv18', HASHBYTES('SHA1', 'svpass18')),
('SV19', N'Phạm Văn Hải', '2002-02-18', N'12 Cộng Hòa, Q.Tân Bình', 'L06', 'sv19', HASHBYTES('SHA1', 'svpass19')),
('SV20', N'Hoàng Thị Thanh Thảo', '2001-06-30', N'34 Lý Tự Trọng, Q1', 'L10', 'sv20', HASHBYTES('SHA1', 'svpass20')),
('SV21', N'Vũ Minh Tuấn', '2000-09-15', N'56 Phạm Ngũ Lão, Q1', 'L09', 'sv21', HASHBYTES('SHA1', 'svpass21')),
('SV22', N'Đặng Thị Kim Chi', '2002-03-22', N'89 Lê Thánh Tôn, Q1', 'L08', 'sv22', HASHBYTES('SHA1', 'svpass22')),
('SV23', N'Bùi Văn Tài', '2001-07-08', N'21 Nguyễn Cư Trinh, Q1', 'L03', 'sv23', HASHBYTES('SHA1', 'svpass23')),
('SV24', N'Ngô Thị Mai Linh', '2000-12-19', N'43 Đinh Tiên Hoàng, Q.Bình Thạnh', 'L10', 'sv24', HASHBYTES('SHA1', 'svpass24')),
('SV25', N'Đỗ Văn Phúc', '2002-05-10', N'67 Võ Văn Tần, Q3', 'L07', 'sv25', HASHBYTES('SHA1', 'svpass25')),
('SV26', N'Nguyễn Hữu Anh', '2001-10-28', N'99 Trương Định, Q3', 'L05', 'sv26', HASHBYTES('SHA1', 'svpass26')),
('SV27', N'Trần Thị Ngọc Anh', '2000-04-15', N'11 Bà Huyện Thanh Quan, Q3', 'L02', 'sv27', HASHBYTES('SHA1', 'svpass27')),
('SV28', N'Lê Văn Thắng', '2002-08-20', N'33 Điện Biên Phủ, Q.Bình Thạnh', 'L06', 'sv28', HASHBYTES('SHA1', 'svpass28')),
('SV29', N'Phạm Thị Hồng Nhung', '2001-01-05', N'55 Nguyễn Đình Chiểu, Q3', 'L05', 'sv29', HASHBYTES('SHA1', 'svpass29')),
('SV30', N'Hoàng Văn Dũng', '2000-07-12', N'77 Lê Quý Đôn, Q3', 'L10', 'sv30', HASHBYTES('SHA1', 'svpass30'));

-- Chèn học phần mẫu
INSERT INTO HOCPHAN (MAHP, TENHP, SOTC)
VALUES 
('HP01', N'Cơ sở dữ liệu', 3),
('HP02', N'Mạng máy tính', 3),
('HP03', N'Cấu trúc dữ liệu', 4),
('HP04', N'Lập trình hướng đối tượng', 3),
('HP05', N'An toàn hệ thống', 2),
('HP06', N'Điện toán đám mây', 3),
('HP07', N'Cơ sở AI', 3),
('HP08', N'Nhập môn lập trình', 4),
('HP09', N'Cơ sở lập trình', 3),
('HP10', N'An toàn máy tính', 2);
*/


-- Stored procedure thêm dữ liệu lớp học
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_INS_LOPHOC')
    DROP PROCEDURE SP_INS_LOPHOC;
GO

CREATE PROCEDURE SP_INS_LOPHOC
    @MALOP VARCHAR(20),
    @TENLOP NVARCHAR(100),
    @MANV VARCHAR(20)
AS
BEGIN
    INSERT INTO LOP(MALOP, TENLOP, MANV)
    VALUES (@MALOP, @TENLOP, @MANV)
END
GO


-- Stored procedure thêm dữ liệu sinh viên
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_INS_SINHVIEN')
    DROP PROCEDURE SP_INS_SINHVIEN;
GO

CREATE PROCEDURE SP_INS_SINHVIEN
    @MASV VARCHAR(20),
    @HOTEN NVARCHAR(100),
    @NGAYSINH DATETIME,
    @DIACHI NVARCHAR(200),
    @MALOP VARCHAR(20),
    @TENDN NVARCHAR(100),
    @MATKHAU VARBINARY(MAX)
AS
BEGIN
    INSERT INTO SINHVIEN (MASV, HOTEN, NGAYSINH, DIACHI, MALOP, TENDN, MATKHAU)
    VALUES (@MASV, @HOTEN, @NGAYSINH, @DIACHI, @MALOP, @TENDN, @MATKHAU)
END
GO



-- Stored procedure thêm dữ liệu học phần
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_INS_HOCPHAN')
    DROP PROCEDURE SP_INS_HOCPHAN;
GO

CREATE PROCEDURE SP_INS_HOCPHAN
    @MAHP VARCHAR(20),
    @TENHP NVARCHAR(100),
    @SOTC INT
AS
BEGIN
    INSERT INTO HOCPHAN (MAHP, TENHP, SOTC)
    VALUES (@MAHP, @TENHP, @SOTC)
END
GO


-- Stored procedure kiểm tra thông tin đăng nhập
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_KIEMTRA_DANGNHAP')
    DROP PROCEDURE SP_KIEMTRA_DANGNHAP;
GO

CREATE PROCEDURE SP_KIEMTRA_DANGNHAP
    @USERNAME VARCHAR(20),
    @PASSWORD VARBINARY(MAX),
    @RETURN_MANV VARCHAR(20) OUTPUT -- thêm tham số output mới
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @TMP_MANV VARCHAR(20);

    
    SELECT @TMP_MANV = MANV
    FROM NHANVIEN
    WHERE TENDN = @USERNAME
        AND MATKHAU = @PASSWORD
	IF @TMP_MANV IS NOT NULL
		BEGIN
			SET @RETURN_MANV = @TMP_MANV; -- Trả về mã nhân viên
		END
		ELSE
		BEGIN
			SET @RETURN_MANV = NULL;    -- Không có mã nhân viên
		END
END
GO

-- Stored procedure quản lý tất cả lớp học
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_QUANLY_LOPHOC')
    DROP PROCEDURE SP_QUANLY_LOPHOC;
GO

CREATE PROCEDURE SP_QUANLY_LOPHOC
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        MALOP,
        TENLOP,
        MANV
    FROM 
        LOP;
END
GO


-- Stored procedure lấy về các lớp mà nhân viên quản lý
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_QUANLY_LOPHOC_NHANVIEN')
    DROP PROCEDURE SP_QUANLY_LOPHOC_NHANVIEN;
GO

CREATE PROCEDURE SP_QUANLY_LOPHOC_NHANVIEN
	@MANV VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        MALOP,
        TENLOP
    FROM 
        LOP L
	INNER JOIN 
        NHANVIEN NV ON L.MANV = NV.MANV
	WHERE NV.MANV = @MANV;
END
GO


-- Stored procedure quản lý sinh viên của từng lớp mà nhân viên quản lý
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_QUANLY_SINHVIEN')
    DROP PROCEDURE SP_QUANLY_SINHVIEN;
GO

CREATE PROCEDURE SP_QUANLY_SINHVIEN
	@MALOP VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        MASV, 
		HOTEN, 
		NGAYSINH, 
		DIACHI, 
		TENDN, 
		MATKHAU
    FROM 
        SINHVIEN
	WHERE MALOP = @MALOP;
END
GO

-- Stored procedure hiển thị thông tin sinh viên và điểm thi(nếu có, dạng mã hoá) của sinh viên
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_HIENTHI_DIEMTHI_SINHVIEN')
    DROP PROCEDURE SP_HIENTHI_DIEMTHI_SINHVIEN;
GO

CREATE PROCEDURE SP_HIENTHI_DIEMTHI_SINHVIEN
    @MALOP VARCHAR(20),
  
	@MAHP VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Truy vấn điểm thi
    SELECT 
        SV.MASV, 
        SV.HOTEN,
		BD.DIEMTHI
    FROM
        SINHVIEN SV
	LEFT JOIN 
		BANGDIEM BD ON BD.MASV = SV.MASV
    WHERE 
        SV.MALOP = @MALOP;

END
GO

-- Stored procedure nhập điểm của từng sinh viên trong các lớp mà nhân viên quản lý(điểm đã được mã hoá ở ứng dụng)
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_NHAPDIEM_SINHVIEN')
    DROP PROCEDURE SP_NHAPDIEM_SINHVIEN;
GO

CREATE PROCEDURE SP_NHAPDIEM_SINHVIEN
	@ENC_DIEM VARBINARY(MAX),
    @MASV VARCHAR(20),
    @MAHP VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra xem đã tồn tại sinh viên và học phần chưa
    IF EXISTS (
        SELECT 1 
        FROM BANGDIEM 
        WHERE MASV = @MASV AND MAHP = @MAHP
    )
    BEGIN
        -- Nếu tồn tại thì UPDATE điểm
        UPDATE BANGDIEM
        SET DIEMTHI = @ENC_DIEM
        WHERE MASV = @MASV AND MAHP = @MAHP;
    END
    ELSE
    BEGIN
        -- Nếu chưa tồn tại thì INSERT mới
        INSERT INTO BANGDIEM (MASV, MAHP, DIEMTHI)
        VALUES (@MASV, @MAHP, @ENC_DIEM);

		SELECT * FROM BANGDIEM 
    END
END
GO

-- Stored procedure tìm học phần theo tên
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_TIM_HOCPHAN_THEOTEN')
    DROP PROCEDURE SP_TIM_HOCPHAN_THEOTEN;
GO

-- Tìm mã học phần 
CREATE PROCEDURE SP_TIM_HOCPHAN_THEOTEN
    @TenLop NVARCHAR(100) -- Thật ra là Tên học phần
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MAHP,         -- Mã học phần
        TENHP,        -- Tên học phần
        SOTC          -- Số tín chỉ
    FROM 
        HOCPHAN
    WHERE 
        TENHP = @TenLop; -- So sánh tên học phần với tham số đầu vào
END
GO


-- Xoá stored procedure SP_CAPNHAT_SINHVIEN nếu đã tồn tại
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CAPNHAT_SINHVIEN')
    DROP PROCEDURE SP_CAPNHAT_SINHVIEN;
GO

-- Stored procedure cập nhật thông tin sinh viên
CREATE PROCEDURE SP_CAPNHAT_SINHVIEN
(
    @MASV VARCHAR(20),
    @HOTEN NVARCHAR(100),
    @NGAYSINH DATETIME = NULL,
    @DIACHI NVARCHAR(200) = NULL,
    @MALOP VARCHAR(20) = NULL,
    @TENDN NVARCHAR(100),
    @MATKHAU VARBINARY(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra: Sinh viên có tồn tại không
    IF NOT EXISTS (SELECT 1 FROM SINHVIEN WHERE MASV = @MASV)
    BEGIN
        RAISERROR(N'Mã sinh viên %s không tồn tại.', 16, 1, @MASV)
        RETURN
    END

    -- Kiểm tra: Tên đăng nhập có trùng với sinh viên khác không
    IF EXISTS (
        SELECT 1 FROM SINHVIEN
        WHERE TENDN = @TENDN AND MASV <> @MASV
    )
    BEGIN
        RAISERROR(N'Tên đăng nhập %s đã được sử dụng bởi sinh viên khác.', 16, 1, @TENDN)
        RETURN
    END
    -- Cập nhật dữ liệu
    UPDATE SINHVIEN
    SET 
        HOTEN = @HOTEN,
        NGAYSINH = @NGAYSINH,
        DIACHI = @DIACHI,
        MALOP = @MALOP,
        TENDN = @TENDN,
        MATKHAU = @MATKHAU
    WHERE MASV = @MASV
END
GO


