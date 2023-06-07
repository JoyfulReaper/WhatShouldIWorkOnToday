
using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.ApiClient.Common.Options;
public class WsiwotClientOptions
{
    [Required]
    public string BaseUrl { get; set; } = default!;
}
