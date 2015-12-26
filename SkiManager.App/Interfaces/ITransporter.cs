﻿using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface ITransporter
    {
        int Slots { get; }

        int UsedSlots { get; }

        bool RequiresDriver { get; }

        bool HasDriver { get; }

        Skill RequiredDriverSkills { get; }

        bool TryLoad(Entity entityToLoad);

        void UnloadAllTo(Entity target);
    }
}
