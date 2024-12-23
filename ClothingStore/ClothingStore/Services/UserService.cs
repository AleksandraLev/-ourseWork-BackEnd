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
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher; // Захеширует (зашифрует) пароль.
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(ILogger<UserService> logger, IUserRepository repository, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _repository = repository;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<User>();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateUserAsync(RegistrationDTO registrationDTO)
        {
            _logger.LogInformation("Создаём пользователя.");
            var _user1 = await _repository.SelectUserByPhoneNumberAsync(registrationDTO.PhoneNumber);
            if (_user1 != null)
            {
                _logger.LogWarning($"Номер телефона \"{registrationDTO.PhoneNumber}\" уже используется.");
                throw new PhoneNumberExistsException(nameof(registrationDTO));
            }
            if (!string.IsNullOrWhiteSpace(registrationDTO.Email))
            {
                var _user2 = await _repository.SelectUserByEmailAsync(registrationDTO.Email);
                if (_user2 != null)
                {
                    _logger.LogWarning($"Почта \"{registrationDTO.Email}\" уже используется.");
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
                _logger.LogInformation("Сохраняем нового пользователя в базе данных.");
                await _repository.AddUserAsync(user);
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении нового пользователя в базе данных.");
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
            _logger.LogInformation($"Пользователь создан. ID: \"{user.Id}\"; Номер телефона: \"{user.PhoneNumber}\"; Почта: \"{user.Email}\"");
        }
        public async Task CreateAdminAsync(RegistrationDTO registrationDTO)
        {
            _logger.LogInformation("Создаём пользователя с правами администратора.");
            var _user1 = await _repository.SelectUserByPhoneNumberAsync(registrationDTO.PhoneNumber);
            if (_user1 != null)
            {
                _logger.LogWarning($"Номер телефона \"{registrationDTO.PhoneNumber}\" уже используется.");
                throw new PhoneNumberExistsException(nameof(registrationDTO));
            }
            if (!string.IsNullOrWhiteSpace(registrationDTO.Email))
            {
                var _user2 = await _repository.SelectUserByEmailAsync(registrationDTO.Email);
                if (_user2 != null)
                {
                    _logger.LogWarning($"Почта \"{registrationDTO.Email}\" уже используется.");
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
                _logger.LogInformation("Сохраняем нового пользователя в базе данных.");
                await _repository.AddUserAsync(user);
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении нового пользователя в базе данных.");
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
            _logger.LogInformation($"Пользователь создан. ID: \"{user.Id}\"; Номер телефона: \"{user.PhoneNumber}\"; Почта: \"{user.Email}\"");
        }
        public async Task<User> GetByPhoneAsync(string phoneNumber)
        {
            _logger.LogInformation($"Ищем пользователя с номером телефона: {phoneNumber}.");
            return await _repository.SelectUserByPhoneNumberAsync(phoneNumber);
        }
        public async Task<User> GetByIdAsync(int userId)
        {
            _logger.LogInformation($"Ищем пользователя с номером ID: {userId}.");
            return await _repository.SelectUserByIdAsync(userId);
        }
        public bool CheckPasswordAsync(User user, string password)
        {
            _logger.LogInformation($"Проверка пароля.");
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
        public async Task<JwtTokenDTO> AuthenticationAsync(SignInDTO user)
        {
            _logger.LogInformation("Авторизация...");
            var _user = await GetByPhoneAsync(user.PhoneNumber);
            if (_user == null)
            {
                _logger.LogWarning($"Пользователь с номером телефона \"{user.PhoneNumber}\" не найден.");
                throw new UserNotFoundException(user.PhoneNumber);
            }
            var isCorrectedPassword = CheckPasswordAsync(_user, user.Password);
            if (!isCorrectedPassword)
            {
                _logger.LogWarning($"Пароль \"{user.Password}\" не верный.");
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
            _logger.LogInformation("Выход из аккаунта.");
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            var deviceId = _httpContextAccessor.HttpContext?.User.FindFirst("DeviceId")?.Value;
            try
            {
                await _tokenService.InvalidationTokenAsync(user.Id, deviceId);
            }
            catch (LogoutFaledException)
            {
                _logger.LogError("Ошибка при выходе из аккаунта.");
                throw new LogoutFaledException();
            }
        }
        public async Task ChangeNameAsync(ChangeNameDTO changeNameDTO)
        {
            _logger.LogInformation("Меняем имя пользователя...");
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден.");
                throw new UserNotFoundException(nameof(changeNameDTO));
            }
            if(user.Name == changeNameDTO.Name)
            {
                _logger.LogWarning("Попытка изменить имя на то же самое.");
                throw new ChangeToSameDataException();
            }
            
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation($"Сохраняем информацию о сменене имени пользователя на {changeNameDTO.Name}; (ID пользователя: {user.Id}.");
                user.Name = changeNameDTO.Name;
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении информации в базу данных.");
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task ChangePhoneNumberAsync(ChangePhoneNumberDTO changePhoneNumberDTO)
        {
            _logger.LogInformation("Меняем номер телефона пользователя...");
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден.");
                throw new UserNotFoundException(nameof(changePhoneNumberDTO));
            }
            if (user.PhoneNumber == changePhoneNumberDTO.PhoneNumberNew)
            {
                _logger.LogWarning("Попытка изменить номер телефона на тот же самый.");
                throw new ChangeToSameDataException();
            }

            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation($"Сохраняем информацию о сменене номера телефона пользователя на {changePhoneNumberDTO.PhoneNumberNew}; (ID пользователя: {user.Id}.");
                user.PhoneNumber = changePhoneNumberDTO.PhoneNumberNew;
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении информации в базу данных.");
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task ChangeEmailAsync(ChangeEmailDTO changeEmailDTO)
        {
            _logger.LogInformation("Меняем почту пользователя...");
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден.");
                throw new UserNotFoundException(nameof(changeEmailDTO));
            }
            if (user.Email == changeEmailDTO.Email)
            {
                _logger.LogWarning("Попытка изменить почту на ту же самую.");
                throw new ChangeToSameDataException();
            }

            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation($"Сохраняем информацию о сменене почты пользователя на {changeEmailDTO.Email}; (ID пользователя: {user.Id}.");
                user.Email = changeEmailDTO.Email;
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении информации в базу данных.");
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public async Task ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            _logger.LogInformation("Меняем пароль пользователя...");
            var user = await _repository.SelectUserByPhoneNumberAsync(GetUserInfo());
            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден.");
                throw new UserNotFoundException(nameof(changePasswordDTO));
            }
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, changePasswordDTO.Password) == PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Попытка изменить пароль на тот же самый.");
                throw new ChangeToSameDataException();
            }

            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation($"Сохраняем информацию о сменене пароля пользователя; (ID пользователя: {user.Id}.");
                user.Password = _passwordHasher.HashPassword(user, changePasswordDTO.Password);
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (UserSavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении информации в базу данных.");
                await transation.RollbackAsync();
                throw new UserSavedFaledException();
            }
        }
        public string GetUserInfo()
        {
            _logger.LogInformation("Получаем номер телефона пользователя из контекста.");

            var userPhoneNumber = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.MobilePhone)?.Value;

            return userPhoneNumber;
        }
        public async Task<JwtTokenDTO> GetJwtTokenAsync(string refreshToken, string deviceId)
        {
            Token? token = await _tokenService.GetJwtTokenAsync(refreshToken, deviceId);
            if (token != null)
            {
                _logger.LogInformation("Получаем JwtToken.");
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
                _logger.LogError("Ошибка при полученнии JwtToken.");
                throw new TokenExistsFaledException();
            }
        }
    }
}
