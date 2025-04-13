using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace Business.Services;

public interface IUserService
{
    Task<UserResult> AddUserToRole(string userId, string roleName);
    Task<string> GetDisplayName(string userId);
    Task<UserResult<AppUser>> GetUserByIdAsync(string id);
    Task<UserResult<IEnumerable<AppUser>>> GetUsersAsync();

    Task<UserResult> CreateUserAsync(SignUpFormData formdata, string roleName = "User");
    Task<UserResult> CreateMemberAsync(AddUserFormData formdata);
}

public class UserService(UserManager<AppUserEntity> userManager, IAppUserRepository appUserRepository, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly UserManager<AppUserEntity> _userManager = userManager;
    private readonly IAppUserRepository _appUserRepository = appUserRepository;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<UserResult<IEnumerable<AppUser>>> GetUsersAsync()
    {
        var repositoryResult = await _appUserRepository.GetAllAsync
            (
                orderByDescending: false,
                sortBy: x => x.FirstName!
            );

        var entities = repositoryResult.Result;
        var users = entities?.Select(entity => entity.MapTo<AppUser>()) ?? [];

        return new UserResult<IEnumerable<AppUser>> { Succeeded = true, StatusCode = 200, Result = users };
    }

    public async Task<UserResult<AppUser>> GetUserByIdAsync(string id)
    {
        var repositoryResult = await _appUserRepository.GetAsync(x => x.Id == id);

        var entity = repositoryResult.Result;
        if (entity == null)
            return new UserResult<AppUser> { Succeeded = false, StatusCode = 404, Error = $"User with id '{id}' was not found." };

        var user = entity.MapTo<AppUser>();
        return new UserResult<AppUser> { Succeeded = true, StatusCode = 200, Result = user };
    }

    public async Task<UserResult> AddUserToRole(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            return new UserResult { Succeeded = false, StatusCode= 404, Error = "Role doesn't exist" };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "User doesn't exist" };

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded 
            ? new UserResult { Succeeded = true, StatusCode = 200 } 
            : new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to add user to role" };
    }

    public async Task<UserResult> CreateUserAsync(SignUpFormData formdata, string roleName = "User")
    {
        if (formdata == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "Invalid form data" };

        var existsResult = await _appUserRepository.ExistsAsync(x => x.Email == formdata.Email);
        if (existsResult.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = 409, Error = "User already exists" };

        try
        {
            var userEntity = formdata.MapTo<AppUserEntity>();
            userEntity.UserName = userEntity.Email;

            var result = await _userManager.CreateAsync(userEntity, formdata.Password);

            if (result.Succeeded)
            {
                var addToRoleResult = await AddUserToRole(userEntity.Id, roleName);
                return result.Succeeded
                    ? new UserResult { Succeeded = true, StatusCode = 201 }
                    : new UserResult { Succeeded = false, StatusCode = 201, Error = "User created but not added to role" };
            }

            return new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user" };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }

    }

    public async Task<UserResult> CreateMemberAsync(AddUserFormData formdata)
    {
        if (formdata == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "Invalid form data" };

        var existsResult = await _appUserRepository.ExistsAsync(x => x.Email == formdata.Email);
        if (existsResult.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = 409, Error = "User already exists" };

        try
        {
            var userEntity = formdata.MapTo<AppUserEntity>();
            userEntity.UserName = userEntity.Email;

            var result = await _userManager.CreateAsync(userEntity);

            return new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user" };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }

    }

    public async Task<string> GetDisplayName(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return "";

        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? "" : $"{user.FirstName} {user.LastName}";
    }

}
