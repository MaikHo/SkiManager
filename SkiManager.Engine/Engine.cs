using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Graphics.Canvas.UI.Xaml;

using SkiManager.Engine.Features;

namespace SkiManager.Engine
{
    public sealed class Engine
    {
        public static Engine Current { get; } = new Engine();

        private CanvasAnimatedControl _control;

        private EngineEvents _events;
        public IEngineEvents Events => _events;

        private readonly EngineStatus _status = new EngineStatus();
        public IEngineStatus Status => _status;

        private readonly List<EngineFeature> _features = new List<EngineFeature>();
        public IReadOnlyList<EngineFeature> Features => _features.AsReadOnly();

        private Engine()
        { }

        public void Attach(CanvasAnimatedControl control)
        {
            _control = control;
            _control.Paused = true;

            _events = EngineEvents.Attach(this, control);
            _status.IsPaused = true;
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

        public void StartOrResume()
        {
            _control.Paused = false;
            _status.IsPaused = false;
        }

        public void Pause()
        {
            _control.Paused = true;
            _status.IsPaused = true;
        }
    }
}
