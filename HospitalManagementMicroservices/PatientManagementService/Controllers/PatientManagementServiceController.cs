using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagementService.Dto;
using PatientManagementService.Entity;
using PatientManagementService.GlobleExceptionhandler;
using PatientManagementService.Interface;
using System.Text.Json;

namespace PatientManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientManagementServiceController : ControllerBase
    {
        private readonly IPatient _patient;
        private readonly HttpClient _httpClient;


        public PatientManagementServiceController(IPatient patient, HttpClient httpClient)
        {
            _patient = patient;
            _httpClient = httpClient;

        }


        [HttpPost("CreatePatient")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientEntity patientEntity, int UserID)
        {
            try
            {
                // Get the user ID from the claims
                //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //int userId = Convert.ToInt32(userIdClaim);

                // Call the service method to create the patient
                var createdPatient = await _patient.CreatePatientDetails(patientEntity, UserID);

                if (createdPatient != null)
                {
                    return Ok(new { Success = true, Message = "Patient created successfully", Data = createdPatient });
                }
                else
                {
                    return StatusCode(500, new { Success = false, Message = "Failed to create patient" });
                }
            }
            catch (PatientCreationException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }



        //[HttpPost("UpdatePatientDetails")]
        //[Authorize(Roles = "Admin, Doctor")]
        //public async Task<IActionResult> UpdatePatientDetails(int UserID, PatientEntity patientEntity)
        //{
        //    try
        //    {
        //        // var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        //int userId = Convert.ToInt32(userIdClaim);

        //        var patientResponse = await _patient.CreatePatientDetails(patientEntity, UserID);


        //        var response = new ResponseModel<PatientResponseModel>
        //        {
        //            Success = true,
        //            Message = "Patient created successfully",
        //            Data = patientResponse
        //        };


        //        return Ok(response);

        //    }
        //    catch (PatientCreationException ex)
        //    {
        //        var response = new ResponseModel<PatientResponseModel>
        //        {
        //            Success = false,
        //            Message = ex.Message,
        //            Data = null
        //        };
        //        return StatusCode(500, response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var response = new ResponseModel<PatientResponseModel>
        //        {
        //            Success = false,
        //            Message = "An unexpected error occurred",
        //            Data = null
        //        };
        //        return StatusCode(500, response);
        //    }
        //}

        [Authorize]
        [HttpGet("GetPatientById")]
        public async Task<IActionResult> GetPatientById(int userId)
        {
            try
            {
                //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //int userId = Convert.ToInt32(userIdClaim);

                var patient = await _patient.GetPatientById(userId);
                if (patient != null)
                {
                    return Ok(new { Success = true, Message = "Patient found.", Data = patient });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Patient not found." });
                }
            }
            catch (PatientRetrievalException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }


        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet("GetPatientDetailss")]
        public async Task<IActionResult> GetPatientDetails(int UserID)
        {
            try
            {
                // var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //int userId = Convert.ToInt32(userIdClaim);

                //if (userId == 0)
                //{
                //    // Handle the case where userId is not valid
                //    return BadRequest(new { Success = false, Message = "Invalid user ID." });
                //}

                // Call the service layer method to get patient details
                var patient = await _patient.GetPatientById(UserID);

                if (patient != null)
                {
                    // Proceed with retrieving user details and combining them with patient details
                    var userResponse = await _httpClient.GetAsync($"https://localhost:7081/api/UserManagement/GetUserById?UserId={UserID}");

                    if (userResponse.IsSuccessStatusCode)
                    {
                        var content = await userResponse.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<ResponseModel<UserResponseModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (responseObject.Success)
                        {
                            // Combine user and patient data into a single object
                            var combinedData = new
                            {
                                UserData = responseObject.Data,
                                PatientData = patient
                            };

                            return Ok(new { Success = true, Message = "User and Patient details retrieved successfully", Data = combinedData });
                        }
                        else
                        {
                            return BadRequest(new { Success = false, Message = responseObject.Message });
                        }
                    }
                    else
                    {
                        return StatusCode(500, new { Success = false, Message = "Failed to retrieve user details" });
                    }
                }
                else
                {
                    return StatusCode(500, new { Success = false, Message = "Failed to retrieve patient details" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }


    }
}