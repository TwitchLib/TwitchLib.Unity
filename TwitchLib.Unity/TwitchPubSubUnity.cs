using System;
using System.Net;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Interfaces;
using TwitchLib.PubSub.Events;
using UnityEngine;

namespace TwitchLib.Unity
{
    public class TwitchPubSubUnity : TwitchPubSub, ITwitchPubSub
    {
        private readonly GameObject _threadDispatcher;

        #region Events
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler OnPubSubServiceConnected;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnPubSubServiceErrorArgs> OnPubSubServiceError;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler OnPubSubServiceClosed;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnListenResponseArgs> OnListenResponse;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnTimeoutArgs> OnTimeout;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnBanArgs> OnBan;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnUnbanArgs> OnUnban;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnUntimeoutArgs> OnUntimeout;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnHostArgs> OnHost;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnSubscribersOnlyArgs> OnSubscribersOnly;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnSubscribersOnlyOffArgs> OnSubscribersOnlyOff;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnClearArgs> OnClear;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnEmoteOnlyArgs> OnEmoteOnly;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnEmoteOnlyOffArgs> OnEmoteOnlyOff;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnR9kBetaArgs> OnR9kBeta;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnR9kBetaOffArgs> OnR9kBetaOff;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnBitsReceivedArgs> OnBitsReceived;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnStreamUpArgs> OnStreamUp;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnStreamDownArgs> OnStreamDown;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnViewCountArgs> OnViewCount;
        /// <summary>EventHandler for named event.</summary>
        public new event EventHandler<OnWhisperArgs> OnWhisper;
        /// <summary>EventHandler for channel subscriptions.</summary>
        public new event EventHandler<OnChannelSubscriptionArgs> OnChannelSubscription;
        #endregion

        public TwitchPubSubUnity(EndPoint proxy = null) : base(null, proxy)
        {
            _threadDispatcher = new GameObject("TwitchPubSubUnityDispatcher");
            _threadDispatcher.AddComponent<TwitchLibUnityThreadDispatcher>();
            UnityEngine.Object.DontDestroyOnLoad(_threadDispatcher);

            base.OnPubSubServiceConnected += ((object sender, EventArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnPubSubServiceConnected?.Invoke(sender, e)); });
            base.OnPubSubServiceError += ((object sender, OnPubSubServiceErrorArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnPubSubServiceError?.Invoke(sender, e)); });
            base.OnPubSubServiceClosed += ((object sender, EventArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnPubSubServiceClosed?.Invoke(sender, e)); });
            base.OnListenResponse += ((object sender, OnListenResponseArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnListenResponse?.Invoke(sender, e)); });
            base.OnTimeout += ((object sender, OnTimeoutArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnTimeout?.Invoke(sender, e)); });
            base.OnBan += ((object sender, OnBanArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnBan?.Invoke(sender, e)); });
            base.OnUnban += ((object sender, OnUnbanArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnUnban?.Invoke(sender, e)); });
            base.OnUntimeout += ((object sender, OnUntimeoutArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnUntimeout?.Invoke(sender, e)); });
            base.OnHost += ((object sender, OnHostArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnHost?.Invoke(sender, e)); });
            base.OnSubscribersOnly += ((object sender, OnSubscribersOnlyArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnSubscribersOnly?.Invoke(sender, e)); });
            base.OnSubscribersOnlyOff += ((object sender, OnSubscribersOnlyOffArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnSubscribersOnlyOff?.Invoke(sender, e)); });
            base.OnClear += ((object sender, OnClearArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnClear?.Invoke(sender, e)); });
            base.OnEmoteOnly += ((object sender, OnEmoteOnlyArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnEmoteOnly?.Invoke(sender, e)); });
            base.OnEmoteOnlyOff += ((object sender, OnEmoteOnlyOffArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnEmoteOnlyOff?.Invoke(sender, e)); });
            base.OnR9kBeta += ((object sender, OnR9kBetaArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnR9kBeta?.Invoke(sender, e)); });
            base.OnR9kBetaOff += ((object sender, OnR9kBetaOffArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnR9kBetaOff?.Invoke(sender, e)); });
            base.OnBitsReceived += ((object sender, OnBitsReceivedArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnBitsReceived?.Invoke(sender, e)); });
            base.OnStreamUp += ((object sender, OnStreamUpArgs arg) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnStreamUp(sender, arg)); });
            base.OnStreamDown += ((object sender, OnStreamDownArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnStreamDown?.Invoke(sender, e)); });
            base.OnViewCount += ((object sender, OnViewCountArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnViewCount?.Invoke(sender, e)); });
            base.OnWhisper += ((object sender, OnWhisperArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnWhisper?.Invoke(sender, e)); });
            base.OnChannelSubscription += ((object sender, OnChannelSubscriptionArgs e) => { TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => OnChannelSubscription?.Invoke(sender, e)); });
        }
    }
}
