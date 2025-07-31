using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models
{
    public class User_Role
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
