using System;
using System.Net;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Interfaces;
using TwitchLib.PubSub.Events;
using UnityEngine;

namespace TwitchLib.Unity
{
    public class PubSub : TwitchPubSub, ITwitchPubSub
    {
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

        public PubSub(EndPoint proxy = null) : base(null)
        {
            ThreadDispatcher.EnsureCreated();

            base.OnPubSubServiceConnected += ((object sender, EventArgs e) => { ThreadDispatcher.Enqueue(() => OnPubSubServiceConnected?.Invoke(sender, e)); });
            base.OnPubSubServiceError += ((object sender, OnPubSubServiceErrorArgs e) => { ThreadDispatcher.Enqueue(() => OnPubSubServiceError?.Invoke(sender, e)); });
            base.OnPubSubServiceClosed += ((object sender, EventArgs e) => { ThreadDispatcher.Enqueue(() => OnPubSubServiceClosed?.Invoke(sender, e)); });
            base.OnListenResponse += ((object sender, OnListenResponseArgs e) => { ThreadDispatcher.Enqueue(() => OnListenResponse?.Invoke(sender, e)); });
            base.OnTimeout += ((object sender, OnTimeoutArgs e) => { ThreadDispatcher.Enqueue(() => OnTimeout?.Invoke(sender, e)); });
            base.OnBan += ((object sender, OnBanArgs e) => { ThreadDispatcher.Enqueue(() => OnBan?.Invoke(sender, e)); });
            base.OnUnban += ((object sender, OnUnbanArgs e) => { ThreadDispatcher.Enqueue(() => OnUnban?.Invoke(sender, e)); });
            base.OnUntimeout += ((object sender, OnUntimeoutArgs e) => { ThreadDispatcher.Enqueue(() => OnUntimeout?.Invoke(sender, e)); });
            base.OnHost += ((object sender, OnHostArgs e) => { ThreadDispatcher.Enqueue(() => OnHost?.Invoke(sender, e)); });
            base.OnSubscribersOnly += ((object sender, OnSubscribersOnlyArgs e) => { ThreadDispatcher.Enqueue(() => OnSubscribersOnly?.Invoke(sender, e)); });
            base.OnSubscribersOnlyOff += ((object sender, OnSubscribersOnlyOffArgs e) => { ThreadDispatcher.Enqueue(() => OnSubscribersOnlyOff?.Invoke(sender, e)); });
            base.OnClear += ((object sender, OnClearArgs e) => { ThreadDispatcher.Enqueue(() => OnClear?.Invoke(sender, e)); });
            base.OnEmoteOnly += ((object sender, OnEmoteOnlyArgs e) => { ThreadDispatcher.Enqueue(() => OnEmoteOnly?.Invoke(sender, e)); });
            base.OnEmoteOnlyOff += ((object sender, OnEmoteOnlyOffArgs e) => { ThreadDispatcher.Enqueue(() => OnEmoteOnlyOff?.Invoke(sender, e)); });
            base.OnR9kBeta += ((object sender, OnR9kBetaArgs e) => { ThreadDispatcher.Enqueue(() => OnR9kBeta?.Invoke(sender, e)); });
            base.OnR9kBetaOff += ((object sender, OnR9kBetaOffArgs e) => { ThreadDispatcher.Enqueue(() => OnR9kBetaOff?.Invoke(sender, e)); });
            base.OnBitsReceived += ((object sender, OnBitsReceivedArgs e) => { ThreadDispatcher.Enqueue(() => OnBitsReceived?.Invoke(sender, e)); });
            base.OnStreamUp += ((object sender, OnStreamUpArgs arg) => { ThreadDispatcher.Enqueue(() => OnStreamUp(sender, arg)); });
            base.OnStreamDown += ((object sender, OnStreamDownArgs e) => { ThreadDispatcher.Enqueue(() => OnStreamDown?.Invoke(sender, e)); });
            base.OnViewCount += ((object sender, OnViewCountArgs e) => { ThreadDispatcher.Enqueue(() => OnViewCount?.Invoke(sender, e)); });
            base.OnWhisper += ((object sender, OnWhisperArgs e) => { ThreadDispatcher.Enqueue(() => OnWhisper?.Invoke(sender, e)); });
            base.OnChannelSubscription += ((object sender, OnChannelSubscriptionArgs e) => { ThreadDispatcher.Enqueue(() => OnChannelSubscription?.Invoke(sender, e)); });
        }
    }
}
