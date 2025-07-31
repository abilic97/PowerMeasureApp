using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerMeasure.Services.Interfaces
{
    public interface IUserInterface
    {
        public Task<IEnumerable<Users>> getUsers();
        public Task<Users> addUser(AddUserRequest user);
        public Task<int> getUsersCount();
        public Task<int> getAdminCount();
        public Task<Users> authenticateUser(string email, string password);

    }
}
