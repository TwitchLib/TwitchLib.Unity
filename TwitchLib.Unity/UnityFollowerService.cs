using System;
using UnityEngine;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.FollowerService;

namespace TwitchLib.Unity
{
    public class UnityFollowerService : FollowerService
    {
        #region Events
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnServiceStartedArgs> OnServiceStarted;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnServiceStoppedArgs> OnServiceStopped;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnNewFollowersDetectedArgs> OnNewFollowersDetected;
        #endregion

        public UnityFollowerService(ITwitchAPI api, int checkIntervalSeconds = 60, int queryCount = 25) : base(api, checkIntervalSeconds, queryCount)
        {
            ThreadDispatcher.EnsureCreated();

            base.OnServiceStarted += ((object sender, OnServiceStartedArgs e) => { ThreadDispatcher.Enqueue(() => OnServiceStarted?.Invoke(sender, e)); });
            base.OnServiceStopped += ((object sender, OnServiceStoppedArgs e) => { ThreadDispatcher.Enqueue(() => OnServiceStopped?.Invoke(sender, e)); });
            base.OnNewFollowersDetected += ((object sender, OnNewFollowersDetectedArgs e) => { ThreadDispatcher.Enqueue(() => OnNewFollowersDetected?.Invoke(sender, e)); });
        }
    }
}