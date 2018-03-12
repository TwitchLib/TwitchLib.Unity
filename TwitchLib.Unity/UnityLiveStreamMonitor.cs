using System;
using System.Net;
using UnityEngine;
using TwitchLib.Api;
using TwitchLib.Api.Enums;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Exceptions;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace TwitchLib.Unity {
    public class UnityLiveStreamMonitor : LiveStreamMonitor {
        private readonly GameObject _threadDispatcher;

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

        public UnityFollowerService(ITwitchAPI api, int checkIntervalSeconds = 60, bool checkStatusOnStart = true, bool invokeEventsOnStart = false) : base(api, checkIntervalSeconds, checkStatusOnStart, invokeEventsOnStart) {
            _threadDispatcher = new GameObject("ThreadDispatcher");
            _threadDispatcher.AddComponent<ThreadDispatcher>();
            UnityEngine.Object.DontDestroyOnLoad(_threadDispatcher);

            base.OnStreamOnline += ((object sender, OnStreamOnlineArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnStreamOnline?.Invoke(sender, e)); });
            base.OnStreamOffline += ((object sender, OnStreamOfflineArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnStreamOffline?.Invoke(sender, e)); });
            base.OnStreamUpdate += ((object sender, OnStreamUpdateArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnStreamUpdate?.Invoke(sender, e)); });
            base.OnStreamMonitorStarted += ((object sender, OnStreamMonitorStartedArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnStreamMonitorStarted?.Invoke(sender, e)); });
            base.OnStreamMonitorEnded += ((object sender, OnStreamMonitorEndedArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnStreamMonitorEnded?.Invoke(sender, e)); });
            base.OnStreamsSet += ((object sender, OnStreamsSetArgs e) => { ThreadDispatcher.Instance().Enqueue(() => OnStreamsSet?.Invoke(sender, e)); });
        }
    }
}