CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY IDENTITY,
    AppointmentDate DATETIME,
    DoctorID INT,
    PatientID INT
);
