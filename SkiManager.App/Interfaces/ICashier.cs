using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiManager.App.Interfaces
{
    public interface ICashier
    {
        float TicketPrice { get; set; }

        int MinimumProcessingSeconds { get; set; }

        int MaximumProcessingSeconds { get; set; }

        bool IsProcessing { get; }

        IGraphNode NextNode { get; set; }
    }
}
