using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Application.Dtos.UserModel;
using ToDoApp.Application.Helpers;
using ToDoApp.Domains.AppSettingsConfigurations;
using ToDoApp.DataAccess.Entities;
using ToDoApp.Infrastructures;
using ToDoApp.Constant;

namespace ToDoApp.Application.Services
{
    public interface IUserService
    {
        int Register(UserRegisterModel user);
        User? Login(UserLoginModel user);
        User GetOrCreateUserFromGoogle(GoogleJsonWebSignature.Payload payload);
        string GenerateJwt(User user);
        string GenerateRefreshToken(int userId);
        User GetUserByRefreshToken(string refreshToken);
        void DeleteOldRefreshToken(int userId);
    }

    public class UserService : IUserService
    {
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public UserService(IApplicationDBContext context,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettingsOptions)
        {
            _context = context;
            _mapper = mapper;
            _jwtSettings = jwtSettingsOptions.Value;
        }

        public User GetUserByRefreshToken(string refreshToken)
        {
            var user = _context.RefreshTokens
                .Where(x => x.Token == refreshToken
                    && !x.IsRevoked 
                    && x.ExpireTime > DateTime.Now)
                .Select(x => x.User)
                .FirstOrDefault();

            return user;
        }

        public string GenerateRefreshToken(int userId)
        {
            string refreshTokens = HashHelper.GenerateRandomString(64);

            string hashedRefreshToken = HashHelper.Hash256(refreshTokens);

            var data = new RefreshToken
            {
                UserId = userId,
                Token = hashedRefreshToken,
                ExpireTime = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(data);
            _context.SaveChanges(); 

            return hashedRefreshToken;
        }

        public void DeleteOldRefreshToken(int userId)
        {
            var entities = _context.RefreshTokens
                .Where(x => x.UserId == userId)
                .ToList();

            if (entities == null) return;

            _context.RefreshTokens.RemoveRange(entities);
            _context.SaveChanges();
        }

        public string GenerateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Email, user.EmailAddress),
                new (ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256Signature
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public User GetOrCreateUserFromGoogle(GoogleJsonWebSignature.Payload payload)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress.Equals(payload.Email));

            if (user == null)
            {
                user = _mapper.Map<User>(payload);
                user.UserName = "";
                user.Password = "";
                user.Salting = "";
                user.Role = Role.User;
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            return user;
        }

        // Kỹ thuật Salting: một str random 100 chars
        public int Register(UserRegisterModel registerModel)
        {
            if (registerModel == null)
            {
                //throw new Exception("Model is null");
                return -1;
            }

            var users = _context.Users.AsNoTracking();

            if (users.Any(x => x.UserName.Equals(registerModel.UserName))
                || users.Any(x => x.EmailAddress.Equals(registerModel.EmailAddress)))
            {
                //throw new Exception("Username or email address are existed");
                return -1;
            }

            var newUser = _mapper.Map<User>(registerModel);

            var salting = HashHelper.GenerateRandomString(100);
            var password = newUser.Password + salting;

            newUser.Password = HashHelper.BCryptHash(password);
            newUser.Salting = salting;

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser.Id;
        }

        public User? Login(UserLoginModel loginModel)
        {
            if (loginModel == null)
            {
                return null;
            }

            var user = _context.Users
                .FirstOrDefault(x =>
                    x.UserName.Equals(loginModel.UserNameOrEmail)
                    || x.EmailAddress.Equals(loginModel.UserNameOrEmail)
                );

            if (user == null)
            {
                return null;
            }

            var password = loginModel.Password + user.Salting;

            if (!HashHelper.BCryptVerify(password, user.Password))
            {
                return null;
            }

            return user;
        }
    }
}
