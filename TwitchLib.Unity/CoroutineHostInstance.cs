using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class CoroutineHostInstance : MonoBehaviour {
    private Queue<IEnumerator<object>> coroutineQueue;
    private Dictionary<IEnumerator<object>, TaskCompletionSource<object>> taskCompletionSources;
    private List<IEnumerator<object>> rememberRemoval;

    public Task<object> QueueCoroutine(IEnumerator<object> coroutine) {
        lock (coroutineQueue) {
            coroutineQueue.Enqueue(coroutine);
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            taskCompletionSources.Add(coroutine, tcs);
            return tcs.Task;
        }
    }

    public void Awake() {
        coroutineQueue = new Queue<IEnumerator<object>>();
        taskCompletionSources = new Dictionary<IEnumerator<object>, TaskCompletionSource<object>>();
        rememberRemoval = new List<IEnumerator<object>>();
    }

    private void Update() {
        lock (coroutineQueue) {
            while (coroutineQueue.Count > 0) {
                StartCoroutine(coroutineQueue.Dequeue());
            }
        }
        foreach (KeyValuePair<IEnumerator<object>, TaskCompletionSource<object>> kvp in taskCompletionSources) {
            if (kvp.Key != null && kvp.Key.MoveNext() == false) {
                kvp.Value.SetResult(kvp.Key.Current);
                rememberRemoval.Add(kvp.Key);
            }
        }
        if (rememberRemoval.Count > 0) {
            foreach (IEnumerator<object> ie in rememberRemoval) {
                taskCompletionSources.Remove(ie);
            }
            rememberRemoval.Clear();
        }
    }
}