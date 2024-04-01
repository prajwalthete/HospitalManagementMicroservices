using Dapper;
using PatientManagementService.Context;
using PatientManagementService.Dto;
using PatientManagementService.Entity;
using PatientManagementService.GlobleExceptionhandler;
using PatientManagementService.Interface;

namespace PatientManagementService.Service
{
    public class PatientService : IPatient
    {
        private readonly PatientManagementServiceContext _context;

        public PatientService(PatientManagementServiceContext context)
        {
            _context = context;
        }


        public async Task<PatientResponseModel> CreatePatientDetails(PatientEntity patientEntity, int UserID)
        {
            try
            {
                // Construct the insert query
                string insertQuery = @"
                    INSERT INTO Patients (PatientID, MedicalHistory, Insurance, Gender, DOB)
                    VALUES (@PatientID, @MedicalHistory, @Insurance, @Gender, @DOB);
                    SELECT SCOPE_IDENTITY();
                ";

                // Execute the insert query
                using (var connection = _context.CreateConnection())
                {
                    // ExecuteAsync method returns the newly inserted patient ID
                    var patient = await connection.ExecuteScalarAsync<PatientResponseModel>(insertQuery, new
                    {
                        PatientID = UserID,
                        patientEntity.MedicalHistory,
                        patientEntity.Insurance,
                        patientEntity.Gender,
                        patientEntity.DOB
                    });
                    return patient;


                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and wrap them in a custom exception if necessary
                throw new PatientCreationException("An error occurred while creating a new patient.", ex);
            }
        }
    }
}
