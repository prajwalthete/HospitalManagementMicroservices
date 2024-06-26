﻿using UserManagementService.Dto;

namespace UserManagementService.Interface
{
    public interface IUser
    {
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel);
        public Task<string> UserLogin(UserLoginModel userLogin);
        public Task<string> ForgetPassword(ForgetPasswordModel forgetPasswordModel);
        public Task<bool> ResetPassword(string NewPassword, int UserId);
        public Task<bool> UpdateProfile(UserRegistrationModel userRegistrationModel, int UserId);
        public Task<UserRegistrationModel> GetUserById(int userId);

    }
}
