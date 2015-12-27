using System.Collections.Generic;
using System.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public abstract class TransporterBaseBehavior : ReactiveBehavior, ITransporter
    {
        private readonly List<Entity> _passengers = new List<Entity>();

        public int Slots { get; set; }

        public int UsedSlots => _passengers.Count;

        public int FreeSlots => Slots - UsedSlots;

        public bool RequiresDriver { get; set; }

        public bool IsParked { get; set; }

        public bool HasDriver => _passengers.Any(EntityCanBeDriver);

        public IReadOnlyList<Entity> Passengers => _passengers.AsReadOnly();

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
            _passengers.Add(entityToLoad);
            return true;
        }

        public void UnloadAllTo(Entity target)
        {
            _passengers.ForEach(_ => _.SetParent(target));
            _passengers.Clear();
        }

        public virtual bool EntityCanBeDriver(Entity entity)
        {
            return true; // TODO: check for required skill
        }
    }
}
