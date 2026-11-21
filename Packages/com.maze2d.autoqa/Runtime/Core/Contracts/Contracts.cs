using System;
using System.Collections.Generic;

namespace AutoQA
{
    /// <summary>A game-specific key/value fact, such as width=8, represented without Unity object references.</summary>
    [Serializable] public class Fact { public string key; public string value; public Fact(string key, string value) { this.key = key; this.value = value; } }
    /// <summary>Reports ready, busy or unavailable with an explanatory reason.</summary>
    [Serializable] public class Readiness { public string status = "unavailable"; public string reason = "No readiness provider"; }
    /// <summary>An objective game-state check produced by an oracle, independent of command completion.</summary>
    [Serializable] public class CheckResult { public string key; public string status; public string description; }
    /// <summary>Serializable UI target description: identity, label, value and availability, without a live Unity reference.</summary>
    [Serializable] public class Element
    {
        public string targetId, semanticKey, kind, name, label, value, blockedReason;
        public bool active, interactable;
    }
    /// <summary>Optional descriptor text with owner and parent target IDs preserving its structural scope.</summary>
    [Serializable] public class ContextDescription { public string ownerId, parentId, semanticKey, description; }
    /// <summary>An offered action with predefined parameters. Clients select its ID instead of inventing execution parameters.</summary>
    [Serializable] public class ActionCandidate
    {
        public string actionId, targetId, semanticKey, kind, description, value;
        public string executionMode = "direct";
    }
    /// <summary>Versioned snapshot of scene, UI, readiness, game facts, descriptions, checks and offered actions.</summary>
    [Serializable] public class Observation
    {
        public int schemaVersion = 1;
        public string sessionId, observationId, scene, screen;
        public long revision;
        public Readiness readiness = new Readiness();
        public List<Element> elements = new List<Element>();
        public List<ActionCandidate> availableActions = new List<ActionCandidate>();
        public List<Fact> gameContext = new List<Fact>();
        public List<ContextDescription> descriptions = new List<ContextDescription>();
        public List<CheckResult> checks = new List<CheckResult>();
        public List<string> diagnostics = new List<string>();
    }
    /// <summary>Requests an offered action from a particular session and observation; commandId identifies retries.</summary>
    [Serializable] public class ActionCommand
    {
        public string commandId, sessionId, observationId, actionId;
        public long revision;
        public string executionMode = "direct";
        public float timeoutSeconds = 10;
    }
    /// <summary>Execution status, reason and duration. Completed means the operation ran, not that the test passed.</summary>
    [Serializable] public class ActionResult
    {
        public string commandId, status, code, description;
        public string executionMode = "direct";
        public double durationSeconds;
        public bool requiresObservation = true;
    }
}
