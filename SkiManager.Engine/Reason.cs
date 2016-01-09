namespace SkiManager.Engine
{
    public struct Reason
    {
        public static readonly Reason None = new Reason { Name = "None", Parameter = null };

        public string Name { get; set; }

        public object Parameter { get; set; }

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (!(obj is Reason))
            {
                return false;
            }
            return ((Reason)obj).Name == Name;
        }

        public bool Equals(Reason other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0) * 397) ^ (Parameter?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(Reason left, Reason right) => left.Equals(right);

        public static bool operator !=(Reason left, Reason right) => !left.Equals(right);
    }
}
