using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            var userById = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (userById == null) return null!;
            return new UserDto {
                UserId = userById.UserId,
                Name = userById.Name,
                Email = userById.Email,
                UserType = userById.UserType
            };
        }

        public UserDto Login(LoginDto login)
        {
            var userValid = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (userValid == null) return null!;
            return new UserDto {
                UserId = userValid.UserId,
                Name = userValid.Name,
                Email = userValid.Email,
                UserType = userValid.UserType
            };
        }
        
        public UserDto Add(UserDtoInsert user)
        {
            var newUser = new User {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = "client"
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return new UserDto {
                UserId = newUser.UserId,
                Name = newUser.Name,
                Email = newUser.Email,
                UserType = newUser.UserType
            };     
        }      

        public UserDto GetUserByEmail(string userEmail)
        {
            var userByEmail = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (userByEmail == null) return null!;
            return new UserDto {
                UserId = userByEmail.UserId,
                Name = userByEmail.Name,
                Email = userByEmail.Email,
                UserType = userByEmail.UserType
            };
        }         

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(u => new UserDto {
               UserId = u.UserId,
               Name = u.Name,
               Email = u.Email,
               UserType = u.UserType
           }).ToList();
        }

    }
}