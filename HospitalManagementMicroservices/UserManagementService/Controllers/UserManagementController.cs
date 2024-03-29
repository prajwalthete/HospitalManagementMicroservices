using Microsoft.AspNetCore.Mvc;
using UserManagementService.Dto;
using UserManagementService.GlobleExceptionhandler;
using UserManagementService.Interface;

namespace UserManagementService.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IUser _user;

        public UserManagementController(IUser user)
        {
            _user = user;
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegistration(UserRegistrationModel user)
        {
            try
            {
                var addedUser = await _user.RegisterUser(user);
                if (addedUser)
                {
                    var response = new ResponseModel<UserRegistrationModel>
                    {

                        Message = "User Registration Successful",

                    };
                    return Ok(response);
                }
                else
                {

                    return BadRequest("invalid input");
                }
            }
            catch (Exception ex)
            {
                if (ex is DuplicateEmailException)
                {
                    var response = new ResponseModel<UserRegistrationModel>
                    {

                        Success = false,
                        Message = ex.Message
                    };
                    return BadRequest(response);
                    // return BadRequest("User with given email already exists");

                }
                else if (ex is InvalidEmailFormatException)
                {
                    var response = new ResponseModel<UserRegistrationModel>
                    {

                        Success = false,
                        Message = ex.Message
                    };
                    return BadRequest(response);

                }
                else
                {
                    return StatusCode(500, $"An error occurred while adding the user: {ex.Message}");
                }
            }
        }


    }
}
