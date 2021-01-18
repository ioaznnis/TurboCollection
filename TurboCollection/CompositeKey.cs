using System;
using System.Collections.Generic;

namespace TurboCollection
{
    public readonly struct CompositeKey<TKeyId, TKeyName> : IEquatable<CompositeKey<TKeyId, TKeyName>>
    {
        public CompositeKey(TKeyId id, TKeyName name)
        {
            Id = id;
            Name = name;
        }

        public readonly TKeyId Id;

        public readonly TKeyName Name;

        public bool Equals(CompositeKey<TKeyId, TKeyName> other)
        {
            return EqualityComparer<TKeyId>.Default.Equals(Id, other.Id) &&
                   EqualityComparer<TKeyName>.Default.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            return obj is CompositeKey<TKeyId, TKeyName> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public static bool operator ==(CompositeKey<TKeyId, TKeyName> left, CompositeKey<TKeyId, TKeyName> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CompositeKey<TKeyId, TKeyName> left, CompositeKey<TKeyId, TKeyName> right)
        {
            return !left.Equals(right);
        }

        public static implicit operator CompositeKey<TKeyId, TKeyName>(ValueTuple<TKeyId, TKeyName> valueTuple) =>
            new(valueTuple.Item1, valueTuple.Item2);

        public void Deconstruct(out TKeyId id, out TKeyName name)
        {
            id = Id;
            name = Name;
        }
    }
}