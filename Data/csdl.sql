CREATE DATABASE  IF NOT EXISTS `quanly_thuctap` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `quanly_thuctap`;
-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: quanly_thuctap
-- ------------------------------------------------------
-- Server version	9.7.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ '09729ebc-4f7a-11f1-b7e5-d45d64ee6360:1-207';

--
-- Table structure for table `admin`
--

DROP TABLE IF EXISTS `admin`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `admin` (
  `id_admin` int NOT NULL AUTO_INCREMENT,
  `ten_admin` varchar(255) NOT NULL,
  `taikhoan_admin` varchar(255) DEFAULT NULL,
  `password_admin` varchar(255) NOT NULL,
  PRIMARY KEY (`id_admin`),
  UNIQUE KEY `taikhoan_admin` (`taikhoan_admin`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `danh_gia_thuc_tap`
--

DROP TABLE IF EXISTS `danh_gia_thuc_tap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `danh_gia_thuc_tap` (
  `id_danh_gia` int NOT NULL AUTO_INCREMENT,
  `mssv` varchar(20) NOT NULL,
  PRIMARY KEY (`id_danh_gia`),
  KEY `mssv` (`mssv`),
  CONSTRAINT `danh_gia_thuc_tap_ibfk_1` FOREIGN KEY (`mssv`) REFERENCES `sinh_vien_thuc_tap` (`mssv`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `diem_clo`
--

DROP TABLE IF EXISTS `diem_clo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `diem_clo` (
  `id_diem_clo` int NOT NULL AUTO_INCREMENT,
  `id_danh_gia` int NOT NULL,
  `nguoi_cham_loai` varchar(20) NOT NULL,
  `nguoi_cham_id` varchar(50) NOT NULL,
  `ma_clo` varchar(10) NOT NULL,
  `diem_chang_1` decimal(4,1) DEFAULT NULL,
  `diem_chang_2` decimal(4,1) DEFAULT NULL,
  PRIMARY KEY (`id_diem_clo`),
  KEY `id_danh_gia` (`id_danh_gia`),
  CONSTRAINT `diem_clo_ibfk_1` FOREIGN KEY (`id_danh_gia`) REFERENCES `danh_gia_thuc_tap` (`id_danh_gia`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `don_vi_hd`
--

DROP TABLE IF EXISTS `don_vi_hd`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `don_vi_hd` (
  `id_don_vi_hd` int NOT NULL AUTO_INCREMENT,
  `id_ki_thuc_tap` int DEFAULT NULL,
  `ten_don_vi_hd` varchar(255) NOT NULL,
  `gmail_don_vi_hd` varchar(255) DEFAULT NULL,
  `password_don_vi_hd` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id_don_vi_hd`),
  UNIQUE KEY `gmail_don_vi_hd` (`gmail_don_vi_hd`),
  KEY `fk_donvi_kithuctap` (`id_ki_thuc_tap`),
  CONSTRAINT `fk_donvi_kithuctap` FOREIGN KEY (`id_ki_thuc_tap`) REFERENCES `ki_thuc_tap` (`id_ki_thuc_tap`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=504 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `giang_vien`
--

DROP TABLE IF EXISTS `giang_vien`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `giang_vien` (
  `ma_so_giang_vien` varchar(20) NOT NULL,
  `ten_giang_vien` varchar(255) NOT NULL,
  `id_khoa` int DEFAULT NULL,
  `gmail_giang_vien` varchar(255) DEFAULT NULL,
  `password_giang_vien` varchar(255) NOT NULL,
  PRIMARY KEY (`ma_so_giang_vien`),
  UNIQUE KEY `gmail_giang_vien` (`gmail_giang_vien`),
  KEY `fk_giangvien_khoa` (`id_khoa`),
  CONSTRAINT `fk_giangvien_khoa` FOREIGN KEY (`id_khoa`) REFERENCES `khoa` (`id_khoa`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `giang_vien_thuc_tap`
--

DROP TABLE IF EXISTS `giang_vien_thuc_tap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `giang_vien_thuc_tap` (
  `ma_so_giang_vien` varchar(20) NOT NULL,
  `id_ki_thuc_tap` int NOT NULL,
  PRIMARY KEY (`ma_so_giang_vien`,`id_ki_thuc_tap`),
  KEY `fk_gvtt_kithuctap` (`id_ki_thuc_tap`),
  CONSTRAINT `fk_gvtt_giangvien` FOREIGN KEY (`ma_so_giang_vien`) REFERENCES `giang_vien` (`ma_so_giang_vien`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_gvtt_kithuctap` FOREIGN KEY (`id_ki_thuc_tap`) REFERENCES `ki_thuc_tap` (`id_ki_thuc_tap`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `khoa`
--

DROP TABLE IF EXISTS `khoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `khoa` (
  `id_khoa` int NOT NULL AUTO_INCREMENT,
  `ten_khoa` varchar(255) NOT NULL,
  `gmail_khoa` varchar(255) DEFAULT NULL,
  `password_khoa` varchar(255) NOT NULL,
  `path_cau_hinh_diem` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id_khoa`),
  UNIQUE KEY `gmail_khoa` (`gmail_khoa`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ki_thuc_tap`
--

DROP TABLE IF EXISTS `ki_thuc_tap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ki_thuc_tap` (
  `id_ki_thuc_tap` int NOT NULL AUTO_INCREMENT,
  `ten_ki_thuc_tap` varchar(255) NOT NULL,
  `time_batdau` datetime DEFAULT NULL,
  `time_ketthuc` datetime DEFAULT NULL,
  `trang_thai` varchar(45) NOT NULL,
  `id_khoa` int DEFAULT NULL,
  PRIMARY KEY (`id_ki_thuc_tap`),
  KEY `fk_kithuctap_khoa` (`id_khoa`),
  CONSTRAINT `fk_kithuctap_khoa` FOREIGN KEY (`id_khoa`) REFERENCES `khoa` (`id_khoa`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=1003 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `log_thuc_tap`
--

DROP TABLE IF EXISTS `log_thuc_tap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `log_thuc_tap` (
  `id_log` int NOT NULL AUTO_INCREMENT,
  `mssv` varchar(20) DEFAULT NULL,
  `id_ki_thuc_tap` int DEFAULT NULL,
  `noi_dung` text,
  `file_path` varchar(500) DEFAULT NULL,
  `ngay_tao` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_log`),
  KEY `fk_log_sinhvien` (`mssv`),
  KEY `fk_log_kithuctap` (`id_ki_thuc_tap`),
  CONSTRAINT `fk_log_kithuctap` FOREIGN KEY (`id_ki_thuc_tap`) REFERENCES `ki_thuc_tap` (`id_ki_thuc_tap`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_log_sinhvien` FOREIGN KEY (`mssv`) REFERENCES `sinh_vien` (`mssv`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sinh_vien`
--

DROP TABLE IF EXISTS `sinh_vien`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sinh_vien` (
  `mssv` varchar(20) NOT NULL,
  `id_khoa` int DEFAULT NULL,
  `ten_sinh_vien` varchar(255) NOT NULL,
  `gmail_sinh_vien` varchar(255) DEFAULT NULL,
  `password_sinh_vien` varchar(255) NOT NULL,
  PRIMARY KEY (`mssv`),
  UNIQUE KEY `gmail_sinh_vien` (`gmail_sinh_vien`),
  KEY `fk_sinhvien_khoa` (`id_khoa`),
  CONSTRAINT `fk_sinhvien_khoa` FOREIGN KEY (`id_khoa`) REFERENCES `khoa` (`id_khoa`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sinh_vien_thuc_tap`
--

DROP TABLE IF EXISTS `sinh_vien_thuc_tap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sinh_vien_thuc_tap` (
  `mssv` varchar(20) NOT NULL,
  `id_ki_thuc_tap` int NOT NULL,
  `id_don_vi_hd` int DEFAULT NULL,
  `ma_so_giang_vien` varchar(20) DEFAULT NULL,
  `trang_thai` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`mssv`,`id_ki_thuc_tap`),
  UNIQUE KEY `uq_svtt_mssv_ki` (`mssv`,`id_ki_thuc_tap`),
  UNIQUE KEY `uq_svtt_mssv` (`mssv`),
  UNIQUE KEY `uq_sinh_vien_thuc_tap_mssv` (`mssv`),
  KEY `fk_svtt_kithuctap` (`id_ki_thuc_tap`),
  KEY `fk_svtt_donvi` (`id_don_vi_hd`),
  KEY `fk_svtt_giangvien` (`ma_so_giang_vien`),
  CONSTRAINT `fk_svtt_donvi` FOREIGN KEY (`id_don_vi_hd`) REFERENCES `don_vi_hd` (`id_don_vi_hd`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_svtt_giangvien` FOREIGN KEY (`ma_so_giang_vien`) REFERENCES `giang_vien` (`ma_so_giang_vien`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_svtt_kithuctap` FOREIGN KEY (`id_ki_thuc_tap`) REFERENCES `ki_thuc_tap` (`id_ki_thuc_tap`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_svtt_sinhvien` FOREIGN KEY (`mssv`) REFERENCES `sinh_vien` (`mssv`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `thong_bao`
--

DROP TABLE IF EXISTS `thong_bao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thong_bao` (
  `id_thong_bao` int NOT NULL AUTO_INCREMENT,
  `loai_nguoi_nhan` varchar(50) DEFAULT NULL,
  `ma_nguoi_nhan` varchar(50) DEFAULT NULL,
  `tieu_de` varchar(255) DEFAULT NULL,
  `noi_dung` text,
  `ngay_tao` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_thong_bao`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `minh_chung`
--

DROP TABLE IF EXISTS `minh_chung`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `minh_chung` (
  `id_minh_chung` int NOT NULL AUTO_INCREMENT,
  `ten_minh_chung` varchar(255) NOT NULL,
  `mssv` varchar(20) NOT NULL,
  `path` varchar(500) NOT NULL,
  PRIMARY KEY (`id_minh_chung`),
  KEY `fk_minhchung_sinhvien` (`mssv`),
  CONSTRAINT `fk_minhchung_sinhvien` FOREIGN KEY (`mssv`) REFERENCES `sinh_vien` (`mssv`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-11 13:51:02
