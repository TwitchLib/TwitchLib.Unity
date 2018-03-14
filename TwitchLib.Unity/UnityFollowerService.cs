using System;
using UnityEngine;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.FollowerService;

namespace TwitchLib.Unity
{
    public class UnityFollowerService : FollowerService
    {
        private readonly GameObject _threadDispatcher;

        #region Events
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnServiceStartedArgs> OnServiceStarted;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnServiceStoppedArgs> OnServiceStopped;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnNewFollowersDetectedArgs> OnNewFollowersDetected;
        #endregion

        public UnityFollowerService(ITwitchAPI api, int checkIntervalSeconds = 60, int queryCount = 25) : base(api, checkIntervalSeconds, queryCount) {
            _threadDispatcher = new GameObject("UnityFollowerServiceThreadDispatcher");
            _threadDispatcher.AddComponent<ThreadDispatcher>();
            UnityEngine.Object.DontDestroyOnLoad(_threadDispatcher);

            base.OnServiceStarted += ((object sender, OnServiceStartedArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnServiceStarted?.Invoke(sender, e)); });
            base.OnServiceStopped += ((object sender, OnServiceStoppedArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnServiceStopped?.Invoke(sender, e)); });
            base.OnNewFollowersDetected += ((object sender, OnNewFollowersDetectedArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnNewFollowersDetected?.Invoke(sender, e)); });
        }
    }
}