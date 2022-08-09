using Microsoft.AspNetCore.Authorization;

namespace WhatShouldIWorkOnToday.Server.Authentication;

public class BasicAuthorizationAttribute : AuthorizeAttribute
{
	public BasicAuthorizationAttribute()
	{
		Policy = "BasicAuthentication";
	}
}
