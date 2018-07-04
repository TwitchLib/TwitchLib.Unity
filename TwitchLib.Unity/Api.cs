using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using UnityEngine;

namespace TwitchLib.Unity
{
    public class Api : TwitchAPI
    {
        /// <summary>
        /// Waits for the task to execute and invokes the provided action with the task's result on Unity's main thread.
        /// </summary>
        public void Invoke<T>(Task<T> func, Action<T> action)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (action == null) throw new ArgumentNullException(nameof(action));

            InvokeInternal(func, action);
        }

        /// <summary>
        /// Waits for the task to execute and invokes the provided action on Unity's main thread.
        /// </summary>
        public void Invoke(Task func, Action action)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (action == null) throw new ArgumentNullException(nameof(action));

            InvokeInternal(func, action);
        }

        /// <summary>
        /// Invokes a function async, and waits for a response before continuing. 
        /// </summary>
        public IEnumerator InvokeAsync<T>(Task<T> func, Action<T> action)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (action == null) throw new ArgumentNullException(nameof(action));

            bool requestCompleted = false;
            InvokeInternal(func, (result) =>
            {
                action.Invoke(result);
                requestCompleted = true;
            });
            yield return new WaitUntil(() => requestCompleted);
        }

        /// <summary>
        /// Invokes a function async, and waits for the request to complete before continuing. 
        /// </summary>
        public IEnumerator InvokeAsync(Task func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            bool requestCompleted = false;
            InvokeInternal(func, () => requestCompleted = true);
            yield return new WaitUntil(() => requestCompleted);
        }

        private void InvokeInternal(Task func, Action action)
        {
            ThreadDispatcher.EnsureCreated();
            func.ContinueWith((x) =>
            {
                x.Wait();
                ThreadDispatcher.Enqueue(() => action.Invoke());
            });
        }

        private void InvokeInternal<T>(Task<T> func, Action<T> action)
        {
            ThreadDispatcher.EnsureCreated();
            func.ContinueWith((x) =>
            {
                var value = x.Result;
                ThreadDispatcher.Enqueue(() => action.Invoke(value));
            });
        }
    }
}
