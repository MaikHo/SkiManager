namespace SkiManager.Engine
{
    public class EngineEventArgs
    {
        public static EngineEventArgs Empty { get; } = new EngineEventArgs(Engine.Current);

        public Engine Engine { get; }

        protected EngineEventArgs(Engine engine)
        {
            Engine = engine;
        }
    }
}
