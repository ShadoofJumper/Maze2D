using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoQA
{
    /// <summary>Keeps up to 20 bounded Unity error, exception and assert records. Dispose unsubscribes from logging.</summary>
    public sealed class DiagnosticBuffer : IDisposable
    {
        readonly Queue<string> entries = new Queue<string>();
        const int Capacity = 20;
        public DiagnosticBuffer() => Application.logMessageReceived += Receive;
        void Receive(string message, string stack, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            var entry = type + ": " + message + "\n" + stack;
            entries.Enqueue(entry.Length > 2048 ? entry.Substring(0, 2048) : entry);
            while (entries.Count > Capacity) entries.Dequeue();
        }
        public void AppendTo(List<string> destination) => destination.AddRange(entries);
        public void Clear() => entries.Clear();
        public void Dispose() => Application.logMessageReceived -= Receive;
    }
}
