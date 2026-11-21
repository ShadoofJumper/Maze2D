using System.Collections;
using System.Linq;
using AutoQA.UGUI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace AutoQA.Tests
{
    /// <summary>Play Mode checks of real UI discovery, execution, availability and optional descriptions.</summary>
    public class UGUITests
    {
        /// <summary>Test-only readiness and fixture provider: always ready and returns input value 7; not a production readiness policy.</summary>
        sealed class Ready : IReadinessProvider, ITestDataProvider
        {
            public Readiness GetReadiness() => new Readiness { status = "ready" };
            public System.Collections.Generic.IEnumerable<string> GetValues(Component target) => new[] { "7" };
        }
        [UnityTest] public IEnumerator StandardControlsWorkWithoutDescriptorsAndHonorAvailability()
        {
            var root = new GameObject("Test Canvas", typeof(Canvas));
            var button = new GameObject("Play", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
            button.transform.SetParent(root.transform);
            var input = new GameObject("Width", typeof(RectTransform), typeof(TMP_InputField)).GetComponent<TMP_InputField>();
            input.transform.SetParent(root.transform);
            var provider = new Ready();
            var runtime = new RuntimeFacade(new UGUIInteractionProvider(provider), readiness: provider);
            runtime.BeginSession(); var clicks = 0; var edits = 0;
            button.onClick.AddListener(() => clicks++); input.onEndEdit.AddListener(_ => edits++);
            try
            {
                var o = runtime.CaptureObservation(); Assert.IsEmpty(o.descriptions);
                foreach (var kind in new[] { "click", "setValue" })
                {
                    o = runtime.CaptureObservation();
                    var targetId = runtime.Sessions.Targets.Register(kind == "click" ? button.gameObject : input.gameObject);
                    var action = o.availableActions.Single(x => x.kind == kind && x.targetId == targetId);
                    var c = new ActionCommand { commandId = kind, sessionId = o.sessionId, observationId = o.observationId,
                        revision = o.revision, actionId = action.actionId };
                    Assert.AreEqual("completed", runtime.Execute(c).status);
                    Assert.AreEqual("completed", runtime.Execute(c).status);
                }
                Assert.AreEqual(1, clicks); Assert.AreEqual(1, edits); Assert.AreEqual("7", input.text);
                button.interactable = false;
                var buttonId = runtime.Sessions.Targets.Register(button.gameObject);
                Assert.IsFalse(runtime.CaptureObservation().availableActions.Any(x => x.kind == "click" && x.targetId == buttonId));
                var context = root.AddComponent<QAContext>(); context.Description = "Test screen";
                Assert.AreEqual(1, runtime.CaptureObservation().descriptions.Count);
                root.AddComponent<CanvasGroup>().alpha = 0;
                o = runtime.CaptureObservation(); Assert.IsEmpty(o.descriptions);
                var inputId = runtime.Sessions.Targets.Register(input.gameObject);
                Assert.IsFalse(o.availableActions.Any(x => x.targetId == buttonId || x.targetId == inputId));
            }
            finally { runtime.Dispose(); Object.Destroy(root); }
            yield return null;
        }
    }
}
