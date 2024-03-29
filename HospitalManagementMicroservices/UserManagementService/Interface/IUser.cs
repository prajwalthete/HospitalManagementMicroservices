using UserManagementService.Dto;

namespace UserManagementService.Interface
{
    public interface IUser
    {
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel);


    }
}
