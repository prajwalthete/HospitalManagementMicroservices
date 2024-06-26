﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementService.Dto;
using UserManagementService.GlobleExceptionhandler;
using UserManagementService.Interface;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                var Token = await _user.UserLogin(userLogin);

                var response = new ResponseModel<string>
                {

                    Message = "Login Sucessfull",
                    Data = Token

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



        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordModel forgetPasswordModel)
        {
            try
            {

                string Token = await _user.ForgetPassword(forgetPasswordModel);

                //HttpContext.Response.Headers.Add("Authorization", $"Bearer {Token}");

                if (Token != null)
                {
                    var response = new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Email sent successfully.",
                        // Data = Token

                    };
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Failed to send email.",
                        Data = null
                    };
                    return BadRequest(response);
                }
            }
            catch (UserNotFoundException ex)
            {
                var response = new ResponseModel<string>
                {

                    Success = false,
                    Message = $"Error sending email: {ex.Message}",
                    Data = null
                };
                return StatusCode(500, response);
            }
            catch (EmailSendingException ex)
            {
                var response = new ResponseModel<string>
                {

                    Success = false,
                    Message = $"Error sending email: {ex.Message}",
                    Data = null
                };
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}",
                    Data = null
                };
                return StatusCode(500, response);
            }
        }

        [Authorize]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int UserId = Convert.ToInt32(userIdClaim);
                bool isPassWordReset = await _user.ResetPassword(resetPasswordModel.NewPassword, UserId);

                var response = new ResponseModel<bool>
                {

                    Success = true,
                    Message = "Password reset successfully",
                    Data = isPassWordReset
                };

                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                var response = new ResponseModel<bool>
                {

                    Success = false,
                    Message = ex.Message,
                    Data = false
                };

                return NotFound(response);
            }
            catch (RepositoryException ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                };

                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                };

                return StatusCode(500, response);
            }
        }

        [Authorize]
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UserRegistrationModel userRegistrationModel)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int UserId = Convert.ToInt32(userIdClaim);
                var success = await _user.UpdateProfile(userRegistrationModel, UserId);

                if (success)
                {
                    return Ok(new { Success = true, Message = "User profile updated successfully." });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Failed to update user profile." });
                }
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }





        // [Authorize]
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(int UserId)
        {
            try
            {
                //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // int userId = Convert.ToInt32(userIdClaim);
                var user = await _user.GetUserById(UserId);

                if (user != null)
                {
                    var userResponse = new UserResponseModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Role = user.Role
                    };
                    return Ok(new { Success = true, Message = "User found.", Data = userResponse });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "User not found." });
                }
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (RepositoryException ex)
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