using AppointmentManagementService.Context;
using AppointmentManagementService.Dto;
using AppointmentManagementService.Entity;
using AppointmentManagementService.Interface;
using Dapper;

namespace AppointmentManagementService.Service
{
    public class AppointmentService : IAppointment
    {
        private readonly AppointmentManagementServiceContext _context;

        public AppointmentService(AppointmentManagementServiceContext context)
        {
            _context = context;
        }


        public async Task<AppointmentResponseModel> AddAppointment(AppointmentEntity appointment, int PatientID, int DoctorID)
        {
            try
            {
                string insertQuery = @"
            INSERT INTO Appointments (AppointmentDate, DoctorID, PatientID)
            VALUES (@AppointmentDate, @DoctorID, @PatientID);
            SELECT SCOPE_IDENTITY();
        ";

                using (var connection = _context.CreateConnection())
                {
                    var appointmentId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                    {
                        appointment.AppointmentDate,
                        DoctorID,
                        PatientID
                    });

                    // Query the newly added appointment from the database
                    string selectQuery = @"
                SELECT AppointmentID, AppointmentDate, DoctorID, PatientID
                FROM Appointments
                WHERE AppointmentID = @AppointmentId;
            ";
                    var addedAppointment = await connection.QueryFirstOrDefaultAsync<AppointmentResponseModel>(selectQuery, new { AppointmentId = appointmentId });

                    return addedAppointment;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and wrap them in a custom exception if necessary
                throw new Exception("An error occurred while adding a new appointment.", ex);
            }
        }

        public async Task<bool> DeleteAppointment(int appointmentId, int userId)
        {
            try
            {
                string deleteQuery = @"
                    DELETE FROM Appointments
                    WHERE AppointmentID = @AppointmentId AND PatientID = @UserId;
                ";

                using (var connection = _context.CreateConnection())
                {
                    // Execute the delete query
                    var affectedRows = await connection.ExecuteAsync(deleteQuery, new { AppointmentId = appointmentId, UserId = userId });

                    // Return true if at least one row was affected, indicating successful deletion
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and log or throw custom exception if necessary
                throw new Exception("An error occurred while deleting the appointment.", ex);
            }
        }

        public async Task<IEnumerable<AppointmentEntity>> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                string selectQuery = @"
                    SELECT * 
                    FROM Appointments 
                    WHERE DoctorID = @DoctorId;
                ";

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentEntity>(selectQuery, new { DoctorId = doctorId });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and log or throw custom exception if necessary
                throw new Exception("An error occurred while retrieving appointments by doctor.", ex);
            }
        }

        public async Task<IEnumerable<AppointmentEntity>> GetAppointmentsByPatient(int patientId)
        {
            try
            {
                string selectQuery = @"
                    SELECT * 
                    FROM Appointments 
                    WHERE PatientID = @PatientId;
                ";

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentEntity>(selectQuery, new { PatientId = patientId });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and log or throw custom exception if necessary
                throw new Exception("An error occurred while retrieving appointments by patient.", ex);
            }
        }

        public async Task<bool> UpdateAppointmentDate(int appointmentId, DateTime newDate)
        {
            try
            {
                string updateQuery = @"
                    UPDATE Appointments 
                    SET AppointmentDate = @NewDate 
                    WHERE AppointmentID = @AppointmentId;
                ";

                using (var connection = _context.CreateConnection())
                {
                    // Execute the update query
                    var affectedRows = await connection.ExecuteAsync(updateQuery, new { NewDate = newDate, AppointmentId = appointmentId });

                    // Return true if at least one row was affected, indicating successful update
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and log or throw custom exception if necessary
                throw new Exception("An error occurred while updating appointment date.", ex);
            }
        }
    }


}

