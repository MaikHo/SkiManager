using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiManager.Engine
{
    internal sealed class EngineStatus : IEngineStatus
    {
        public bool IsPaused { get; internal set; }


    }
}
