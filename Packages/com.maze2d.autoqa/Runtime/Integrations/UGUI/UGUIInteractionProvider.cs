using System;
using System.Linq;
using AutoQA;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AutoQA.UGUI
{
    /// <summary>Discovers visible Button and TMP_InputField controls and offers direct click/setValue actions without game view accessors.</summary>
    public sealed class UGUIInteractionProvider : IInteractionProvider
    {
        readonly ITestDataProvider data;
        readonly ISemanticProvider semantics;
        public UGUIInteractionProvider(ITestDataProvider data = null, ISemanticProvider semantics = null) { this.data = data; this.semantics = semantics; }
        public void Collect(CaptureContext context)
        {
            foreach (var selectable in UnityEngine.Object.FindObjectsByType<Selectable>(FindObjectsSortMode.InstanceID))
            {
                if (!(selectable is Button) && !(selectable is TMP_InputField)) continue;
                var visible = ContextVisibility.IsVisible(selectable);
                if (!visible) continue;
                var field = selectable as TMP_InputField;
                var key = semantics?.GetKey(selectable);
                var descriptor = selectable.GetComponent<QAContext>();
                if (string.IsNullOrEmpty(key) && descriptor && descriptor.enabled) key = descriptor.SemanticKey;
                var id = context.Targets.Register(selectable.gameObject);
                var available = selectable.IsActive() && selectable.IsInteractable() && (!field || !field.readOnly);
                var label = selectable.GetComponentInChildren<TMP_Text>()?.text ?? selectable.GetComponentInChildren<Text>()?.text ?? selectable.name;
                context.Observation.elements.Add(new Element { targetId = id, semanticKey = key,
                    kind = field ? "inputField" : "button", name = selectable.name, label = label,
                    value = field ? field.text : null, active = true, interactable = available,
                    blockedReason = available ? null : "Control is disabled, blocked by CanvasGroup, or read-only" });
                if (!available || semantics?.IsAllowed(selectable) == false) continue;
                if (!field) Add(context, selectable, id, key, "click", null, "Activate " + label);
                else
                {
                    var values = data?.GetValues(field);
                    if (values == null) continue;
                    foreach (var value in values.Where(v => v != null).Distinct())
                        Add(context, selectable, id, key, "setValue", value, "Set " + label + " to " + value);
                }
            }
        }
        static void Add(CaptureContext context, Selectable control, string id, string key, string kind, string value, string description)
        {
            var candidate = new ActionCandidate { actionId = id + ":" + kind + ":" + (value ?? ""), targetId = id,
                semanticKey = key, kind = kind, value = value, description = description };
            context.Actions.Add(candidate, new Handler(control, value)); context.Observation.availableActions.Add(candidate);
        }
        /// <summary>Direct control handler: invokes Button.onClick or sets TMP text and invokes onEndEdit. Does not simulate physical input.</summary>
        sealed class Handler : IActionHandler
        {
            readonly Selectable control; readonly string value;
            public Handler(Selectable control, string value) { this.control = control; this.value = value; }
            public bool CanExecute(out string reason)
            {
                reason = "Control unavailable";
                return control && ContextVisibility.IsVisible(control) && control.IsActive() && control.IsInteractable()
                    && (!(control is TMP_InputField f) || !f.readOnly);
            }
            public void Execute()
            {
                if (control is Button button) button.onClick.Invoke();
                else if (control is TMP_InputField field)
                {
                    field.text = value;
                    field.onEndEdit.Invoke(field.text);
                }
            }
        }
    }
}
