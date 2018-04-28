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
    public class Api : TwitchAPI, ITwitchAPI
    {
        public Api() : base()
        {
        }

        public void Invoke<T>(Func<Task<T>> func, Action<T> action = null)
        {
            ThreadDispatcher.EnsureCreated();
            Task.Run(func).ContinueWith((x) =>
            {               
                var value = x.Result;
                ThreadDispatcher.Enqueue(() => action?.Invoke(value));
            });
        }

        public void Invoke(Func<Task> func, Action action = null)
        {
            ThreadDispatcher.EnsureCreated();
            Task.Run(func).ContinueWith((x) =>
            {
                x.Wait();
                ThreadDispatcher.Enqueue(() => action?.Invoke());
            });
        }

        /// <summary>
        /// Invokes a function async, and waits for a response before continuing. 
        /// </summary>
        public IEnumerator InvokeAsync<T>(Func<Task<T>> func, Action<T> action = null)
        {
            bool requestCompleted = false;
            Invoke(func, (result) =>
            {
                action?.Invoke(result);
                requestCompleted = true;
            });
            yield return new WaitUntil(() => requestCompleted);
        }

        /// <summary>
        /// Invokes a function async, and waits for the request to complete before continuing. 
        /// </summary>
        public IEnumerator InvokeAsync(Func<Task> func)
        {
            bool requestCompleted = false;
            Invoke(func, () => requestCompleted = true);
            yield return new WaitUntil(() => requestCompleted);
        }
    }
}
