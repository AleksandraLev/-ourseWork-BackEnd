using ClothingStore.DTOs;
using ClothingStore.Model;

namespace ClothingStore.Services // Бизнес-логика
{
    public interface IUserService
    {
        Task CreateUserAsync(RegistrationDTO registrationDTO);
        Task CreateAdminAsync(RegistrationDTO registrationDTO);
        Task<User> GetByPhoneAsync(string phoneNumber);
        Task<User> GetByIdAsync(int userId);
        bool CheckPasswordAsync(User user, string password);
        Task<JwtTokenDTO> AuthenticationAsync(SignInDTO user);
        Task LogoutAsync();
        Task ChangeNameAsync(ChangeNameDTO changeNameDTO);
        Task ChangePhoneNumberAsync(ChangePhoneNumberDTO changePhoneNumberDTO);
        Task ChangeEmailAsync(ChangeEmailDTO changeEmailDTO);
        Task ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);
        Task<JwtTokenDTO> GetJwtTokenAsync(string refreshToken, string deviceId);
    }
}
