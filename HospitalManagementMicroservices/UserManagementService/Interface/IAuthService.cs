using UserManagementService.Entity;

namespace UserManagementService.Interface
{
    public interface IAuthService
    {
        public string GenerateJwtToken(UserEntity user);
    }
}
