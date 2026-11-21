using System;
using NUnit.Framework;
using UnityEngine;

namespace AutoQA.Tests
{
    /// <summary>Edit Mode checks of serialization, snapshot identity, session lifecycle, deduplication and failure handling.</summary>
    public class RuntimeTests
    {
        /// <summary>Minimal test provider and handler with an effect counter, mutable state and controlled failures.</summary>
        sealed class Fixture : IInteractionProvider, IReadinessProvider, IActionHandler
        {
            public GameObject Target;
            public int Count;
            public string Value = "first";
            public bool Throw;
            public bool ThrowOnExecute;
            public Readiness GetReadiness() => new Readiness { status = "ready" };
            public void Collect(CaptureContext context)
            {
                if (Throw) throw new InvalidOperationException("provider error");
                if (!Target) return;
                var id = context.Targets.Register(Target);
                context.Observation.elements.Add(new Element { targetId = id, value = Value });
                var candidate = new ActionCandidate { actionId = "click", targetId = id };
                context.Actions.Add(candidate, this); context.Observation.availableActions.Add(candidate);
            }
            public bool CanExecute(out string reason) { reason = null; return Target; }
            public void Execute() { Count++; if (ThrowOnExecute) throw new InvalidOperationException("handler error"); }
        }
        GameObject target;
        Fixture fixture;
        RuntimeFacade runtime;
        [SetUp] public void Setup()
        {
            target = new GameObject("Target"); fixture = new Fixture { Target = target };
            runtime = new RuntimeFacade(fixture, readiness: fixture); runtime.BeginSession();
        }
        [TearDown] public void Cleanup() { runtime.Dispose(); if (target) UnityEngine.Object.DestroyImmediate(target); }
        ActionCommand Command(Observation o) => new ActionCommand { commandId = Guid.NewGuid().ToString("N"),
            sessionId = o.sessionId, observationId = o.observationId, revision = o.revision, actionId = "click" };
        [Test] public void JsonRoundTripAndStableObservation()
        {
            var a = runtime.CaptureObservation(); var b = runtime.CaptureObservation();
            Assert.AreEqual(a.observationId, b.observationId); Assert.AreEqual(a.revision, b.revision);
            var restored = JsonUtility.FromJson<Observation>(JsonUtility.ToJson(a));
            Assert.AreEqual(a.availableActions[0].targetId, restored.availableActions[0].targetId);
            Assert.IsEmpty(restored.descriptions);
        }
        [Test] public void ChangedStateRejectsStaleCommand()
        {
            var c = Command(runtime.CaptureObservation()); fixture.Value = "changed";
            Assert.AreEqual("staleObservation", runtime.Execute(c).code); Assert.AreEqual(0, fixture.Count);
        }
        [Test] public void DuplicateCommandRunsOnceAndConflictingPayloadIsRejected()
        {
            var c = Command(runtime.CaptureObservation());
            Assert.AreEqual("completed", runtime.Execute(c).status);
            Assert.AreEqual("completed", runtime.Execute(c).status); Assert.AreEqual(1, fixture.Count);
            c.actionId = "other"; Assert.AreEqual("commandConflict", runtime.Execute(c).code);
        }
        [Test] public void SessionRestartRejectsOldCommandAndClearsResults()
        {
            var c = Command(runtime.CaptureObservation()); runtime.Execute(c); runtime.BeginSession();
            Assert.AreEqual("wrongSession", runtime.Execute(c).code);
            Assert.AreEqual("unknown", runtime.GetCommandStatus(c.commandId).status);
        }
        [Test] public void DestroyedTargetCannotExecute()
        {
            var c = Command(runtime.CaptureObservation()); UnityEngine.Object.DestroyImmediate(target);
            Assert.AreEqual("rejected", runtime.Execute(c).status); Assert.AreEqual(0, fixture.Count);
        }
        [Test] public void ProviderFailureIsUnavailable()
        {
            fixture.Throw = true; var o = runtime.CaptureObservation();
            Assert.AreEqual("unavailable", o.readiness.status); Assert.IsEmpty(o.availableActions); Assert.IsNotEmpty(o.diagnostics);
        }
        [Test] public void ExecutionInvalidatesPreviousCandidates()
        {
            var o = runtime.CaptureObservation(); runtime.Execute(Command(o));
            Assert.AreEqual("staleObservation", runtime.Execute(Command(o)).code); Assert.AreEqual(1, fixture.Count);
        }
        [Test] public void UnknownActionIsRejected()
        {
            var c = Command(runtime.CaptureObservation()); c.actionId = "missing";
            Assert.AreEqual("unknownAction", runtime.Execute(c).code); Assert.AreEqual(0, fixture.Count);
        }
        [Test] public void HandlerFailureIsStoredAndNotRetried()
        {
            fixture.ThrowOnExecute = true; var c = Command(runtime.CaptureObservation());
            Assert.AreEqual("failed", runtime.Execute(c).status);
            Assert.AreEqual("executionException", runtime.Execute(c).code); Assert.AreEqual(1, fixture.Count);
        }
    }
}
