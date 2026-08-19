namespace WhatShouldIWorkOnToday.Auth;

public sealed class ApiKeyOptions
{
    public const string SectionName = "Api";

    public string Key { get; set; } = string.Empty;
}