namespace SkiManager.Engine
{
    public sealed class RenderLayer
    {
        public static RenderLayer Default { get; } = new RenderLayer(0);

        public int Index { get; }

        public RenderLayer(int index)
        {
            Index = index;
        }

        public override string ToString() => $"RenderLayer[{Index}]";

        public override int GetHashCode() => Index.GetHashCode();

        public override bool Equals(object obj)
        {
            var layer = obj as RenderLayer;

            return layer?.Index == Index;
        }

        public static bool operator ==(RenderLayer left, RenderLayer right) => left?.Equals(right) ?? Equals(right, null);

        public static bool operator !=(RenderLayer left, RenderLayer right) => !(left == right);
    }
}
