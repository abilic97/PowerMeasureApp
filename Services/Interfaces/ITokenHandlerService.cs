using PowerMeasure.Models;
using System.Threading.Tasks;

namespace PowerMeasure.Services.Interfaces
{
    public interface ITokenHandlerService
    {
        Task<string> CreateTokenAsync(Users user);
    }
}
