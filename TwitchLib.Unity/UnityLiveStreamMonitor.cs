using System;
using UnityEngine;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Api.Services.Events;

namespace TwitchLib.Unity
{
    public class UnityLiveStreamMonitor : LiveStreamMonitorService
    {
        #region EVENTS
        /// <summary>Event fires when Stream goes online</summary>
        public new event EventHandler<OnStreamOnlineArgs> OnStreamOnline;
        /// <summary>Event fires when Stream goes offline</summary>
        public new event EventHandler<OnStreamOfflineArgs> OnStreamOffline;
        /// <summary>Event fires when Stream gets updated</summary>
        public new event EventHandler<OnStreamUpdateArgs> OnStreamUpdate;
        /// <summary>Event fires when service stops.</summary>
        public new event EventHandler<OnServiceStartedArgs> OnServiceStarted;
        /// <summary>Event fires when service starts.</summary>
        public new event EventHandler<OnServiceStoppedArgs> OnServiceStopped;
        /// <summary>Event fires when channels to monitor are intitialized.</summary>
        public new event EventHandler<OnChannelsSetArgs> OnChannelsSet;
        #endregion

        public UnityLiveStreamMonitor(ITwitchAPI api, int checkIntervalSeconds = 60, int maxStreamRequestCountPerRequest = 100) : base(api, checkIntervalSeconds, maxStreamRequestCountPerRequest)
        {
            ThreadDispatcher.EnsureCreated();

            base.OnStreamOnline += ((object sender, OnStreamOnlineArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamOnline?.Invoke(sender, e)); });
            base.OnStreamOffline += ((object sender, OnStreamOfflineArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamOffline?.Invoke(sender, e)); });
            base.OnStreamUpdate += ((object sender, OnStreamUpdateArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamUpdate?.Invoke(sender, e)); });
            base.OnServiceStarted += ((object sender, OnServiceStartedArgs e) => {  ThreadDispatcher.Enqueue(() => OnServiceStarted?.Invoke(sender, e)); });
            base.OnServiceStopped += ((object sender, OnServiceStoppedArgs e) => {  ThreadDispatcher.Enqueue(() => OnServiceStopped?.Invoke(sender, e)); });
            base.OnChannelsSet += ((object sender, OnChannelsSetArgs e) => {  ThreadDispatcher.Enqueue(() => OnChannelsSet?.Invoke(sender, e)); });
        }


	}
}