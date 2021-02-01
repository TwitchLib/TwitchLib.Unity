using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TwitchLib.Unity
{
	public class ThreadDispatcher : MonoBehaviour
	{
		private static ThreadDispatcher _instance;

		private static ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

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
			while (!_executionQueue.IsEmpty)
			{
				Action action;
				if (_executionQueue.TryDequeue(out action))
				{
					action.Invoke();
				}
			}
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
