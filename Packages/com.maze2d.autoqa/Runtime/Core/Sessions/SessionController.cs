using System;
using System.Collections.Generic;

namespace AutoQA
{
    /// <summary>Owns the test session identity, targets and command history. A new session clears these; it is not a gameplay round.</summary>
    public sealed class SessionController
    {
        public string SessionId { get; private set; }
        public bool IsActive => SessionId != null;
        public TargetRegistry Targets { get; } = new TargetRegistry();
        internal readonly Dictionary<string, ActionResult> Results = new Dictionary<string, ActionResult>();
        internal readonly Dictionary<string, string> Payloads = new Dictionary<string, string>();
        internal bool Executing;
        public event Action Changed;
        public string BeginSession() { EndSession(); SessionId = Guid.NewGuid().ToString("N"); Changed?.Invoke(); return SessionId; }
        public void EndSession()
        {
            if (Executing) throw new InvalidOperationException("Cannot end a session during execution");
            SessionId = null; Targets.Clear(); Results.Clear(); Payloads.Clear(); Changed?.Invoke();
        }
    }
}
