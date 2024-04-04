using DoctorManagementService.Dto;
using DoctorManagementService.Entity;
using DoctorManagementService.GlobleExceptionhandler;
using DoctorManagementService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace DoctorManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctor _doctorService;
        private readonly HttpClient _httpClient;

        public DoctorController(IDoctor doctorService, HttpClient httpClient)
        {
            _doctorService = doctorService;
            _httpClient = httpClient;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorEntity doctorEntity, int DoctorID)
        {
            try
            {
                // Retrieve the authenticated user's role
                var role = User.FindFirstValue(ClaimTypes.Role);

                // Check if the user is authorized to add a doctor
                if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                // Call the doctor service to add the doctor
                var addedDoctor = await _doctorService.AddDoctor(DoctorID, doctorEntity);

                // Return the added doctor details
                return Ok(new { Success = true, Message = "Doctor added successfully", Data = addedDoctor });
            }
            catch (DoctorCreationException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteDoctor(int DoctorID)
        {
            try
            {
                var role = User.FindFirstValue(ClaimTypes.Role);

                // Check if the user is authorized to add a doctor
                if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
                // Call the doctor service to delete the doctor
                var isDeleted = await _doctorService.DeleteDoctor(DoctorID);

                if (isDeleted)
                {
                    return Ok(new { Success = true, Message = "Doctor deleted successfully" });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Doctor not found" });
                }
            }
            catch (DoctorDeletionException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Admin ,Patient")]

        [HttpGet("GetDoctorsBySpecialization")]
        public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
        {
            try
            {
                // Call the doctor service to get doctors by specialization
                var doctors = await _doctorService.GetDoctorsBySpecialization(specialization);
                if (doctors != null)
                {
                    return Ok(new { Success = true, Message = "Doctors retrieved successfully", Data = doctors });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "No doctors found for the specified specialization." });
                }
            }
            catch (DoctorRetrievalException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPatch]
        public async Task<IActionResult> UpdateSchedule(int doctorId, [FromBody] string newSchedule)
        {
            try
            {
                var role = User.FindFirstValue(ClaimTypes.Role);

                // Check if the user is authorized to update Schedule
                if (!string.Equals(role, "Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
                var success = await _doctorService.UpdateSchedule(doctorId, newSchedule);
                if (success)
                {
                    return Ok(new { Success = true, Message = "Doctor's schedule updated successfully." });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Doctor not found or schedule not updated." });
                }
            }
            catch (DoctorUpdateException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Doctor, Admin, Patient")]
        [HttpGet("GetPatientDetails")]
        public async Task<IActionResult> GetPatientDetails(int userId)
        {
            try
            {
                // Retrieve the JWT token from the authorization header
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                string[] parts = authorizationHeader.Split(' ');
                string token = parts[1];

                // Set the JWT token in the authorization header of the HttpClient
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Make a GET request to the GetPatientDetails endpoint
                var response = await _httpClient.GetAsync($"https://localhost:7283/api/PatientManagementService/GetPatientDetailss?UserID={userId}");

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var content = await response.Content.ReadAsStringAsync();

                    // Parse the JSON string in the 'data' field
                    var parsedData = JsonSerializer.Deserialize<ResponseModel<dynamic>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Extract the inner user and patient data
                    var userData = parsedData.Data.UserData;
                    var patientData = parsedData.Data.PatientData;

                    // Format the response with the extracted data
                    return Ok(new
                    {
                        Success = true,
                        Message = "User and Patient details retrieved successfully",
                        Data = new
                        {
                            UserData = new
                            {
                                FirstName = userData.FirstName,
                                LastName = userData.LastName,
                                Email = userData.Email,
                                Role = userData.Role
                            },
                            PatientData = new
                            {
                                PatientID = patientData.PatientID,
                                MedicalHistory = patientData.MedicalHistory,
                                Insurance = patientData.Insurance,
                                Gender = patientData.Gender,
                                DOB = patientData.DOB
                            }
                        }
                    });
                }
                else
                {
                    // Handle unsuccessful response
                    return StatusCode((int)response.StatusCode, new { Success = false, Message = "Failed to retrieve patient details" });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }


        [Authorize(Roles = "Doctor, Admin, Patient")]
        [HttpGet("GetPatientById")]
        public async Task<IActionResult> GetPatientById(int userId)
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                string[] parts = authorizationHeader.Split(' ');
                string token = parts[1];

                // Set the JWT token in the authorization header of the HttpClient
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Make a GET request to the GetPatientById endpoint
                var response = await _httpClient.GetAsync($"https://localhost:7283/api/PatientManagementService/GetPatientById?userId={userId}");

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var content = await response.Content.ReadAsStringAsync();

                    // Handle the response accordingly
                    return Ok(content);
                }
                else
                {
                    // Handle unsuccessful response
                    return StatusCode((int)response.StatusCode, new { Success = false, Message = "Failed to retrieve patient details" });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }





        /*
        [Authorize(Roles = "Doctor ,Admin,Patient")]
        [HttpGet("GetPatientDetails")]
        public async Task<OkObjectResult> GetPatientDetails(int userId)
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                string[] parts = authorizationHeader.Split(' ');


                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", parts[1]);
                // Make a GET request to the GetPatientDetails endpoint
                var response = await _httpClient.GetAsync($"https://localhost:7283/api/PatientManagementService/GetPatientDetailss?UserID={userId}");


                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var content = await response.Content.ReadAsStringAsync();
                    // Handle the response accordingly
                    return Ok(new { Success = true, Message = "Doctors retrieved successfully", Data = content });

                    Console.WriteLine(content);
                }
                else
                {
                    // Handle unsuccessful response
                    Console.WriteLine("Failed to retrieve patient details");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        */

    }
}
