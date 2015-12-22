namespace SkiManager.Engine.Features
{
    public abstract class EngineFeature
    {
        public bool IsAttached { get; private set; }

        protected Engine Engine { get; private set; }

        public void Attach(Engine engine)
        {
            Engine = engine;
            Attach();
            IsAttached = true;
        }

        public void Detach(Engine engine)
        {
            Detach();
            Engine = null;
            IsAttached = false;
        }

        protected abstract void Attach();

        protected abstract void Detach();
    }
}
