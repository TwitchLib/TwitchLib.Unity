using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TwitchLib.Unity
{
    public class ThreadDispatcher : MonoBehaviour
    {
        private static ThreadDispatcher _instance;

        private static Queue<Action> _executionQueue = new Queue<Action>();

        public static void EnsureCreated()
        {
            if (_instance == null || _instance.gameObject == null)
                _instance = CreateThreadDispatcherSingleton();
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        public static void Enqueue(Action action)
        {
            _executionQueue.Enqueue(action);
        }

        private void Update()
        {
            //storing the count here instead of locking the queue so we don't end up with a deadlock when one of the actions queues another action
            int count = _executionQueue.Count;
            for (int i = 0; i < count; i++)
                _executionQueue.Dequeue().Invoke();
        }

        private static ThreadDispatcher CreateThreadDispatcherSingleton()
        {
            ThreadDispatcher threadDispatcher = new GameObject(nameof(ThreadDispatcher)).AddComponent<ThreadDispatcher>();
            DontDestroyOnLoad(threadDispatcher);
            return threadDispatcher;
        }
    }
}
