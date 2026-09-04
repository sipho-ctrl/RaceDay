-- =============================================
-- RACEDAY DATABASE SCRIPT
-- Part 1 - System Planning and Database
-- Student: Sipho Swartbooi
-- Student Number: ST10467895
-- Date: September 2026
-- =============================================

-- =============================================
-- CREATE DATABASE
-- =============================================
CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

-- =============================================
-- DROP EXISTING TABLES (if re-running)
-- =============================================
IF OBJECT_ID('Results', 'U') IS NOT NULL DROP TABLE Results;
IF OBJECT_ID('Enrolments', 'U') IS NOT NULL DROP TABLE Enrolments;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Events', 'U') IS NOT NULL DROP TABLE Events;
IF OBJECT_ID('Participants', 'U') IS NOT NULL DROP TABLE Participants;
IF OBJECT_ID('Organisers', 'U') IS NOT NULL DROP TABLE Organisers;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

-- =============================================
-- 1. CREATE TABLES
-- =============================================

-- Users Table (Base table for authentication)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- Organisers Table
CREATE TABLE Organisers (
    OrganiserID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    OrganisationName NVARCHAR(100) NOT NULL,
    ContactNumber NVARCHAR(15) NOT NULL,
    CONSTRAINT FK_Organisers_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Participants Table
CREATE TABLE Participants (
    ParticipantID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10),
    ContactNumber NVARCHAR(15),
    EmergencyContact NVARCHAR(100),
    CONSTRAINT FK_Participants_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Events Table
CREATE TABLE Events (
    EventID INT PRIMARY KEY IDENTITY(1,1),
    OrganiserID INT NOT NULL,
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    MaxParticipants INT,
    RegistrationDeadline DATE,
    EventStatus NVARCHAR(20) DEFAULT 'Open' CHECK (EventStatus IN ('Open', 'Closed', 'Cancelled')),
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Events_Organisers FOREIGN KEY (OrganiserID) REFERENCES Organisers(OrganiserID)
);
GO

-- Categories Table
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    EventID INT NOT NULL,
    CategoryName NVARCHAR(50) NOT NULL,
    Distance DECIMAL(5,2) NOT NULL,
    AgeGroup NVARCHAR(20),
    GenderGroup NVARCHAR(10),
    EntryFee DECIMAL(10,2),
    MaxParticipants INT,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE
);
GO

-- Enrolments Table
CREATE TABLE Enrolments (
    EnrolmentID INT PRIMARY KEY IDENTITY(1,1),
    ParticipantID INT NOT NULL,
    CategoryID INT NOT NULL,
    RegistrationDate DATETIME DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Paid', 'Cancelled')),
    StartNumber INT UNIQUE,
    CONSTRAINT FK_Enrolments_Participants FOREIGN KEY (ParticipantID) REFERENCES Participants(ParticipantID),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

-- Results Table
CREATE TABLE Results (
    ResultID INT PRIMARY KEY IDENTITY(1,1),
    EnrolmentID INT NOT NULL,
    FinishTime TIME(3) NOT NULL,
    OverallPosition INT,
    CategoryPosition INT,
    Pace DECIMAL(5,2),
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Disqualified')),
    RecordedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID)
);
GO

-- =============================================
-- 2. INSERT SAMPLE DATA (Minimum requirements)
-- =============================================

-- Insert Users (4 users: 2 Organisers, 2 Participants)
INSERT INTO Users (Username, Email, PasswordHash, Role) VALUES
('organiser1', 'organiser1@raceday.com', 'hashedpassword123', 'Organiser'),
('organiser2', 'organiser2@raceday.com', 'hashedpassword456', 'Organiser'),
('runner1', 'sipho.nkosi@email.com', 'hashedpassword789', 'Participant'),
('runner2', 'lindiwe.mthembu@email.com', 'hashedpassword101', 'Participant');
GO

-- Insert Organisers (2 organisers)
INSERT INTO Organisers (UserID, OrganisationName, ContactNumber) VALUES
(1, 'Cape Town Events Management', '0821234567'),
(2, 'Durban Running Club', '0839876543');
GO

