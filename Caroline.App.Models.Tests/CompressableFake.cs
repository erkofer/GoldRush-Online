using System;

namespace Caroline.App.Models.Tests
{
    class CompressableFake : ICompressable<CompressableFake>, IIdentifiableObject
    {
        protected bool Equals(CompressableFake other)
        {
            if (ReferenceEquals(this, other))
                return true;
            return Id == other.Id && Value == other.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ Value;
            }
        }

        public CompressableFake(int id, int value)
        {
            Value = value;
            Id = id;
        }

        public CompressableFake Compress(CompressableFake oldObject)
        {
            if (Id != oldObject.Id)
                throw new InvalidOperationException();
            if (Value != oldObject.Value)
                return new CompressableFake(Id, Value);
            return null;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CompressableFake)obj);
        }

        public override string ToString()
        {
            return String.Format("Id: {0}, Value: {1}", Id, Value);
        }

        public int Id { get; private set; }
        public int Value { get; private set; }
    }
}