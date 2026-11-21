using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoQA
{
    /// <summary>Combines provider data and optional descriptors, publishes actions and updates revision when the snapshot changes.</summary>
    public sealed class Observer
    {
        readonly SessionController session;
        readonly IInteractionProvider interactions;
        readonly IGameContextProvider context;
        readonly IReadinessProvider readiness;
        readonly ITestOracle oracle;
        string projection, observationId;
        long revision;
        public ActionCatalog Catalog { get; private set; } = new ActionCatalog();
        public Observer(SessionController session, IInteractionProvider interactions, IGameContextProvider context,
            IReadinessProvider readiness, ITestOracle oracle)
        {
            this.session = session; this.interactions = interactions; this.context = context;
            this.readiness = readiness; this.oracle = oracle;
            session.Changed += Reset;
        }
        public void Dispose() => session.Changed -= Reset;
        void Reset() { projection = null; revision = 0; observationId = null; Catalog = new ActionCatalog(); }
        public void Invalidate() { projection = null; Catalog = new ActionCatalog(); }
        public Observation Capture()
        {
            if (!session.IsActive) throw new InvalidOperationException("No active session");
            session.Targets.Prune();
            var o = new Observation { sessionId = session.SessionId, scene = SceneManager.GetActiveScene().name };
            var catalog = new ActionCatalog();
            try
            {
                context?.CollectContext(o);
                o.readiness = readiness?.GetReadiness() ?? new Readiness();
                interactions?.Collect(new CaptureContext(o, session.Targets, catalog));
                foreach (var descriptor in UnityEngine.Object.FindObjectsByType<QAContext>(FindObjectsSortMode.InstanceID))
                {
                    if (!descriptor.enabled || !ContextVisibility.IsVisible(descriptor) || string.IsNullOrWhiteSpace(descriptor.Description)) continue;
                    o.descriptions.Add(new ContextDescription {
                        ownerId = session.Targets.Register(descriptor.gameObject),
                        parentId = descriptor.transform.parent ? session.Targets.Register(descriptor.transform.parent.gameObject) : null,
                        semanticKey = descriptor.SemanticKey, description = descriptor.Description });
                }
                oracle?.Evaluate(o);
                var keys = new System.Collections.Generic.HashSet<string>();
                foreach (var e in o.elements)
                    if (!string.IsNullOrEmpty(e.semanticKey) && !keys.Add(e.semanticKey))
                        throw new InvalidOperationException("Ambiguous semantic key: " + e.semanticKey);
            }
            catch (Exception ex)
            {
                o.readiness = new Readiness { status = "unavailable", reason = "Provider failure" };
                o.diagnostics.Add(ex.ToString()); o.availableActions.Clear(); catalog = new ActionCatalog();
            }
            if (o.readiness.status != "ready") { o.availableActions.Clear(); catalog = new ActionCatalog(); }
            // Metadata is assigned after comparison, so repeated reads retain identity.
            var next = JsonUtility.ToJson(o);
            if (next != projection) { revision++; observationId = Guid.NewGuid().ToString("N"); projection = next; }
            o.revision = revision; o.observationId = observationId; Catalog = catalog;
            return o;
        }
    }
}
