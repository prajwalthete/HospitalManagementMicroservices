using DoctorManagementService.Entity;
using DoctorManagementService.GlobleExceptionhandler;
using DoctorManagementService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctor _doctorService;

        public DoctorController(IDoctor doctorService)
        {
            _doctorService = doctorService;
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
    }
}
