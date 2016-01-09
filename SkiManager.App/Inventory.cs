using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SkiManager.App
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class Inventory
    {
        [JsonProperty]
        private readonly Dictionary<string, float> _store = new Dictionary<string, float>();

        [JsonProperty]
        private readonly List<Item> _storedItemTypes = new List<Item>();

        [JsonProperty]
        public float Capacity { get; set; }

        public float UsedCapacity => _store.Select(kvp => _storedItemTypes.First(_ => _.ToString() == kvp.Key).UsedCapacityPerUnit * kvp.Value).Sum();

        public float FreeCapacity => Capacity - UsedCapacity;

        public Inventory(float capacity = float.MaxValue)
        {
            Capacity = capacity;
        }

        public void AddItem(Item item, int amount) => AddInternal(item, amount);

        public void AddItem(Item item, float amount)
        {
            if (item.Unit != ItemUnit.Continuous)
            {
                throw new InvalidOperationException("Item can only be added in discrete amounts");
            }
            AddInternal(item, amount);
        }

        public bool HasItem(Item item, int amount = 1) => HasItemInternal(item, amount);

        public bool HasItem(Item item, float amount)
        {
            if (item.Unit != ItemUnit.Continuous)
            {
                throw new InvalidOperationException("Item uses only discrete amounts");
            }
            return HasItemInternal(item, amount);
        }

        public bool TryTakeItem(Item item, int amount = 1) => TryTakeItemInternal(item, amount);

        public bool TryTakeItem(Item item, float amount)
        {
            if (item.Unit != ItemUnit.Continuous)
            {
                throw new InvalidOperationException("Item uses only discrete amounts");
            }
            return TryTakeItemInternal(item, amount);
        }

        private void AddInternal(Item item, float amount)
        {
            EnsureItemStoreSlot(item);
            _store[item.ToString()] += amount;
        }

        private bool HasItemInternal(Item item, float amount)
        {
            EnsureItemStoreSlot(item);
            return _store[item.ToString()] >= amount;
        }

        private bool TryTakeItemInternal(Item item, float amount)
        {
            if (!HasItemInternal(item, amount))
            {
                return false;
            }
            _store[item.ToString()] -= amount;
            return true;
        }

        private void EnsureItemStoreSlot(Item item)
        {
            if (_store.All(kvp => kvp.Key != item.ToString()))
            {
                _store.Add(item.ToString(), 0.0f);
            }
            if (_storedItemTypes.All(_ => _ != item))
            {
                _storedItemTypes.Add(item);
            }
        }
    }

    [JsonObject]
    public struct Item : IEquatable<Item>
    {
        [JsonProperty]
        public string Name { get; }

        [JsonProperty]
        public ItemUnit Unit { get; }

        [JsonProperty]
        public float UsedCapacityPerUnit { get; }

        public Item(string name, ItemUnit unit, float usedCapacityPerUnit)
        {
            Name = name;
            Unit = unit;
            UsedCapacityPerUnit = usedCapacityPerUnit;
        }

        public override int GetHashCode() => ToString()?.GetHashCode() ?? 0;

        public override bool Equals(object obj)
        {
            if (!(obj is Item))
            {
                return false;
            }
            return Equals((Item)obj);
        }

        public bool Equals(Item other) => other.Name == Name && other.Unit == Unit && Math.Abs(other.UsedCapacityPerUnit - UsedCapacityPerUnit) < float.Epsilon;

        public static bool operator ==(Item left, Item right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Item left, Item right)
        {
            return !(left == right);
        }

        public override string ToString() => $"{Name} ({UsedCapacityPerUnit} per {(Unit == ItemUnit.Discrete ? "piece" : "unit")})";
    }

    public enum ItemUnit
    {
        Discrete,
        Continuous
    }
}
