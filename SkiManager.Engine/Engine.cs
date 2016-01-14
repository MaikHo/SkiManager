using System;
using System.Collections.Generic;

using Microsoft.Graphics.Canvas.UI.Xaml;

using SkiManager.Engine.Features;

namespace SkiManager.Engine
{
    public sealed class Engine
    {
        public static Engine Current { get; } = new Engine();

        private CanvasVirtualControl _control;

        private EngineEvents _events;
        public IEngineEvents Events => _events;

        private readonly EngineStatus _status = new EngineStatus();
        public IEngineStatus Status => _status;

        private readonly List<EngineFeature> _features = new List<EngineFeature>();
        public IReadOnlyList<EngineFeature> Features => _features.AsReadOnly();

        public Level CurrentLevel { get; private set; }

        private Engine()
        { }

        public void Attach(CanvasVirtualControl control)
        {
            _control = control;

            _events = EngineEvents.Attach(this, control);
            _status.IsPaused = true;
        }

        public void RegisterRenderLayer(RenderLayer renderLayer)
        {
            if (Events == null)
            {
                throw new InvalidOperationException("Engine has to be attached to register render layers.");
            }
            if (!_status.IsPaused)
            {
                throw new InvalidOperationException("Engine may not be running when registering render layers.");
            }

            _events.RegisterRenderLayer(renderLayer);
        }

        public void AddFeature(EngineFeature feature)
        {
            if (feature.IsAttached)
            {
                throw new InvalidOperationException("Cannot add an already attached feature.");
            }

            feature.Attach(this);
            _features.Add(feature);
        }

        public void RemoveFeature(EngineFeature feature)
        {
            if (!feature.IsAttached || !_features.Contains(feature))
            {
                throw new InvalidOperationException("Cannot detach feature that is not attached to this engine.");
            }

            feature.Detach(this);
            _features.Remove(feature);
        }

        public void LoadLevel(Level level)
        {
            CurrentLevel?.Unloading();

            CurrentLevel = level;

            if (level.StartDate == DateTime.MinValue)
            {
                level.StartDate = DateTime.Now;
            }

            CurrentLevel?.Loaded();
        }

        public void StartOrResume()
        {
            _events.LastUpdateTime = DateTime.Now;
            _status.IsPaused = false;
        }

        public void Pause()
        {
            _status.IsPaused = true;
        }
    }
}
