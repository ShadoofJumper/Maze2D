using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace AutoQA.Maze2D.Tests
{
    public class Maze2DScenarioTests
    {
        [UnityTest] public IEnumerator StartsConfiguredGameplayWithoutDescriptors()
        {
            // The game is in Assembly-CSharp; test assemblies cannot reference it directly.
            var scenario = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType("AutoQA.Maze2D.Maze2DQAScenario")).FirstOrDefault(t => t != null);
            Assert.IsNotNull(scenario, "Maze2D runtime integration not loaded");
            bool? passed = null; string message = null;
            Action<bool, string> callback = (ok, text) => { passed = ok; message = text; };
            var routine = (IEnumerator)scenario.GetMethod("Run").Invoke(null, new object[] { callback });
            yield return routine;
            Assert.AreEqual(true, passed, message);
        }
    }
}
