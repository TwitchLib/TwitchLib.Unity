using System;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events.Client;

public class TwitchClientUnity : TwitchClient, ITwitchClient
{
    #region Events
    /// <summary>
    /// Fires whenever a log write happens.
    /// </summary>
    public new event EventHandler<OnLogArgs> OnLog;

    /// <summary>
    /// Fires when client connects to Twitch.
    /// </summary>
    public new event EventHandler<OnConnectedArgs> OnConnected;

    /// <summary>
    /// Fires when client joins a channel.
    /// </summary>
    public new event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;

    /// <summary>
    /// Fires on logging in with incorrect details, returns ErrorLoggingInException.
    /// </summary>
    public new event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;

    /// <summary>
    /// Fires when connecting and channel state is changed, returns ChannelState.
    /// </summary>
    public new event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;

    /// <summary>
    /// Fires when a user state is received, returns UserState.
    /// </summary>
    public new event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;

    /// <summary>
    /// Fires when a new chat message arrives, returns ChatMessage.
    /// </summary>
    public new event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

    /// <summary>
    /// Fires when a new whisper arrives, returns WhisperMessage.
    /// </summary>
    public new event EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;

    /// <summary>
    /// Fires when a chat message is sent, returns username, channel and message.
    /// </summary>
    public new event EventHandler<OnMessageSentArgs> OnMessageSent;

    /// <summary>
    /// Fires when a whisper message is sent, returns username and message.
    /// </summary>
    public new event EventHandler<OnWhisperSentArgs> OnWhisperSent;

    /// <summary>
    /// Fires when command (uses custom chat command identifier) is received, returns channel, command, ChatMessage, arguments as string, arguments as list.
    /// </summary>
    public new event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;

    /// <summary>
    /// Fires when command (uses custom whisper command identifier) is received, returns command, Whispermessage.
    /// </summary>
    public new event EventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;

    /// <summary>
    /// Fires when a new viewer/chatter joined the channel's chat room, returns username and channel.
    /// </summary>
    public new event EventHandler<OnUserJoinedArgs> OnUserJoined;

    /// <summary>
    /// Fires when a moderator joined the channel's chat room, returns username and channel.
    /// </summary>
    public new event EventHandler<OnModeratorJoinedArgs> OnModeratorJoined;

    /// <summary>
    /// Fires when a moderator joins the channel's chat room, returns username and channel.
    /// </summary>
    public new event EventHandler<OnModeratorLeftArgs> OnModeratorLeft;

    /// <summary>
    /// Fires when new subscriber is announced in chat, returns Subscriber.
    /// </summary>
    public new event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;

    /// <summary>
    /// Fires when current subscriber renews subscription, returns ReSubscriber.
    /// </summary>
    public new event EventHandler<OnReSubscriberArgs> OnReSubscriber;

    /// <summary>
    /// Fires when a hosted streamer goes offline and hosting is killed.
    /// </summary>
    public new event EventHandler OnHostLeft;

    /// <summary>
    /// Fires when Twitch notifies client of existing users in chat.
    /// </summary>
    public new event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;

    /// <summary>
    /// Fires when a PART message is received from Twitch regarding a particular viewer
    /// </summary>
    public new event EventHandler<OnUserLeftArgs> OnUserLeft;

    /// <summary>
    /// Fires when the joined channel begins hosting another channel.
    /// </summary>
    public new event EventHandler<OnHostingStartedArgs> OnHostingStarted;

    /// <summary>
    /// Fires when the joined channel quits hosting another channel.
    /// </summary>
    public new event EventHandler<OnHostingStoppedArgs> OnHostingStopped;

    /// <summary>
    /// Fires when bot has disconnected.
    /// </summary>
    public new event EventHandler<OnDisconnectedArgs> OnDisconnected;

    /// <summary>
    /// Forces when bot suffers conneciton error.
    /// </summary>
    public new event EventHandler<OnConnectionErrorArgs> OnConnectionError;

    /// <summary>
    /// Fires when a channel's chat is cleared.
    /// </summary>
    public new event EventHandler<OnChatClearedArgs> OnChatCleared;

    /// <summary>
    /// Fires when a viewer gets timedout by any moderator.
    /// </summary>
    public new event EventHandler<OnUserTimedoutArgs> OnUserTimedout;

    /// <summary>
    /// Fires when client successfully leaves a channel.
    /// </summary>
    public new event EventHandler<OnLeftChannelArgs> OnLeftChannel;

    /// <summary>
    /// Fires when a viewer gets banned by any moderator.
    /// </summary>
    public new event EventHandler<OnUserBannedArgs> OnUserBanned;

