namespace WhatShouldIWorkOnToday.Server.Authentication

type AuthenticatedUser(authenticationType: string, isAuthenticated: bool, name: string) =
    interface System.Security.Principal.IIdentity with
        member this.AuthenticationType = authenticationType
        member this.IsAuthenticated = isAuthenticated
        member this.Name = name