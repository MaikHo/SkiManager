using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiManager.App.Behaviors
{
    public sealed class RoadBehavior : GraphEdgeBehavior
    {
        public RoadBehavior()
        {
            BaseSpeedModifier = 1.2f;
        }
    }
}
