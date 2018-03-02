using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

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
        CoroutineHostInstance instance = coroutineHost.AddComponent<CoroutineHostInstance>();
        GameObject.DontDestroyOnLoad(coroutineHost);
        return instance;
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
    public static Task Host(Action action)
    {
        return Host(ConvertActionToCoroutine(action));
    }

    private static IEnumerator ConvertActionToCoroutine(Action action) {
        action.Invoke();
        yield break;
    }

    /// <summary>
    /// Think of this as just StartCoroutine() that is able to use from outside of MonoBehaviour or static methods.
    /// </summary>
    public static Task Host(IEnumerator coroutine)
    {
        return CrossThreadHost.QueueCoroutine(YieldConvert(coroutine));
    }

    // I am not sure if this costs 1 more frame for all lambdas or not?
    private static IEnumerator<object> YieldConvert(IEnumerator coroutine) {
        yield return coroutine;
    }
}
