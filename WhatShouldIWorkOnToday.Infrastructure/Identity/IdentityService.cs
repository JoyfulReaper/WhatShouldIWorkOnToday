using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;
using WhatShouldIWorkOnToday.Domain.Common.Errors;

namespace WhatShouldIWorkOnToday.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _signInManager = signInManager;
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<ErrorOr<string>> LoginUserAsync(string username, string password)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(username);
        if(user is null)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
        if(result.Succeeded)
        {
            return user.Id;
        }

        return Errors.Authentication.InvalidCredentials;
    }

    public async Task<ErrorOr<string>> CreateUserAsync(string userName, string password, string email)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
        };

        var result = await _userManager.CreateAsync(user, password);
        if(!result.Succeeded)
        {
            if(result.Errors.Any(e => e.Code == "PasswordRequiresNonAlphanumeric"))
            {
                return Errors.Authentication.PasswordRequiresNonAlpha;
            }

            return Errors.Authentication.Unknown;
        }

        return user.Id;
    }

    public async Task<ErrorOr<bool>> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
        if (user is not null)
        {
            var result = await DeleteUserAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
        }

        return Errors.Authentication.Unknown;
    }

    public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);
        return result;
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);
        return user.UserName;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }
}
