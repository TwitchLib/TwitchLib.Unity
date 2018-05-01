using System.Threading;
using UnityEngine;

namespace TwitchLib.Unity
{
    public class UnityThread : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetMainUnityThread() => _mainThread = Thread.CurrentThread;

        private static Thread _mainThread;

        public static Thread Main => _mainThread;
        public static bool CurrentIsMainThread => _mainThread == null || _mainThread == Thread.CurrentThread;
    }
}