-- Insert Participants (2 participants)
INSERT INTO Participants (UserID, FullName, DateOfBirth, Gender, ContactNumber, EmergencyContact) VALUES
(3, 'Sipho Nkosi', '1990-05-15', 'Male', '0712345678', '0823456789'),
(4, 'Lindiwe Mthembu', '1988-10-22', 'Female', '0734567890', '0845678901');
GO

-- Insert Events (3 events)
INSERT INTO Events (OrganiserID, EventName, EventDate, Location, Description, MaxParticipants, RegistrationDeadline, EventStatus) VALUES
(1, 'Cape Town Cycle Tour', '2026-03-08', 'Cape Town', 'Iconic cycle race through Cape Town - 109km route', 35000, '2026-03-01', 'Open'),
(1, 'Two Oceans Marathon', '2026-04-11', 'Cape Town', '56km ultra marathon - the worlds most beautiful marathon', 12000, '2026-04-01', 'Open'),
(2, 'Comrades Marathon', '2026-06-14', 'Durban', 'Up-run from Durban to Pietermaritzburg - 89km', 25000, '2026-05-15', 'Open');
GO

-- Insert Categories (at least 2 per event)
-- Cape Town Cycle Tour Categories
INSERT INTO Categories (EventID, CategoryName, Distance, AgeGroup, GenderGroup, EntryFee, MaxParticipants) VALUES
(1, 'Elite Men', 109.00, '18-34', 'Male', 450.00, 500),
(1, 'Elite Women', 109.00, '18-34', 'Female', 450.00, 300),
(1, 'Veteran Men', 109.00, '35-49', 'Male', 350.00, 1000),
(1, 'Veteran Women', 109.00, '35-49', 'Female', 350.00, 800);
GO

-- Two Oceans Marathon Categories
INSERT INTO Categories (EventID, CategoryName, Distance, AgeGroup, GenderGroup, EntryFee, MaxParticipants) VALUES
(2, 'Ultra Marathon', 56.00, '18-39', 'All', 300.00, 8000),
(2, 'Half Marathon', 21.10, '18-39', 'All', 200.00, 4000);
GO

-- Comrades Marathon Categories
INSERT INTO Categories (EventID, CategoryName, Distance, AgeGroup, GenderGroup, EntryFee, MaxParticipants) VALUES
(3, 'Ultra Marathon', 89.00, '18-39', 'All', 550.00, 15000);
GO

-- Insert Enrolments (sample enrolments)
INSERT INTO Enrolments (ParticipantID, CategoryID, PaymentStatus, StartNumber) VALUES
(1, 1, 'Paid', 101),
(1, 5, 'Paid', 102),
(2, 2, 'Paid', 103);
GO

-- Insert Results (sample results for enrolments)
INSERT INTO Results (EnrolmentID, FinishTime, OverallPosition, CategoryPosition, Pace, Status) VALUES
(1, '03:15:30.000', 15, 8, 3.15, 'Confirmed'),
(2, '05:45:20.000', 42, 12, 5.45, 'Confirmed');
GO

-- =============================================
-- 3. VERIFY DATA (Display counts)
-- =============================================
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Organisers', COUNT(*) FROM Organisers
UNION ALL
SELECT 'Participants', COUNT(*) FROM Participants
UNION ALL
SELECT 'Events', COUNT(*) FROM Events
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Enrolments', COUNT(*) FROM Enrolments
UNION ALL
SELECT 'Results', COUNT(*) FROM Results;
GO

-- =============================================
-- 4. VIEW SAMPLE DATA (Optional verification)
-- =============================================
-- View all events with organiser names
SELECT 
    e.EventName,
    e.EventDate,
    e.Location,
    o.OrganisationName,
    e.EventStatus
FROM Events e
INNER JOIN Organisers o ON e.OrganiserID = o.OrganiserID;
GO

-- View all enrolments with participant and event details
SELECT 
    p.FullName,
    ev.EventName,
    c.CategoryName,
    en.PaymentStatus,
    en.StartNumber
FROM Enrolments en
INNER JOIN Participants p ON en.ParticipantID = p.ParticipantID
INNER JOIN Categories c ON en.CategoryID = c.CategoryID
INNER JOIN Events ev ON c.EventID = ev.EventID;
GO