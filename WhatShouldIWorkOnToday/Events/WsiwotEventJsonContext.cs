using System.Text.Json.Serialization;

namespace WhatShouldIWorkOnToday.Events;

[JsonSerializable(typeof(WsiwotLoginSucceededEvent))]
[JsonSerializable(typeof(WsiwotLoginFailedEvent))]
internal sealed partial class WsiwotEventJsonContext
    : JsonSerializerContext;