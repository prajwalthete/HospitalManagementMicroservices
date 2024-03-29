using Dapper;
using System.Data;
using System.Text.RegularExpressions;
using UserManagementService.Context;
using UserManagementService.Dto;
using UserManagementService.Entity;
using UserManagementService.GlobleExceptionhandler;
using UserManagementService.Interface;

namespace UserManagementService.Service;

public class UserService : IUser
{
    private readonly UserManagementServiceContext _context;
    private readonly IAuthService _authService;
    private readonly IEmail _emailService;


    public UserService(UserManagementServiceContext context, IAuthService authService, IEmail emailService)
    {
        _context = context;
        _authService = authService;
        _emailService = emailService;
    }



    public async Task<bool> RegisterUser(UserRegistrationModel userRegModel)
    {
        var parametersToCheckEmailIsValid = new DynamicParameters();
        parametersToCheckEmailIsValid.Add("Email", userRegModel.Email, DbType.String);

        var querytoCheckEmailIsNotDuplicate = @"
                SELECT COUNT(*)
                FROM Users
                WHERE Email = @Email;
            ";

        var query = @"
                INSERT INTO Users (FirstName, LastName, Email, Password,Role)
                VALUES (@FirstName, @LastName, @Email, @Password,@Role);
            ";

        var parameters = new DynamicParameters();
        parameters.Add("FirstName", userRegModel.FirstName, DbType.String);
        parameters.Add("LastName", userRegModel.LastName, DbType.String);

        //Check Emailformat Using Regex
        if (!IsValidEmail(userRegModel.Email))
        {
            throw new InvalidEmailFormatException("Invalid email format");
        }

        parameters.Add("Email", userRegModel.Email, DbType.String);

        //convert Plain Password into cryptographic String 
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegModel.Password);
        parameters.Add("Password", hashedPassword, DbType.String);

        parameters.Add("Role", userRegModel.Role, DbType.String);

        using (var connection = _context.CreateConnection())
        {
            // Check if table exists
            bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>(
                @"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'Users';
                ");

            // Create table if it doesn't exist
            if (!tableExists)
            {
                await connection.ExecuteAsync(@"
                       CREATE TABLE Users (
                                         UserID INT IDENTITY(1, 1) PRIMARY KEY,
                                         FirstName NVARCHAR(50) NOT NULL,
                                         LastName NVARCHAR(50) NOT NULL,
                                         Email NVARCHAR(100) UNIQUE NOT NULL,
                                         Password NVARCHAR(100) NOT NULL,
                                         Role NVARCHAR(50) CHECK (Role IN ('Admin', 'Doctor', 'Patient')) NOT NULL,
                                         IsApproved BIT DEFAULT 0 NOT NULL
                                            );

                    ");
            }

            // Check if email already exists
            bool emailExists = await connection.QueryFirstOrDefaultAsync<bool>(querytoCheckEmailIsNotDuplicate, parametersToCheckEmailIsValid);
            if (emailExists)
            {
                throw new DuplicateEmailException("Email address is already in use");
            }

            // Insert new user
            await connection.ExecuteAsync(query, parameters);
        }

        return true;
    }

    public async Task<string> UserLogin(UserLoginModel userLogin)
    {
        using (var connection = _context.CreateConnection())
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", userLogin.Email);


            string query = @"
                        SELECT * FROM Users WHERE Email = @Email ;
                       ";


            var user = await connection.QueryFirstOrDefaultAsync<UserEntity>(query, parameters);

            if (user == null)
            {
                throw new UserNotFoundException($"User with email '{userLogin.Email}' not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                throw new InvalidPasswordException($"User with Password '{userLogin.Password}' not Found.");
            }
            //if password enterd from user and password in db match then generate Token 
            var token = _authService.GenerateJwtToken(user);
            return token;

        }
    }


    public async Task<string> ForgetPassword(ForgetPasswordModel forgetPasswordModel)
    {

        if (forgetPasswordModel == null)
        {
            throw new ArgumentNullException(nameof(forgetPasswordModel), "ForgetPasswordModel cannot be null.");
        }

        var parameters = new DynamicParameters();
        parameters.Add("Email", forgetPasswordModel.Email);


        string query = @"
                            SELECT *
                            FROM Users 
                            WHERE Email = @Email;
                            ";

        using (var connection = _context.CreateConnection())
        {
            var user = await connection.QueryFirstOrDefaultAsync<UserEntity>(query, parameters);

            if (user == null)
            {
                throw new UserNotFoundException($"User with email '{forgetPasswordModel.Email}' not found.");
            }
            //if password enterd from user and password in db match then generate Token 
            var _token = _authService.GenerateJwtToken(user);


            // Generate password reset link
            var resetPasswordUrl = $"https://localhost:7081/api/User/ResetPassword?token={_token}";

            var emailBody = $@"
                            <html>
                                 <head>
                                         <style>
                                              .reset-link {{
                                              color: blue;
                                              font-weight: bold;
                                              }}
                                         </style>
                                 </head>
                            <body>
                                        <p>Hello,</p>
                                        <p>Please click on the following link to reset your password:</p>
                                         <p>
                                         <a href=""{resetPasswordUrl}"" class=""reset-link"">{resetPasswordUrl} </a>
                                         </p>
                                         <p>Thank you!</p>
                            </body>
                            </html>
                                   ";


            // Send email with HTML body
            await _emailService.SendEmailAsync(forgetPasswordModel.Email, "Reset Password", emailBody);

            return _token;

        }
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

}