    /// <summary>
    /// Fires when a list of moderators is received.
    /// </summary>
    public new event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;

    /// <summary>
    /// Fires when confirmation of a chat color change request was received.
    /// </summary>
    public new event EventHandler<OnChatColorChangedArgs> OnChatColorChanged;

    /// <summary>
    /// Fires when data is either received or sent.
    /// </summary>
    public new event EventHandler<OnSendReceiveDataArgs> OnSendReceiveData;

    /// <summary>
    /// Fires when client receives notice that a joined channel is hosting another channel.
    /// </summary>
    public new event EventHandler<OnNowHostingArgs> OnNowHosting;

    /// <summary>
    /// Fires when the library detects another channel has started hosting the broadcaster's stream. MUST BE CONNECTED AS BROADCASTER.
    /// </summary>
    public new event EventHandler<OnBeingHostedArgs> OnBeingHosted;

    /// <summary>
    /// Fires when a raid notification is detected in chat
    /// </summary>
    public new event EventHandler<OnRaidNotificationArgs> OnRaidNotification;

    /// <summary>
    /// Fires when a subscription is gifted and announced in chat
    /// </summary>
    public new event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;

    /// <summary>Fires when TwitchClient attempts to host a channel it is in.</summary>
    public new event EventHandler OnSelfRaidError;

    /// <summary>Fires when TwitchClient receives generic no permission error from Twitch.</summary>
    public new event EventHandler OnNoPermissionError;

    /// <summary>Fires when newly raided channel is mature audience only.</summary>
    public new event EventHandler OnRaidedChannelIsMatureAudience;

    /// <summary>Fires when a ritual for a new chatter is received.</summary>
    public new event EventHandler<OnRitualNewChatterArgs> OnRitualNewChatter;

    /// <summary>Fires when the client was unable to join a channel.</summary>
    public new event EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;

