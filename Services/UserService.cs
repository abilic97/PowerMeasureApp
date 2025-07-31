using Microsoft.EntityFrameworkCore;
using PowerMeasure.Data;
using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using PowerMeasure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Services
{
    public class UserService : IUserInterface
    {
        private PowerMeasureDbContext _powerMeasureDbContext;
        public UserService(PowerMeasureDbContext context)
        {
            _powerMeasureDbContext = context;
        }
        public async Task<Users> addUser(AddUserRequest user)
        {
            Users newUser = new Users();
            newUser.FirstName = user.FirstName;
            newUser.LastName = user.LastName;
            newUser.EmailAddress = user.Email;
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            newUser.isActive = user.IsUserActive;
            newUser.createdAt = DateTime.Now;
            newUser.Roles = new List<User_Role>();
            newUser.Contracts = new List<UserContract>();
            await _powerMeasureDbContext.Users.AddAsync(newUser);

            EnergyMeter em = new EnergyMeter();
            em.EnergyMeterCode = user.EmCode;
            em.Active = user.EmActive;
            em.DateActiveFrom = user.EmContractValidFrom;
            em.DateActiveTo = user.EmContractValidTo;

            UserContract contract = new UserContract();
            contract.EnergyMeter = em;
            contract.ContractValidFrom = user.ContractValidFrom;
            contract.ContractValidTo = user.ContractValidTo;
            newUser.Contracts.Add(contract);

            var userrole = new User_Role
            {
                User = newUser,
                Role = _powerMeasureDbContext.Roles.Where(x => x.RoleName == user.RoleName).FirstOrDefault()
            };
            await _powerMeasureDbContext.User_Roles.AddAsync(userrole);


            await _powerMeasureDbContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<IEnumerable<Users>> getUsers()
        {
            var users = from u in _powerMeasureDbContext.Users
                        join ur in _powerMeasureDbContext.User_Roles on u.Id equals ur.UserId
                        join r in _powerMeasureDbContext.Roles on ur.RoleId equals r.Id
                        where r.RoleName == "user"
                        select u;
            return await users.ToListAsync();
        }

        public async Task<int> getUsersCount()
        {
            var users = from u in _powerMeasureDbContext.Users
                        join ur in _powerMeasureDbContext.User_Roles on u.Id equals ur.UserId
                        join r in _powerMeasureDbContext.Roles on ur.RoleId equals r.Id
                        where r.RoleName == "user"
                        select u;
            var count = users.Count();
            return count;
        }

        public async Task<int> getAdminCount()
        {
            var users = from u in _powerMeasureDbContext.Users
                        join ur in _powerMeasureDbContext.User_Roles on u.Id equals ur.UserId
                        join r in _powerMeasureDbContext.Roles on ur.RoleId equals r.Id
                        where r.RoleName == "admin"
                        select u;
            var count = users.Count();
            return count;
        }

        public async Task<Users> authenticateUser(string email, string password)
        {
            var user = await _powerMeasureDbContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == email);
            if (user == null) return null;
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (isValidPassword)
            {
                return user;
            }
            return null;
        }
    }
}
