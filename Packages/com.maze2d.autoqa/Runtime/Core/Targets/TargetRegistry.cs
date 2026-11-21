using System.Collections.Generic;
using UnityEngine;

namespace AutoQA
{
    /// <summary>Maps session-scoped IDs to live Unity objects. Repeated registration retains identity; Prune removes destroyed targets.</summary>
    public sealed class TargetRegistry
    {
        readonly Dictionary<int, string> ids = new Dictionary<int, string>();
        readonly Dictionary<string, Object> targets = new Dictionary<string, Object>();
        long next;
        public string Register(Object target)
        {
            if (!target) throw new System.ArgumentNullException(nameof(target));
            var instance = target.GetInstanceID();
            if (ids.TryGetValue(instance, out var id) && targets.TryGetValue(id, out var old) && old == target) return id;
            id = "target-" + ++next;
            ids[instance] = id; targets[id] = target; return id;
        }
        public bool TryGet(string id, out Object target) => targets.TryGetValue(id, out target) && target;
        public void Prune()
        {
            var dead = new List<string>();
            foreach (var pair in targets) if (!pair.Value) dead.Add(pair.Key);
            foreach (var id in dead) targets.Remove(id);
        }
        public void Clear() { ids.Clear(); targets.Clear(); next = 0; }
    }
}
