using RhyionSystemsTask3.BusinessLogicLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Models;
using RhyionSystemTask3.DataAccessLayer.UnitOfWork;

namespace RhyionSystemsTask3.BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository , IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> RegisterNewUserAsync(string firstName, string lastName, string email, string passwordHash)
        {
            var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {email} already exists.");
            }

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,    
                Email = email,
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.CommitAsync();

            return newUser.UserId;
        }

        public async Task<User> GetUserProfileAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, string newEmail)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var existingUser = await _userRepository.GetUserByEmailAsync(newEmail);
            if (existingUser != null && existingUser.UserId != userId)
            {
                throw new InvalidOperationException($"Email {newEmail} is already taken.");
            }

            user.Email = newEmail;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
