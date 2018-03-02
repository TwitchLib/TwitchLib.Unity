using System;
using System.Net;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Interfaces;
using TwitchLib.PubSub.Events;

public class TwitchPubSubUnity : TwitchPubSub, ITwitchPubSub
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

    public TwitchPubSubUnity(EndPoint proxy = null) : base(null,proxy)
    {
        CoroutineHost.PrepareCrossThread();
        base.OnPubSubServiceConnected += ((object sender, EventArgs e) => { CoroutineHost.Host(() => OnPubSubServiceConnected?.Invoke(sender, e)); });
        base.OnPubSubServiceError += ((object sender, OnPubSubServiceErrorArgs e) => { CoroutineHost.Host(() => OnPubSubServiceError?.Invoke(sender, e)); });
        base.OnPubSubServiceClosed += ((object sender, EventArgs e) => { CoroutineHost.Host(() => OnPubSubServiceClosed?.Invoke(sender, e)); });
        base.OnListenResponse += ((object sender, OnListenResponseArgs e) => { CoroutineHost.Host(() => OnListenResponse?.Invoke(sender, e)); });
        base.OnTimeout += ((object sender, OnTimeoutArgs e) => { CoroutineHost.Host(() => OnTimeout?.Invoke(sender, e)); });
        base.OnBan += ((object sender, OnBanArgs e) => { CoroutineHost.Host(() => OnBan?.Invoke(sender, e)); });
        base.OnUnban += ((object sender, OnUnbanArgs e) => { CoroutineHost.Host(() => OnUnban?.Invoke(sender, e)); });
        base.OnUntimeout += ((object sender, OnUntimeoutArgs e) => { CoroutineHost.Host(() => OnUntimeout?.Invoke(sender, e)); });
        base.OnHost += ((object sender, OnHostArgs e) => { CoroutineHost.Host(() => OnHost?.Invoke(sender, e)); });
        base.OnSubscribersOnly += ((object sender, OnSubscribersOnlyArgs e) => { CoroutineHost.Host(() => OnSubscribersOnly?.Invoke(sender, e)); });
        base.OnSubscribersOnlyOff += ((object sender, OnSubscribersOnlyOffArgs e) => { CoroutineHost.Host(() => OnSubscribersOnlyOff?.Invoke(sender, e)); });
        base.OnClear += ((object sender, OnClearArgs e) => { CoroutineHost.Host(() => OnClear?.Invoke(sender, e)); });
        base.OnEmoteOnly += ((object sender, OnEmoteOnlyArgs e) => { CoroutineHost.Host(() => OnEmoteOnly?.Invoke(sender, e)); });
        base.OnEmoteOnlyOff += ((object sender, OnEmoteOnlyOffArgs e) => { CoroutineHost.Host(() => OnEmoteOnlyOff?.Invoke(sender, e)); });
        base.OnR9kBeta += ((object sender, OnR9kBetaArgs e) => { CoroutineHost.Host(() => OnR9kBeta?.Invoke(sender, e)); });
        base.OnR9kBetaOff += ((object sender, OnR9kBetaOffArgs e) => { CoroutineHost.Host(() => OnR9kBetaOff?.Invoke(sender, e)); });
        base.OnBitsReceived += ((object sender, OnBitsReceivedArgs e) => { CoroutineHost.Host(() => OnBitsReceived?.Invoke(sender, e)); });
        base.OnStreamUp += ((object sender, OnStreamUpArgs arg) => { CoroutineHost.Host(() => OnStreamUp(sender, arg)); });
        base.OnStreamDown += ((object sender, OnStreamDownArgs e) => { CoroutineHost.Host(() => OnStreamDown?.Invoke(sender, e)); });
        base.OnViewCount += ((object sender, OnViewCountArgs e) => { CoroutineHost.Host(() => OnViewCount?.Invoke(sender, e)); });
        base.OnWhisper += ((object sender, OnWhisperArgs e) => { CoroutineHost.Host(() => OnWhisper?.Invoke(sender, e)); });
        base.OnChannelSubscription += ((object sender, OnChannelSubscriptionArgs e) => { CoroutineHost.Host(() => OnChannelSubscription?.Invoke(sender, e)); });
    }
}
