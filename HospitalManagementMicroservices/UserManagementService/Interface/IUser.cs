using UserManagementService.Dto;

namespace UserManagementService.Interface
{
    public interface IUser
    {
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel);
        public Task<string> UserLogin(UserLoginModel userLogin);



    }
}
