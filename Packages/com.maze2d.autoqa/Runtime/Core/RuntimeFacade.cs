using System;
using System.Threading;

namespace AutoQA
{
    /// <summary>Public entry point for session lifecycle, observation and execution. Create on the Unity main thread and dispose with its owner.</summary>
    public sealed class RuntimeFacade : IDisposable
    {
        readonly int threadId = Thread.CurrentThread.ManagedThreadId;
        readonly Observer observer;
        readonly Executor executor;
        readonly DiagnosticBuffer diagnostics = new DiagnosticBuffer();
        public SessionController Sessions { get; } = new SessionController();
        public RuntimeFacade(IInteractionProvider interactions, IGameContextProvider context = null,
            IReadinessProvider readiness = null, ITestOracle oracle = null)
        {
            observer = new Observer(Sessions, interactions, context, readiness, oracle);
            executor = new Executor(Sessions, observer);
        }
        void CheckThread() { if (Thread.CurrentThread.ManagedThreadId != threadId) throw new InvalidOperationException("Call RuntimeFacade on the Unity main thread"); }
        public string BeginSession() { CheckThread(); diagnostics.Clear(); return Sessions.BeginSession(); }
        public void EndSession() { CheckThread(); Sessions.EndSession(); }
        public Observation CaptureObservation() { CheckThread(); var result = observer.Capture(); diagnostics.AppendTo(result.diagnostics); return result; }
        public ActionResult Execute(ActionCommand command) { CheckThread(); return executor.Execute(command); }
        public ActionResult GetCommandStatus(string id) { CheckThread(); return executor.GetCommandStatus(id); }
        public void Dispose() { CheckThread(); Sessions.EndSession(); observer.Dispose(); diagnostics.Dispose(); }
    }
}
