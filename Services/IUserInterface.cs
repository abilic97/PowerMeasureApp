using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Services
{
    public interface IUserInterface
    {
        public Task<IEnumerable<Users>> getUsers();
        public Task<Users> getUser(int id);
        public Task<Users> addUser(AddUserRequest user);
        public Task<int> deleteUser();
        public Task<int> getUsersCount();
        public Task<int> getAdminCount();
        public Task updateUser(Users user);

        public Task<Users> authenticateUser(string email, string password);

    }
}
