using Business_Logic_Layer.Services.IServices;
using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateOrUpdateUserAsync(string userId, string userName, string displayName, string email, string profilePictureUrl)
    {
        // Attempt to find the user in the database by the Auth0 user id
        var existingUser = await _unitOfWork.Repository<ApplicationUser>()
                                .GetByCondition(u => u.Id == userId)
                                .FirstOrDefaultAsync();

        if (existingUser == null)
        {
            // Create a new ApplicationUser entity
            var newUser = new ApplicationUser
            {
                Id = userId,
                UserName = userName,
                DisplayName = displayName,
                Email = email,
                ProfilePictureUrl = profilePictureUrl
            };

            _unitOfWork.Repository<ApplicationUser>().Create(newUser);
        }
        else
        {
            // Update the existing record if necessary
            existingUser.UserName = userName;
            existingUser.DisplayName = displayName;
            existingUser.Email = email;
            existingUser.ProfilePictureUrl = profilePictureUrl;

            _unitOfWork.Repository<ApplicationUser>().Update(existingUser);
        }

        await _unitOfWork.Repository<ApplicationUser>().SaveChangesAsync();
    }
}
