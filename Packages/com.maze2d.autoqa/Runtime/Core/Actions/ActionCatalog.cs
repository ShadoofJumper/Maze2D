using System.Collections.Generic;

namespace AutoQA
{
    /// <summary>Maps current observation action IDs to candidates and local handlers. Observer replaces the catalog on each capture.</summary>
    public sealed class ActionCatalog
    {
        /// <summary>Pairs a serializable candidate with the local handler that performs its effect.</summary>
        public sealed class Entry { public ActionCandidate Candidate; public IActionHandler Handler; }
        readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>();
        public void Add(ActionCandidate candidate, IActionHandler handler)
        {
            if (string.IsNullOrEmpty(candidate.actionId)) throw new System.ArgumentException("Missing actionId");
            entries.Add(candidate.actionId, new Entry { Candidate = candidate, Handler = handler });
        }
        public bool TryGet(string id, out Entry entry) => entries.TryGetValue(id, out entry);
    }
}
