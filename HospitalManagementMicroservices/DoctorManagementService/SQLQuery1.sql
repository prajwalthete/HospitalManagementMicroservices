CREATE TABLE Doctors (
    DoctorID INT PRIMARY KEY,
    Specialization VARCHAR(255),
    IsAvailable BIT,
    Qualifications VARCHAR(MAX),
    Schedule VARCHAR(MAX)
);
