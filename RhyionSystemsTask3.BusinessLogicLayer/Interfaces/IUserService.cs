using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemsTask3.BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<int> RegisterNewUserAsync(string firstName, string lastName, string email, string passwordHash);
        Task<User> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(int userId, string newEmail);
    }
}
