using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WhatShouldIWorkOnToday.Auth;

public sealed class SingleUserAuthService
{
    private readonly SingleUser _user;
    private readonly string _passwordHash;
    private readonly IPasswordHasher<SingleUser> _passwordHasher;

    public SingleUserAuthService(
        IOptions<SingleUserOptions> options,
        IPasswordHasher<SingleUser> passwordHasher)
    {
        var settings = options.Value;

        _user = new SingleUser(settings.Username);
        _passwordHasher = passwordHasher;

        _passwordHash = passwordHasher.HashPassword(_user, settings.Password);
    }

    public string Username => _user.Username;

    public bool Validate(
        string username,
        string password)
    {
        if (!string.Equals(username, _user.Username, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            _user,
            _passwordHash,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}