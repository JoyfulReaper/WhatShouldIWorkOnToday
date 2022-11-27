using ErrorOr;

namespace WhatShouldIWorkOnToday.Domain.Common.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicateEmail => Error.Conflict(
            code: "User.DuplicateEmail",
            description: "Email already in use");

        public static Error IdNotFound => Error.NotFound(
            code: "User.UserIdNotFound",
            description: "No users exist with the provided Id");
    }
}
