using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<User_Role> Roles { get; set; }
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }
        public ICollection<UserContract> Contracts { get; set; }
        public ICollection<Bill> Bills { get; set; }
    }
}
