using UnityEngine;

namespace AutoQA
{
    [DisallowMultipleComponent]
    /// <summary>Optional Inspector description of a screen or control. Standard UI discovery and actions do not require this component.</summary>
    public sealed class QAContext : MonoBehaviour
    {
        [TextArea(3, 12)] public string Description;
        public string SemanticKey;
    }
    /// <summary>Filters inactive objects, disabled canvases and transparent CanvasGroups; does not test occlusion or raycast accessibility.</summary>
    public static class ContextVisibility
    {
        public static bool IsVisible(Component component)
        {
            if (!component || !component.gameObject.activeInHierarchy) return false;
            foreach (var canvas in component.GetComponentsInParent<Canvas>())
                if (!canvas.enabled) return false;
            foreach (var group in component.GetComponentsInParent<CanvasGroup>())
            {
                if (group.alpha <= 0.001f) return false;
                if (group.ignoreParentGroups) break;
            }
            return true;
        }
    }
}
