using ClothingStore.DTOs;
using ClothingStore.Model;
using ClothingStore.Repositories;
using ClothingStore.Repositories.Exeptions;
using ClothingStore.Services.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace ClothingStore.Services // Бизнес-логика
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher; // Захеширует (зашифрует) пароль.
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository repository, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<User>();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateUserAsync(RegistrationDTO registrationDTO)
        {
            var _user1 = await _repository.SelectUserByPhoneNumberAsync(registrationDTO.PhoneNumber);
            if (_user1 != null)
            {
                throw new PhoneNumberExistsException(nameof(registrationDTO));
            }
            if (!string.IsNullOrWhiteSpace(registrationDTO.Email))
            {
                var _user2 = await _repository.SelectUserByEmailAsync(registrationDTO.Email);
                if (_user2 != null)
                {
                    throw new EmailExistsException(nameof(registrationDTO));
                }
            }
            var user = new User
            {
                Name = registrationDTO.Name,
                PhoneNumber = registrationDTO.PhoneNumber,
                Email = registrationDTO.Email,
                Password = _passwordHasher.HashPassword(_user1, registrationDTO.Password),
                Role = "user"
            };
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                await _repository.AddUserAsync(user);
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task CreateAdminAsync(RegistrationDTO registrationDTO)
        {
            var _user1 = await _repository.SelectUserByPhoneNumberAsync(registrationDTO.PhoneNumber);
            if (_user1 != null)
            {
                throw new PhoneNumberExistsException(nameof(registrationDTO));
            }
            if (!string.IsNullOrWhiteSpace(registrationDTO.Email))
            {
                var _user2 = await _repository.SelectUserByEmailAsync(registrationDTO.Email);
                if (_user2 != null)
                {
                    throw new EmailExistsException(nameof(registrationDTO));
                }
            }
            var user = new User
            {
                Name = registrationDTO.Name,
                PhoneNumber = registrationDTO.PhoneNumber,
                Email = registrationDTO.Email,
                Password = _passwordHasher.HashPassword(_user1, registrationDTO.Password),
                Role = "admin"
            };
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                await _repository.AddUserAsync(user);
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task<User> GetByPhoneAsync(string phoneNumber)
        {
            return await _repository.SelectUserByPhoneNumberAsync(phoneNumber);
        }
        public async Task<User> GetByIdAsync(int userId)
        {
            return await _repository.SelectUserByIdAsync(userId);
        }
        public bool CheckPasswordAsync(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
        public async Task<JwtTokenDTO> AuthenticationAsync(SignInDTO user)
        {
            var _user = await GetByPhoneAsync(user.PhoneNumber);
            if (_user == null)
            {
                throw new UserNotFoundException(user.PhoneNumber);
            }
            var isCorrectedPassword = CheckPasswordAsync(_user, user.Password);
            if (!isCorrectedPassword)
            {
                throw new InvalidPasswordException();
            }
            var jwtToken = new JwtTokenDTO
            {
                AccessToken = _tokenService.GenerateAccessToken(_user, user.DeviceId),
                RefreshToken = _tokenService.GenerateRefreshToken(_user, user.DeviceId)
            };
            await _tokenService.CreateTokenAsync(jwtToken.RefreshToken, _user.Id, user.DeviceId);
            return jwtToken; // Возможно, нужна обработка ошибок.
        }
        public async Task LogoutAsync()
        {
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            var deviceId = _httpContextAccessor.HttpContext?.User.FindFirst("DeviceId")?.Value;
            try
            {
                await _tokenService.InvalidationTokenAsync(user.Id, deviceId);
            }
            catch (LogoutFaledException)
            {
                throw new LogoutFaledException();
            }
        }
        public async Task ChangeNameAsync(ChangeNameDTO changeNameDTO)
        {
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                throw new UserNotFoundException(nameof(changeNameDTO));
            }
            if(user.Name == changeNameDTO.Name)
            {
                throw new ChangeToSameDataException();
            }
            
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                user.Name = changeNameDTO.Name;
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task ChangePhoneNumberAsync(ChangePhoneNumberDTO changePhoneNumberDTO)
        {
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                throw new UserNotFoundException(nameof(changePhoneNumberDTO));
            }
            if (user.PhoneNumber == changePhoneNumberDTO.PhoneNumberNew)
            {
                throw new ChangeToSameDataException();
            }

            var transation = await _repository.BeginTransactionAsync();
            try
            {
                user.PhoneNumber = changePhoneNumberDTO.PhoneNumberNew;
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task ChangeEmailAsync(ChangeEmailDTO changeEmailDTO)
        {
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                throw new UserNotFoundException(nameof(changeEmailDTO));
            }
            if (user.Email == changeEmailDTO.Email)
            {
                throw new ChangeToSameDataException();
            }

            var transation = await _repository.BeginTransactionAsync();
            try
            {
                user.Email = changeEmailDTO.Email;
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                throw new UserNotFoundException(nameof(changePasswordDTO));
            }
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, changePasswordDTO.Password) == PasswordVerificationResult.Success)
            {
                throw new ChangeToSameDataException();
            }

            var transation = await _repository.BeginTransactionAsync();
            try
            {
                user.Password = _passwordHasher.HashPassword(user, changePasswordDTO.Password);
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public string GetUserInfo()
        {
            var userPhoneNumber = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.MobilePhone)?.Value;

            return userPhoneNumber;
        }
        public async Task<JwtTokenDTO> GetJwtTokenAsync(string refreshToken, string deviceId)
        {
            Token? token = await _tokenService.GetJwtTokenAsync(refreshToken, deviceId);
            if (token != null)
            {
                var user = await _repository.SelectUserByIdAsync(token.UserId);
                await _tokenService.InvalidationTokenAsync(user.Id, deviceId);
                string accessToken = _tokenService.GenerateAccessToken(user, deviceId);
                string newRefreshToken = _tokenService.GenerateRefreshToken(user, deviceId);
                await _tokenService.CreateTokenAsync(newRefreshToken, user.Id, deviceId);
                return new JwtTokenDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken,
                };
            }
            else
            {
                throw new TokenExistsFaledException();
            }
        }
    }
}
