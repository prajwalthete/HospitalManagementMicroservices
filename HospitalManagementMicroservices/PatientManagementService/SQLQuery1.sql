CREATE TABLE Patients (
    PatientID INT PRIMARY KEY, -- Primary Key for Patient table which is UserId from UserTbale
    MedicalHistory NVARCHAR(MAX),
    Insurance NVARCHAR(MAX),
    Gender NVARCHAR(10),
    DOB DATE
);
