using System.Collections.Generic;
using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface ITransporter : IHasEntity
    {
        int Slots { get; }

        int UsedSlots { get; }

        bool RequiresDriver { get; }

        bool HasDriver { get; }

        bool IsParked { get; set; }

        Skill RequiredDriverSkills { get; }

        IReadOnlyList<Entity> Passengers { get; }

        bool TryLoad(Entity entityToLoad);

        void UnloadAllTo(Entity target);
    }
}
