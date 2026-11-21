using System;
using System.Diagnostics;
using UnityEngine;

namespace AutoQA
{
    /// <summary>Validates session, observation and target before execution; stores command results to prevent duplicate effects.</summary>
    public sealed class Executor
    {
        readonly SessionController session;
        readonly Observer observer;
        public Executor(SessionController session, Observer observer) { this.session = session; this.observer = observer; }
        public ActionResult GetCommandStatus(string id) => id != null && session.Results.TryGetValue(id, out var result)
            ? JsonUtility.FromJson<ActionResult>(JsonUtility.ToJson(result))
            : new ActionResult { commandId = id, status = "unknown", code = "unknownCommand" };
        static ActionResult Reject(ActionCommand c, string code) => new ActionResult { commandId = c?.commandId, status = "rejected", code = code };
        public ActionResult Execute(ActionCommand command)
        {
            if (command == null || string.IsNullOrEmpty(command.commandId)) return Reject(command, "invalidCommand");
            if (!session.IsActive || command.sessionId != session.SessionId) return Reject(command, "wrongSession");
            var payload = JsonUtility.ToJson(command);
            if (session.Payloads.TryGetValue(command.commandId, out var old))
                return old == payload ? GetCommandStatus(command.commandId) : Reject(command, "commandConflict");
            if (session.Executing) return Reject(command, "commandInProgress");
            var result = Reject(command, "invalidCommand");
            session.Payloads.Add(command.commandId, payload); session.Results.Add(command.commandId, result);
            session.Executing = true;
            var watch = Stopwatch.StartNew();
            try
            {
                if (command.timeoutSeconds <= 0 || float.IsNaN(command.timeoutSeconds) || float.IsInfinity(command.timeoutSeconds)) result.code = "invalidTimeout";
                else if (command.executionMode != "direct") result.code = "unsupportedMode";
                else
                {
                    var observation = observer.Capture();
                    if (command.revision != observation.revision || command.observationId != observation.observationId) result.code = "staleObservation";
                    else if (observation.readiness.status != "ready") result.code = "notReady";
                    else if (string.IsNullOrEmpty(command.actionId) || !observer.Catalog.TryGet(command.actionId, out var entry)) result.code = "unknownAction";
                    else if (!session.Targets.TryGet(entry.Candidate.targetId, out _)) result.code = "missingTarget";
                    else if (!entry.Handler.CanExecute(out var reason)) { result.code = "unavailableTarget"; result.description = reason; }
                    else
                    {
                        observer.Invalidate(); result.status = "running"; result.code = "running";
                        entry.Handler.Execute();
                        result.status = watch.Elapsed.TotalSeconds > command.timeoutSeconds ? "timedOut" : "completed";
                        result.code = result.status == "completed" ? "ok" : "deadlineExceededAfterExecution";
                    }
                }
            }
            catch (Exception ex) { observer.Invalidate(); result.status = "failed"; result.code = "executionException"; result.description = ex.ToString(); }
            finally { result.durationSeconds = watch.Elapsed.TotalSeconds; session.Executing = false; }
            return GetCommandStatus(command.commandId);
        }
    }
}
