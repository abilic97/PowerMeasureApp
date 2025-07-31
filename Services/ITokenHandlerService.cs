using PowerMeasure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Services
{
    public interface ITokenHandlerService
    {
        Task<string> CreateTokenAsync(Users user);
    }
}
