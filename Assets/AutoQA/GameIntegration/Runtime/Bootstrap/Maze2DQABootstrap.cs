using AutoQA.UGUI;
using UnityEngine;

namespace AutoQA.Maze2D
{
    public sealed class Maze2DQABootstrap : MonoBehaviour
    {
        public static Maze2DQABootstrap Instance { get; private set; }
        public RuntimeFacade Runtime { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ClearStatic() => Instance = null;
        // Explicit opt-in: attach to a test entry scene or create from a test harness.
        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
            var adapter = new Maze2DAdapter();
            Runtime = new RuntimeFacade(new UGUIInteractionProvider(adapter, adapter), adapter, adapter, adapter);
            Runtime.BeginSession();
        }
        void OnDestroy()
        {
            Runtime?.Dispose(); Runtime = null;
            if (Instance == this) Instance = null;
        }
    }
}
