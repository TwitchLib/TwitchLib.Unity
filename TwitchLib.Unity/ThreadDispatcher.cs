using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TwitchLib.Unity
{
	public class ThreadDispatcher : MonoBehaviour
	{
		private static ThreadDispatcher _instance;

		private static Queue<Action> _executionQueue = new Queue<Action>();

		/// <summary>
		/// Ensures a thread dispatcher is created if there is none.
		/// </summary>
		public static void EnsureCreated([CallerMemberName] string callerMemberName = null)
		{
			if (!Application.isPlaying)
				return;
			if (_instance == null || _instance.gameObject == null)
				_instance = CreateThreadDispatcherSingleton(callerMemberName);
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

		private static ThreadDispatcher CreateThreadDispatcherSingleton(string callerMemberName)
		{
			if (!UnityThread.CurrentIsMainThread)
				throw new InvalidOperationException($"The {callerMemberName} can only be created from the main thread. Did you accidentally delete the " + nameof(ThreadDispatcher) + " in your scene?");

			ThreadDispatcher threadDispatcher = new GameObject(nameof(ThreadDispatcher)).AddComponent<ThreadDispatcher>();
			DontDestroyOnLoad(threadDispatcher);
			return threadDispatcher;
		}
	}
}
