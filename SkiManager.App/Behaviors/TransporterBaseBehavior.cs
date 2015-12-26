using System.Collections.Generic;
using System.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public abstract class TransporterBaseBehavior : ReactiveBehavior, ITransporter
    {
        private readonly List<Entity> _transportees = new List<Entity>();

        public int Slots { get; set; }

        public int UsedSlots => _transportees.Count;

        public int FreeSlots => Slots - UsedSlots;

        public bool RequiresDriver { get; set; }

        public bool HasDriver => _transportees.Any(EntityCanBeDriver);

        public Skill RequiredDriverSkills { get; set; }

        public bool TryLoad(Entity entityToLoad)
        {
            if (FreeSlots == 0)
            {
                return false;
            }

            if (!HasDriver && !EntityCanBeDriver(entityToLoad) && FreeSlots == 1)
            {
                return false; // cannot load non-driver into last slot
            }

            entityToLoad.SetParent(Entity);
            _transportees.Add(entityToLoad);
            return true;
        }

        public void UnloadAllTo(Entity target)
        {
            _transportees.ForEach(_ => _.SetParent(target));
            _transportees.Clear();
        }

        private bool EntityCanBeDriver(Entity entity)
        {
            return true; // TODO: check for required skill
        }
    }
}
