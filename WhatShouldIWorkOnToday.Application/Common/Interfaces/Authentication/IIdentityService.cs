using ErrorOr;
using WhatShouldIWorkOnToday.Application.Common.Models;

namespace WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;

public interface IIdentityService
{
    Task<string> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<ErrorOr<string>> CreateUserAsync(string firstName, string lastName, string userName, string password, string email);

    Task<ErrorOr<bool>> DeleteUserAsync(string userId);
    Task<ErrorOr<string>> LoginUserAsync(string username, string password);
    Task<ErrorOr<User>> GetUser(string userId);
}
