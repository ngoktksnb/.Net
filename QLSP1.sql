-- Tạo cơ sở dữ liệu
CREATE DATABASE QLSP1;
GO

-- Sử dụng cơ sở dữ liệu vừa tạo
USE QLSP1;
GO

-- Tạo bảng tbl_DanhSach
CREATE TABLE tbl_DanhSach (
    IDDS INT PRIMARY KEY IDENTITY(1,1),
    TenDanhSach NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(1000),
    IDDanhSachCha INT
);
GO

-- Tạo bảng tbl_SanPham
CREATE TABLE tbl_SanPham (
    IDSP INT PRIMARY KEY IDENTITY(1,1),
    MaSP NVARCHAR(50) NOT NULL,
    MoTa NVARCHAR(1000),
    HinhAnh TEXT,
    IDDS INT,
    CONSTRAINT FK_tbl_SanPham_tbl_DanhSach FOREIGN KEY (IDDS) REFERENCES tbl_DanhSach(IDDS)
);
GO


-- Tạo bảng tbl_ảnh bìa
CREATE TABLE tbl_AnhBia (
    IDAB INT PRIMARY KEY IDENTITY(1,1),
    HinhAnhBia TEXT,
    
);
GO

-- Tạo bảng tbl_NguoiDung
CREATE TABLE tbl_NguoiDung (
    id INT PRIMARY KEY IDENTITY(1,1),
    email NVARCHAR(255) NOT NULL,
    matkhau NVARCHAR(255) NOT NULL,
    hoten NVARCHAR(255) NOT NULL,
    mssv NVARCHAR(50) NOT NULL,
    isDelete BIT NOT NULL DEFAULT 0
);
GO
select * from tbl_NguoiDung