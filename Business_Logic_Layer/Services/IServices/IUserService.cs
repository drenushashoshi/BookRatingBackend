using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services.IServices
{
    public interface IUserService
    {
        Task CreateOrUpdateUserAsync(string userId, string userName, string displayName, string email, string profilePictureUrl);
    }
}

