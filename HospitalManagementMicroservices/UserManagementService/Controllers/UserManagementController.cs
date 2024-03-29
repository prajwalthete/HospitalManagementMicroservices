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



        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(UserLoginModel userLogin)
        {
            try
            {
                // Authenticate the user and generate JWT token
                var result = await _user.UserLogin(userLogin);

                var response = new ResponseModel<UserLoginModel>
                {

                    Message = "Login Sucessfull",

                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                if (ex is UserNotFoundException)
                {
                    var response = new ResponseModel<UserLoginModel>
                    {

                        Success = false,
                        Message = ex.Message

                    };
                    return Conflict(response);

                }
                else if (ex is InvalidPasswordException)
                {
                    var response = new ResponseModel<UserLoginModel>
                    {

                        Success = false,
                        Message = ex.Message

                    };
                    return BadRequest(response);
                }
                else
                {
                    return StatusCode(500, $"An error occurred while processing the login request: {ex.Message}");

                }
            }

        }




    }
}
