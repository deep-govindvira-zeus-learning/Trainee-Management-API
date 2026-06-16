CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

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

CREATE TABLE `Users` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Username` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Role` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_User_Role` CHECK (`Role` IN ('Admin', 'Mentor', 'Trainee'))
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

CREATE INDEX `IX_Assignments_LearningTaskId` ON `Assignments` (`LearningTaskId`);

CREATE INDEX `IX_Assignments_MentorId` ON `Assignments` (`MentorId`);

CREATE INDEX `IX_Assignments_TraineeId` ON `Assignments` (`TraineeId`);

CREATE INDEX `IX_Reviews_MentorId` ON `Reviews` (`MentorId`);

CREATE INDEX `IX_Reviews_SubmissionId` ON `Reviews` (`SubmissionId`);

CREATE INDEX `IX_Submissions_AssignmentId` ON `Submissions` (`AssignmentId`);

CREATE UNIQUE INDEX `IX_Users_Username` ON `Users` (`Username`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260616061403_InitialCreate', '9.0.0');

COMMIT;

