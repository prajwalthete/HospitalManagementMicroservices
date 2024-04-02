using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagementService.Dto;
using PatientManagementService.Entity;
using PatientManagementService.GlobleExceptionhandler;
using PatientManagementService.Interface;
using System.Security.Claims;

namespace PatientManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientManagementServiceController : ControllerBase
    {
        private readonly IPatient _patient;

        public PatientManagementServiceController(IPatient patient)
        {
            _patient = patient;
        }



        [HttpPost("UpdatePatientDetails")]
        [Authorize(Roles = "Admin, Doctor,Patient")]
        public async Task<IActionResult> UpdatePatientDetails(PatientEntity patientEntity)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userId = Convert.ToInt32(userIdClaim);

                var patientResponse = await _patient.CreatePatientDetails(patientEntity, userId);

                var response = new ResponseModel<PatientResponseModel>
                {
                    Success = true,
                    Message = "Patient created successfully",
                    Data = patientResponse
                };

                return Ok(response);
            }
            catch (PatientCreationException ex)
            {
                var response = new ResponseModel<PatientResponseModel>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<PatientResponseModel>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Data = null
                };
                return StatusCode(500, response);
            }
        }

        [Authorize(Roles = "Admin, Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetPatientById()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userId = Convert.ToInt32(userIdClaim);
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
    }
}

