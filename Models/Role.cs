using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public List<User_Role> Roles { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
