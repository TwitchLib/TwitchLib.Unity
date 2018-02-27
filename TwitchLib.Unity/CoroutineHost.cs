#define COROUTINEHOST_TASKS //this option grants you ability to wait/await on the Task that indicates completion of your coroutine
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if COROUTINEHOST_TASKS
using System.Threading.Tasks;
#endif

public class CoroutineHost {
    protected static CoroutineHostInstance crossThreadHost;
    protected static CoroutineHostInstance CrossThreadHost {
        get {
            if (crossThreadHost == null) {
                crossThreadHost = InstantiateCoroutineHost(); //error when on non-main thread. Call PrepareCrossthread beforehand.
            }
            return crossThreadHost;
        }
    }

    private static CoroutineHostInstance InstantiateCoroutineHost() {
        GameObject coroutineHost = new GameObject("Coroutine Host");
        CoroutineHostInstance chi = coroutineHost.AddComponent<CoroutineHostInstance>();
        GameObject.DontDestroyOnLoad(coroutineHost);
        return chi;
    }

    /// <summary>
    /// Because we can't instantiate game objects from other thread, call this to pre-instantiate the host in the main thread first.
    /// </summary>
    public static void PrepareCrossThread() {
        crossThreadHost = InstantiateCoroutineHost();
    }

    /// <summary>
    /// You can run some lambdas immediately on the main thread. It will becomes a one-line coroutine internally.
    /// </summary>
    /// <param name="delay">If you use null, it will results in a 1 frame wait</param>
    /// <returns></returns>
#if COROUTINEHOST_TASKS
    public static Task Host(Action action, WaitForSeconds delay)
#else
    public static void Host(Action action,WaitForSeconds delay)
#endif
    {
#if COROUTINEHOST_TASKS
        return Host(ConvertActionToCoroutine(action, delay));
#else
        return Host(ConvertActionToCoroutine(action,delay));
#endif
    }

    /// <summary>
    /// You can run some lambdas immediately on the main thread. It will becomes a one-line coroutine internally.
    /// </summary>
#if COROUTINEHOST_TASKS
    public static Task Host(Action action)
#else
    public static void Host(Action action)
#endif
    {
#if COROUTINEHOST_TASKS
        return Host(ConvertActionToCoroutine(action));
#else
        return Host(ConvertActionToCoroutine(action));
#endif
    }

    private static IEnumerator ConvertActionToCoroutine(Action action) {
        action.Invoke();
        yield break;
    }

    private static IEnumerator ConvertActionToCoroutine(Action action, WaitForSeconds delay) {
        yield return delay;
        action.Invoke();
    }

    /// <summary>
    /// Think of this as just StartCoroutine() that is able to use from outside of MonoBehaviour or static methods.
    /// </summary>
#if COROUTINEHOST_TASKS
    public static Task Host(IEnumerator coroutine)
#else
    public static void Host(IEnumerator coroutine)
#endif
    {
#if COROUTINEHOST_TASKS
        return CrossThreadHost.QueueCoroutine(YieldConvert(coroutine));
#else
        CrossThreadHost.QueueCoroutine(YieldConvert(coroutine));
#endif
    }

    // I am not sure if this costs 1 more frame for all lambdas or not?
    private static IEnumerator<object> YieldConvert(IEnumerator coroutine) {
        yield return coroutine;
    }

    /// <summary>
    /// You can run lambdas WITH return value on the main thread and get that return value via Task.
    /// You still need to cast object to the type by yourself, since T cannot go down the way to MonoBehaviour where the value is returned.
    /// T is just a small compiler check so that your lambda really returns the value you want.
    /// </summary>
#if COROUTINEHOST_TASKS
    public static Task<object> Host<T>(Func<T> func)
#else
    public static void Host<T>(Func<T> func)
#endif
    {
#if COROUTINEHOST_TASKS
        return Host(ConvertFuncToCoroutine<T>(func));
#else
        return Host(ConvertFuncToCoroutine<T>(func));
#endif
    }

    private static IEnumerator<object> ConvertFuncToCoroutine<T>(Func<T> func) {
        yield return func.Invoke();
    }

    /// <summary>
    /// A method used internally but you might be able to have return value from a coroutine.. 
    /// Don't know if it works or not but for now I leave it public.
    /// But in that case you cannot yield return WaitForSeconds since it is expecting a normal IEnumerator.
    /// </summary>
#if COROUTINEHOST_TASKS
    public static Task<object> Host(IEnumerator<object> coroutine)
#else
    public static void Host(IEnumerator<object> coroutine)
#endif
    {
#if COROUTINEHOST_TASKS
        return CrossThreadHost.QueueCoroutine(coroutine);
#else
        CrossThreadHost.QueueCoroutine(coroutine);
#endif
    }

    public class CoroutineHostInstance : MonoBehaviour {
        private Queue<IEnumerator<object>> coroutineQueue;
#if COROUTINEHOST_TASKS
        private Dictionary<IEnumerator<object>, TaskCompletionSource<object>> taskCompletionSources;
        private List<IEnumerator<object>> rememberRemoval;
#endif

#if COROUTINEHOST_TASKS
        public Task<object> QueueCoroutine(IEnumerator<object> coroutine)
#else
    private void QueueCoroutine(IEnumerator<object> coroutine)
#endif
        {
            lock (coroutineQueue) {
                coroutineQueue.Enqueue(coroutine);
#if COROUTINEHOST_TASKS
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                taskCompletionSources.Add(coroutine, tcs);
                return tcs.Task;
#endif
            }
        }

        public void Awake() {
            coroutineQueue = new Queue<IEnumerator<object>>();
#if COROUTINEHOST_TASKS
            taskCompletionSources = new Dictionary<IEnumerator<object>, TaskCompletionSource<object>>();
            rememberRemoval = new List<IEnumerator<object>>();
#endif
        }

        private void Update() {
            lock (coroutineQueue) {
                while (coroutineQueue.Count > 0) {
                    //Debug.Log("CH : StartCoroutine");
                    StartCoroutine(coroutineQueue.Dequeue());
                }
            }
#if COROUTINEHOST_TASKS
            foreach (KeyValuePair<IEnumerator<object>, TaskCompletionSource<object>> kvp in taskCompletionSources) {
                if (kvp.Key != null && kvp.Key.MoveNext() == false) {
                    kvp.Value.SetResult(kvp.Key.Current);
                    rememberRemoval.Add(kvp.Key);
                }
            }
            if (rememberRemoval.Count > 0) {
                foreach (IEnumerator<object> ie in rememberRemoval) {
                    taskCompletionSources.Remove(ie);
                    //Debug.Log("Removing...");
                }
                rememberRemoval.Clear();
            }
#endif
        }
    }
}
