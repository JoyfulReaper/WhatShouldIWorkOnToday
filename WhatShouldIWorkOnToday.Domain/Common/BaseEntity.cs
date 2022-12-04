namespace WhatShouldIWorkOnToday.Domain.Common;

public class BaseEntity : IEquatable<BaseEntity>
{
    public int Id { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is BaseEntity entity && Id.Equals(entity.Id);
    }

    public bool Equals(BaseEntity? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(BaseEntity left, BaseEntity right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity left, BaseEntity right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
