using ErrorOr;

namespace WhatShouldIWorkOnToday.Domain.Common.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials => Error.Validation(
            code: "Auth.InvalidCredentials",
            description: "Invalid Credentials");

        public static Error PasswordRequiresNonAlpha => Error.Validation(
            code: "Auth.PasswordRequiredNonAlpha",
            description: "Password must contain a non-alphanumeric character");

        public static Error Unknown => Error.Unexpected(
            code: "Auth.Unexpected",
            description: "Unexpected authorization error");
    }
}
