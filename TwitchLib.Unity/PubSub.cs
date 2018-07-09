using System;
using System.Net;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Interfaces;

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

        public PubSub(EndPoint proxy = null) : base(null, proxy)
        {
            ThreadDispatcher.EnsureCreated();

            base.OnPubSubServiceConnected += (sender, e) => { ThreadDispatcher.Enqueue(() => OnPubSubServiceConnected?.Invoke(sender, e)); };
            base.OnPubSubServiceError += (sender, e) => { ThreadDispatcher.Enqueue(() => OnPubSubServiceError?.Invoke(sender, e)); };
            base.OnPubSubServiceClosed += (sender, e) => { ThreadDispatcher.Enqueue(() => OnPubSubServiceClosed?.Invoke(sender, e)); };
            base.OnListenResponse += (sender, e) => { ThreadDispatcher.Enqueue(() => OnListenResponse?.Invoke(sender, e)); };
            base.OnTimeout += (sender, e) => { ThreadDispatcher.Enqueue(() => OnTimeout?.Invoke(sender, e)); };
            base.OnBan += (sender, e) => { ThreadDispatcher.Enqueue(() => OnBan?.Invoke(sender, e)); };
            base.OnUnban += (sender, e) => { ThreadDispatcher.Enqueue(() => OnUnban?.Invoke(sender, e)); };
            base.OnUntimeout += (sender, e) => { ThreadDispatcher.Enqueue(() => OnUntimeout?.Invoke(sender, e)); };
            base.OnHost += (sender, e) => { ThreadDispatcher.Enqueue(() => OnHost?.Invoke(sender, e)); };
            base.OnSubscribersOnly += (sender, e) => { ThreadDispatcher.Enqueue(() => OnSubscribersOnly?.Invoke(sender, e)); };
            base.OnSubscribersOnlyOff += (sender, e) => { ThreadDispatcher.Enqueue(() => OnSubscribersOnlyOff?.Invoke(sender, e)); };
            base.OnClear += (sender, e) => { ThreadDispatcher.Enqueue(() => OnClear?.Invoke(sender, e)); };
            base.OnEmoteOnly += (sender, e) => { ThreadDispatcher.Enqueue(() => OnEmoteOnly?.Invoke(sender, e)); };
            base.OnEmoteOnlyOff += (sender, e) => { ThreadDispatcher.Enqueue(() => OnEmoteOnlyOff?.Invoke(sender, e)); };
            base.OnR9kBeta += (sender, e) => { ThreadDispatcher.Enqueue(() => OnR9kBeta?.Invoke(sender, e)); };
            base.OnR9kBetaOff += (sender, e) => { ThreadDispatcher.Enqueue(() => OnR9kBetaOff?.Invoke(sender, e)); };
            base.OnBitsReceived += (sender, e) => { ThreadDispatcher.Enqueue(() => OnBitsReceived?.Invoke(sender, e)); };
            base.OnStreamUp += (sender, arg) => { ThreadDispatcher.Enqueue(() => OnStreamUp(sender, arg)); };
            base.OnStreamDown += (sender, e) => { ThreadDispatcher.Enqueue(() => OnStreamDown?.Invoke(sender, e)); };
            base.OnViewCount += (sender, e) => { ThreadDispatcher.Enqueue(() => OnViewCount?.Invoke(sender, e)); };
            base.OnWhisper += (sender, e) => { ThreadDispatcher.Enqueue(() => OnWhisper?.Invoke(sender, e)); };
            base.OnChannelSubscription += (sender, e) => { ThreadDispatcher.Enqueue(() => OnChannelSubscription?.Invoke(sender, e)); };
        }
    }
}
