using System;
using UnityEngine;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace TwitchLib.Unity
{
    public class UnityLiveStreamMonitor : LiveStreamMonitor
    {
        #region EVENTS
        /// <summary>Event fires when Stream goes online</summary>
        public new event EventHandler<OnStreamOnlineArgs> OnStreamOnline;
        /// <summary>Event fires when Stream goes offline</summary>
        public new event EventHandler<OnStreamOfflineArgs> OnStreamOffline;
        /// <summary>Event fires when Stream gets updated</summary>
        public new event EventHandler<OnStreamUpdateArgs> OnStreamUpdate;
        /// <summary>Event fires when service stops.</summary>
        public new event EventHandler<OnStreamMonitorStartedArgs> OnStreamMonitorStarted;
        /// <summary>Event fires when service starts.</summary>
        public new event EventHandler<OnStreamMonitorEndedArgs> OnStreamMonitorEnded;
        /// <summary>Event fires when channels to monitor are intitialized.</summary>
        public new event EventHandler<OnStreamsSetArgs> OnStreamsSet;
        #endregion

        public UnityLiveStreamMonitor(ITwitchAPI api, int checkIntervalSeconds = 60, bool checkStatusOnStart = true, bool invokeEventsOnStart = false) : base(api, checkIntervalSeconds, checkStatusOnStart, invokeEventsOnStart)
        {
            ThreadDispatcher.EnsureCreated();

            base.OnStreamOnline += ((object sender, OnStreamOnlineArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamOnline?.Invoke(sender, e)); });
            base.OnStreamOffline += ((object sender, OnStreamOfflineArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamOffline?.Invoke(sender, e)); });
            base.OnStreamUpdate += ((object sender, OnStreamUpdateArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamUpdate?.Invoke(sender, e)); });
            base.OnStreamMonitorStarted += ((object sender, OnStreamMonitorStartedArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamMonitorStarted?.Invoke(sender, e)); });
            base.OnStreamMonitorEnded += ((object sender, OnStreamMonitorEndedArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamMonitorEnded?.Invoke(sender, e)); });
            base.OnStreamsSet += ((object sender, OnStreamsSetArgs e) => {  ThreadDispatcher.Enqueue(() => OnStreamsSet?.Invoke(sender, e)); });
        }
    }
}