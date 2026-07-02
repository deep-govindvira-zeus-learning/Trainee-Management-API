CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `JobStatuses` (
    `Id` int NOT NULL,
    `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_JobStatuses` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `LearningTasks` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Title` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ExpectedTechStack` longtext CHARACTER SET utf8mb4 NOT NULL,
    `DueDate` date NOT NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    CONSTRAINT `PK_LearningTasks` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_LearningTask_Status` CHECK (`Status` IN ('Draft', 'Published', 'Closed'))
) CHARACTER SET=utf8mb4;

CREATE TABLE `Mentors` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `FirstName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `LastName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Expertise` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    CONSTRAINT `PK_Mentors` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Mentor_Status` CHECK (`Status` IN ('Active', 'Inactive'))
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProcessingJobs` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `SubmissionId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FileId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `MessageId` char(36) COLLATE ascii_general_ci NOT NULL,
    `CorrelationId` char(36) COLLATE ascii_general_ci NOT NULL,
    `Status` int NOT NULL,
    `Attempts` int NOT NULL,
    `ErrorSummary` longtext CHARACTER SET utf8mb4 NULL,
    `GeneratedChecksum` longtext CHARACTER SET utf8mb4 NULL,
    `OutputFilePath` longtext CHARACTER SET utf8mb4 NULL,
    `RequestedAt` datetime(6) NOT NULL,
    `StartedAt` datetime(6) NULL,
    `CompletedAt` datetime(6) NULL,
    CONSTRAINT `PK_ProcessingJobs` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Trainees` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `FirstName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `LastName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TechStack` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    CONSTRAINT `PK_Trainees` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Trainee_Status` CHECK (`Status` IN ('Active', 'Inactive', 'Completed'))
) CHARACTER SET=utf8mb4;

CREATE TABLE `UserRoles` (
    `Id` int NOT NULL,
    `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_UserRoles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Assignments` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `TraineeId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `MentorId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LearningTaskId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `AssignedDate` date NOT NULL,
    `DueDate` date NOT NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Remarks` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    CONSTRAINT `PK_Assignments` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Assignment_Status` CHECK (`Status` IN ('Assigned', 'InProgress', 'Submitted', 'Reviewed', 'Completed')),
    CONSTRAINT `FK_Assignments_LearningTasks_LearningTaskId` FOREIGN KEY (`LearningTaskId`) REFERENCES `LearningTasks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Assignments_Mentors_MentorId` FOREIGN KEY (`MentorId`) REFERENCES `Mentors` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Assignments_Trainees_TraineeId` FOREIGN KEY (`TraineeId`) REFERENCES `Trainees` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Users` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Username` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Role` int NOT NULL,
    `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_User_Role` CHECK (`Role` IN ('0', '1', '2')),
    CONSTRAINT `FK_Users_UserRoles_Role` FOREIGN KEY (`Role`) REFERENCES `UserRoles` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Submissions` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `AssignmentId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `SubmissionUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Notes` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SubmittedDate` date NOT NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Submissions` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Submission_Status` CHECK (`Status` IN ('Submitted', 'Resubmitted')),
    CONSTRAINT `FK_Submissions_Assignments_AssignmentId` FOREIGN KEY (`AssignmentId`) REFERENCES `Assignments` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Reviews` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `SubmissionId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `MentorId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Feedback` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Score` int NULL,
    `ReviewStatus` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ReviewedDate` date NOT NULL,
    CONSTRAINT `PK_Reviews` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Review_Status` CHECK (`ReviewStatus` IN ('Accepted', 'ChangesRequired', 'Rejected')),
    CONSTRAINT `FK_Reviews_Mentors_MentorId` FOREIGN KEY (`MentorId`) REFERENCES `Mentors` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Reviews_Submissions_SubmissionId` FOREIGN KEY (`SubmissionId`) REFERENCES `Submissions` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `SubmissionFiles` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `SubmissionId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `OriginalFileName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `StorageName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ContentType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `SizeInBytes` bigint NOT NULL,
    `Checksum` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `UploadedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    CONSTRAINT `PK_SubmissionFiles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SubmissionFiles_Submissions_SubmissionId` FOREIGN KEY (`SubmissionId`) REFERENCES `Submissions` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

INSERT INTO `JobStatuses` (`Id`, `Name`)
VALUES (0, 'Queued'),
(1, 'Processing'),
(2, 'Completed'),
(3, 'Failed');

INSERT INTO `UserRoles` (`Id`, `Name`)
VALUES (0, 'Admin'),
(1, 'Mentor'),
(2, 'Trainee');

CREATE INDEX `IX_Assignments_LearningTaskId` ON `Assignments` (`LearningTaskId`);

CREATE INDEX `IX_Assignments_MentorId` ON `Assignments` (`MentorId`);

CREATE INDEX `IX_Assignments_TraineeId` ON `Assignments` (`TraineeId`);

CREATE INDEX `IX_ProcessingJobs_FileId` ON `ProcessingJobs` (`FileId`);

CREATE UNIQUE INDEX `IX_ProcessingJobs_MessageId` ON `ProcessingJobs` (`MessageId`);

CREATE INDEX `IX_Reviews_MentorId` ON `Reviews` (`MentorId`);

CREATE INDEX `IX_Reviews_SubmissionId` ON `Reviews` (`SubmissionId`);

CREATE INDEX `IX_SubmissionFiles_SubmissionId` ON `SubmissionFiles` (`SubmissionId`);

CREATE INDEX `IX_Submissions_AssignmentId` ON `Submissions` (`AssignmentId`);

CREATE INDEX `IX_Users_Role` ON `Users` (`Role`);

CREATE UNIQUE INDEX `IX_Users_Username` ON `Users` (`Username`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260630085421_InitialCreate', '9.0.0');

COMMIT;

