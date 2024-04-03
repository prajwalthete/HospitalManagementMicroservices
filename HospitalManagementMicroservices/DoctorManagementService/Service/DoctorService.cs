using Dapper;
using DoctorManagementService.Context;
using DoctorManagementService.Entity;
using DoctorManagementService.GlobleExceptionhandler;
using DoctorManagementService.Interface;

namespace DoctorManagementService.Service
{
    public class DoctorService : IDoctor
    {
        private readonly DoctorManagementServiceContext _context;

        public DoctorService(DoctorManagementServiceContext context)
        {
            _context = context;
        }

        public async Task<DoctorEntity> AddDoctor(int DoctorID, DoctorEntity doctorEntity)
        {
            try
            {
                string insertQuery = @"
                    INSERT INTO Doctors (DoctorID, Specialization, IsAvailable, Qualifications, Schedule)
                    VALUES (@DoctorID, @Specialization, @IsAvailable, @Qualifications, @Schedule);
                ";

                using (var connection = _context.CreateConnection())
                {
                    // Execute the insert query
                    await connection.QueryFirstOrDefaultAsync(insertQuery, new
                    {
                        DoctorID = DoctorID,
                        doctorEntity.Specialization,
                        doctorEntity.IsAvailable,
                        doctorEntity.Qualifications,
                        doctorEntity.Schedule
                    });


                    // Query the newly added doctor from the database
                    string selectQuery = @"
                        SELECT *
                        FROM Doctors
                        WHERE DoctorID = @DoctorID;
                    ";
                    var addedDoctor = await connection.QueryFirstOrDefaultAsync<DoctorEntity>(selectQuery, new { DoctorID });

                    return addedDoctor;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and wrap them in a custom exception if necessary
                throw new DoctorCreationException("An error occurred while adding a new doctor.", ex);
            }
        }


        public async Task<bool> DeleteDoctor(int DoctorID)
        {
            try
            {
                string deleteQuery = @"
                    DELETE FROM Doctors
                    WHERE DoctorID = @DoctorID;
                ";

                using (var connection = _context.CreateConnection())
                {
                    // Execute the delete query
                    var affectedRows = await connection.ExecuteAsync(deleteQuery, new { DoctorID });

                    // Return true if at least one row was affected, indicating successful deletion
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and wrap them in a custom exception if necessary
                throw new DoctorDeletionException("An error occurred while deleting the doctor.", ex);
            }
        }

        public async Task<IEnumerable<DoctorEntity>> GetDoctorsBySpecialization(string specialization)
        {
            try
            {
                string selectQuery = @"
                    SELECT *
                    FROM Doctors
                    WHERE Specialization = @Specialization;
                ";

                using (var connection = _context.CreateConnection())
                {
                    var doctors = await connection.QueryAsync<DoctorEntity>(selectQuery, new { Specialization = specialization });
                    return doctors;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and wrap them in a custom exception if necessary
                throw new DoctorRetrievalException("An error occurred while retrieving doctors by specialization.", ex);
            }


        }

        public async Task<bool> UpdateSchedule(int doctorId, string newSchedule)
        {
            try
            {
                string updateQuery = @"
                    UPDATE Doctors
                    SET Schedule = @NewSchedule
                    WHERE DoctorID = @DoctorID;
                ";

                using (var connection = _context.CreateConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(updateQuery, new { DoctorID = doctorId, NewSchedule = newSchedule });

                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                throw new DoctorUpdateException("An error occurred while updating the doctor's schedule.", ex);
            }
        }
    }
}