    /// <summary>Fires when data is received from Twitch that is not able to be parsed.</summary>
    public new event EventHandler<OnUnaccountedForArgs> OnUnaccountedFor;
    #endregion
    public TwitchClientUnity(ILogger<TwitchClientUnity> logger) :base(logger)
   {
        CoroutineHost.PrepareCrossThread();
       // client = new TwitchClient(credentials, channel, chatCommandIdentifier,whisperCommandIdentifier,logging,logger,autoReListenOnExceptions);
        base.OnLog += ((object sender, OnLogArgs e) => { CoroutineHost.Host(() => OnLog?.Invoke(sender, e)); });
        base.OnConnected += ((object sender, OnConnectedArgs e) => { CoroutineHost.Host(() => OnConnected?.Invoke(sender, e)); });
        base.OnJoinedChannel += ((object sender, OnJoinedChannelArgs e) => { CoroutineHost.Host(() => OnJoinedChannel?.Invoke(sender, e)); });
        base.OnIncorrectLogin += ((object sender, OnIncorrectLoginArgs e) => { CoroutineHost.Host(() => OnIncorrectLogin?.Invoke(sender, e)); });
        base.OnChannelStateChanged += ((object sender, OnChannelStateChangedArgs e) => { CoroutineHost.Host(() => OnChannelStateChanged?.Invoke(sender, e)); });
        base.OnUserStateChanged += ((object sender, OnUserStateChangedArgs e) => { CoroutineHost.Host(() => OnUserStateChanged?.Invoke(sender, e)); });
        base.OnMessageReceived += ((object sender, OnMessageReceivedArgs e) => { CoroutineHost.Host(() => OnMessageReceived?.Invoke(sender, e)); });
        base.OnWhisperReceived += ((object sender, OnWhisperReceivedArgs e) => { CoroutineHost.Host(() => OnWhisperReceived?.Invoke(sender, e)); });
        base.OnMessageSent += ((object sender, OnMessageSentArgs e) => { CoroutineHost.Host(() => OnMessageSent?.Invoke(sender, e)); });
        base.OnWhisperSent += ((object sender, OnWhisperSentArgs e) => { CoroutineHost.Host(() => OnWhisperSent?.Invoke(sender, e)); });
        base.OnChatCommandReceived += ((object sender, OnChatCommandReceivedArgs e) => { CoroutineHost.Host(() => OnChatCommandReceived?.Invoke(sender, e)); });
        base.OnWhisperCommandReceived += ((object sender, OnWhisperCommandReceivedArgs e) => { CoroutineHost.Host(() => OnWhisperCommandReceived?.Invoke(sender, e)); });
        base.OnUserJoined += ((object sender, OnUserJoinedArgs e) => { CoroutineHost.Host(() => OnUserJoined?.Invoke(sender, e)); });
        base.OnModeratorJoined += ((object sender, OnModeratorJoinedArgs e) => { CoroutineHost.Host(() => OnModeratorJoined?.Invoke(sender, e)); });
        base.OnModeratorLeft += ((object sender, OnModeratorLeftArgs e) => { CoroutineHost.Host(() => OnModeratorLeft?.Invoke(sender, e)); });
        base.OnNewSubscriber += ((object sender, OnNewSubscriberArgs e) => { CoroutineHost.Host(() => OnNewSubscriber?.Invoke(sender, e)); });
        base.OnReSubscriber += ((object sender, OnReSubscriberArgs e) => { CoroutineHost.Host(() => OnReSubscriber?.Invoke(sender, e)); });
        base.OnHostLeft += ((object sender, EventArgs arg) => { CoroutineHost.Host(() => OnHostLeft(sender, arg)); });
        base.OnExistingUsersDetected += ((object sender, OnExistingUsersDetectedArgs e) => { CoroutineHost.Host(() => OnExistingUsersDetected?.Invoke(sender, e)); });
        base.OnUserLeft += ((object sender, OnUserLeftArgs e) => { CoroutineHost.Host(() => OnUserLeft?.Invoke(sender, e)); });
        base.OnHostingStarted += ((object sender, OnHostingStartedArgs e) => { CoroutineHost.Host(() => OnHostingStarted?.Invoke(sender, e)); });
        base.OnHostingStopped += ((object sender, OnHostingStoppedArgs e) => { CoroutineHost.Host(() => OnHostingStopped?.Invoke(sender, e)); });
        base.OnDisconnected += ((object sender, OnDisconnectedArgs e) => { CoroutineHost.Host(() => OnDisconnected?.Invoke(sender, e)); });
        base.OnConnectionError += ((object sender, OnConnectionErrorArgs e) => { CoroutineHost.Host(() => OnConnectionError?.Invoke(sender, e)); });
        base.OnChatCleared += ((object sender, OnChatClearedArgs e) => { CoroutineHost.Host(() => OnChatCleared?.Invoke(sender, e)); });
        base.OnUserTimedout += ((object sender, OnUserTimedoutArgs e) => { CoroutineHost.Host(() => OnUserTimedout?.Invoke(sender, e)); });
        base.OnLeftChannel += ((object sender, OnLeftChannelArgs e) => { CoroutineHost.Host(() => OnLeftChannel?.Invoke(sender, e)); });
        base.OnUserBanned += ((object sender, OnUserBannedArgs e) => { CoroutineHost.Host(() => OnUserBanned?.Invoke(sender, e)); });
        base.OnModeratorsReceived += ((object sender, OnModeratorsReceivedArgs e) => { CoroutineHost.Host(() => OnModeratorsReceived?.Invoke(sender, e)); });
        base.OnChatColorChanged += ((object sender, OnChatColorChangedArgs e) => { CoroutineHost.Host(() => OnChatColorChanged?.Invoke(sender, e)); });
        base.OnSendReceiveData += ((object sender, OnSendReceiveDataArgs e) => { CoroutineHost.Host(() => OnSendReceiveData?.Invoke(sender, e)); });
        base.OnNowHosting += ((object sender, OnNowHostingArgs e) => { CoroutineHost.Host(() => OnNowHosting?.Invoke(sender, e)); });
        base.OnBeingHosted += ((object sender, OnBeingHostedArgs e) => { CoroutineHost.Host(() => OnBeingHosted?.Invoke(sender, e)); });
        base.OnRaidNotification += ((object sender, OnRaidNotificationArgs e) => { CoroutineHost.Host(() => OnRaidNotification?.Invoke(sender, e)); });
        base.OnGiftedSubscription += ((object sender, OnGiftedSubscriptionArgs e) => { CoroutineHost.Host(() => OnGiftedSubscription?.Invoke(sender, e)); });
        base.OnRaidedChannelIsMatureAudience += ((object sender, EventArgs arg) => { CoroutineHost.Host(() => OnRaidedChannelIsMatureAudience(sender, arg)); });
        base.OnRitualNewChatter += ((object sender, OnRitualNewChatterArgs e) => { CoroutineHost.Host(() => OnRitualNewChatter?.Invoke(sender, e)); });
        base.OnFailureToReceiveJoinConfirmation += ((object sender, OnFailureToReceiveJoinConfirmationArgs e) => { CoroutineHost.Host(() => OnFailureToReceiveJoinConfirmation?.Invoke(sender, e)); });
        base.OnUnaccountedFor += ((object sender, OnUnaccountedForArgs e) => { CoroutineHost.Host(() => OnUnaccountedFor?.Invoke(sender, e)); });
    }

}
