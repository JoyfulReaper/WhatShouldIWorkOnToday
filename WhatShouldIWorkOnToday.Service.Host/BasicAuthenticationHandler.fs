namespace WhatShouldIWorkOnToday.Server.Authentication

open Microsoft.AspNetCore.Authentication
open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging
open System.Security.Claims
open System.Text
open System.Text.Encodings.Web
open System.Text.RegularExpressions
open Microsoft.Extensions.Configuration
open System.Threading.Tasks
open System

type BasicAuthenticationHandler (options: IOptionsMonitor<AuthenticationSchemeOptions>, loggerFactory: ILoggerFactory, encoder: UrlEncoder, clock: ISystemClock, config: IConfiguration) =
    inherit AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder, clock)

    let _config = config

    override __.HandleAuthenticateAsync () =
        base.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"WhatShouldIWorkOnToday\"")

        if not <| base.Request.Headers.ContainsKey("Authorization") then
            Task.FromResult(AuthenticateResult.Fail("Authorization header missing."))
        else
            let authorizationHeader = base.Request.Headers.["Authorization"].ToString()
            let authHeaderRegex = Regex(@"Basic (.*)")

            if not <| authHeaderRegex.IsMatch(authorizationHeader) then
                 Task.FromResult(AuthenticateResult.Fail("Authorization code not formatted properly."))
            else
                let authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")))
                let authSplit = authBase64.Split(Convert.ToChar(":"), 2)
                let authUsername = authSplit.[0]
                let authPassword =
                    if authSplit.Length > 1 then
                        authSplit.[1]
                    else
                        failwith "Unable to get password"

                let validUser = "user"          // TODO: Get this from config
                let validPassword = "password"

                if authUsername <> validUser || authPassword <> validPassword then
                     Task.FromResult(AuthenticateResult.Fail("The username or password is not correct."))
                else
                    let authenticatedUser = AuthenticatedUser("BasicAuthentication", true, authUsername)
                    let claimsPrinciple = ClaimsPrincipal(new ClaimsIdentity(authenticatedUser))
                    Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrinciple, base.Scheme.Name)))
