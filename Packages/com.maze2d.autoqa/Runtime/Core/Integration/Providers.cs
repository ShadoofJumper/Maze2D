using System.Collections.Generic;
using UnityEngine;

namespace AutoQA
{
    /// <summary>Game integration hook that adds meaningful game facts and screen identity to an observation.</summary>
    public interface IGameContextProvider { void CollectContext(Observation observation); }
    /// <summary>Supplies an explicit readiness signal. Without a provider the runtime reports unavailable.</summary>
    public interface IReadinessProvider { Readiness GetReadiness(); }
    /// <summary>Adds objective scenario or state checks to the observation; does not choose actions.</summary>
    public interface ITestOracle { void Evaluate(Observation observation); }
    /// <summary>Supplies predefined input fixtures. Without values, input fields are observed but have no setValue candidates.</summary>
    public interface ITestDataProvider { IEnumerable<string> GetValues(Component target); }
    /// <summary>Optionally supplies semantic target keys and a game-specific policy for allowed actions.</summary>
    public interface ISemanticProvider
    {
        string GetKey(Component target);
        bool IsAllowed(Component target);
    }
    /// <summary>Discovers elements and offered actions for an interaction system, registering targets and handlers through CaptureContext.</summary>
    public interface IInteractionProvider { void Collect(CaptureContext context); }
    /// <summary>Local implementation of one action: rechecks availability in CanExecute and applies the effect in Execute.</summary>
    public interface IActionHandler
    {
        bool CanExecute(out string reason);
        void Execute();
    }
    /// <summary>Working objects for one capture: the observation to fill, live target registry and candidate/handler catalog.</summary>
    public sealed class CaptureContext
    {
        public Observation Observation { get; }
        public TargetRegistry Targets { get; }
        public ActionCatalog Actions { get; }
        public CaptureContext(Observation observation, TargetRegistry targets, ActionCatalog actions)
        { Observation = observation; Targets = targets; Actions = actions; }
    }
}
