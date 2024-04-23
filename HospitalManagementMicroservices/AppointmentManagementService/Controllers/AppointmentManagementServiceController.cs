using AppointmentManagementService.Entity;
using AppointmentManagementService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AppointmentManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointment _appointmentService;
        private readonly HttpClient _httpClient;

        public AppointmentController(IAppointment appointmentService, HttpClient httpClient)
        {
            _appointmentService = appointmentService;
            _httpClient = httpClient;
        }


        [Authorize(Roles = "Patient")]
        [HttpPost("BookAppointment")]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentEntity appointment, int DoctorID)
        {
            try
            {
                // Extract PatientID from the token
                var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int patientId = Convert.ToInt32(patientIdClaim);

                // Call the appointment service to add the appointment
                var addedAppointment = await _appointmentService.AddAppointment(appointment, patientId, DoctorID);

                // Return the added appointment details
                return Ok(new { Success = true, Message = "Appointment added successfully", Data = addedAppointment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            try
            {
                // Get the current user's role
                var role = User.FindFirstValue(ClaimTypes.Role);

                // If the user is not a patient or doctor, deny access
                if (!string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(role, "Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                // Get the user ID
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int UserID = Convert.ToInt32(userID);

                // Call the appointment service to delete the appointment
                var isDeleted = await _appointmentService.DeleteAppointment(appointmentId, UserID);

                // Return success message if appointment is deleted
                if (isDeleted)
                {
                    return Ok(new { Success = true, Message = "Appointment deleted successfully." });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Appointment not found or you don't have permission to delete it." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

        [HttpGet("GetByDoctor")]
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctor(doctorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving appointments by doctor: {ex.Message}");
            }
        }

        [HttpGet("GetByPatient")]
        public async Task<IActionResult> GetAppointmentsByPatient()
        {
            try
            {
                // Extract patient ID from the token
                var patientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var appointments = await _appointmentService.GetAppointmentsByPatient(patientId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving appointments by patient: {ex.Message}");
            }
        }

        [HttpPatch("UpdateAppointmentDate")]
        public async Task<IActionResult> UpdateAppointmentDate(int appointmentId, [FromBody] DateTime newDate)
        {
            try
            {
                var success = await _appointmentService.UpdateAppointmentDate(appointmentId, newDate);
                if (success)
                {
                    return Ok("Appointment date updated successfully.");
                }
                else
                {
                    return BadRequest("Failed to update appointment date.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating appointment date: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin ,Patient")]
        [HttpGet("GetDoctorsBySpecialization")]
        public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                string[] parts = authorizationHeader.Split(' ');


                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", parts[1]);
                // Make a GET request to the GetDoctorsBySpecialization endpoint
                var response = await _httpClient.GetAsync($"https://localhost:7244/api/Doctor/GetDoctorsBySpecialization?specialization={specialization}");

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var content = await response.Content.ReadAsStringAsync();
                    // Return the response
                    return Ok(content);
                }
                else
                {
                    // Handle unsuccessful response
                    return BadRequest("Failed to retrieve doctors by specialization");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
