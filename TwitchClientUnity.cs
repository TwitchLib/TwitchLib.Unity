using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Exceptions.Client;
using TwitchLib.Internal;
using TwitchLib.Models.Client;
using TwitchLib.Logging;
using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Proxy;
using TwitchLib.Services;

public class TwitchClientUnity : ITwitchClient {


    #region Public Variables
    /// <summary>A list of all channels the client is currently in.</summary>
    public List<JoinedChannel> JoinedChannels {
        get { return client.JoinedChannels; }
    }
    /// <summary>Username of the user connected via this library.</summary>
    public string TwitchUsername {
        get { return client.TwitchUsername; }
    }
    /// <summary>The most recent whisper received.</summary>
    public WhisperMessage PreviousWhisper {
        get { return client.PreviousWhisper; }
    }
    /// <summary>The current connection status of the client.</summary>
    public bool IsConnected => client.IsConnected;
    /// <summary>Assign this property a valid MessageThrottler to apply message throttling on chat messages.</summary>
    public MessageThrottler ChatThrottler {
        get { return client.ChatThrottler; }
        set { client.ChatThrottler = value; }
    }
    /// <summary>Assign this property a valid MessageThrottler to apply message throttling on whispers.</summary>
    public MessageThrottler WhisperThrottler {
        get { return client.WhisperThrottler; }
        set { client.WhisperThrottler = value; }
    }
    /// <summary>The emotes this channel replaces.</summary>
    /// <remarks>
    ///     Twitch-handled emotes are automatically added to this collection (which also accounts for
    ///     managing user emote permissions such as sub-only emotes). Third-party emotes will have to be manually
    ///     added according to the availability rules defined by the third-party.
    /// </remarks>
    public MessageEmoteCollection ChannelEmotes => client.ChannelEmotes;
    /// <summary>Will disable the client from sending automatic PONG responses to PING</summary>
    public bool DisableAutoPong {
        get { return client.DisableAutoPong; }
        set { client.DisableAutoPong = value; }
    }
    /// <summary>Determines whether Emotes will be replaced in messages.</summary>
    public bool WillReplaceEmotes {
        get { return client.WillReplaceEmotes; }
        set { client.WillReplaceEmotes = value; }
    }
    /// <summary>If set to true, the library will not check upon channel join that if BeingHosted event is subscribed, that the bot is connected as broadcaster. Only override if the broadcaster is joining multiple channels, including the broadcaster's.</summary>
    public bool OverrideBeingHostedCheck {
        get { return client.OverrideBeingHostedCheck; }
        set { client.OverrideBeingHostedCheck = value; }
    }
    /// <summary>Provides access to connection credentials object.</summary>
    public ConnectionCredentials ConnectionCredentials {
        get { return client.ConnectionCredentials; }
        set { client.ConnectionCredentials = value; }
    }

    /// <summary>Provides access to autorelistiononexception on off boolean.</summary>
    public bool AutoReListenOnException {
        get { return client.AutoReListenOnException; }
        set { client.AutoReListenOnException = value; }
    }
    /// <summary>Provides access to a Logger</summary>
    public TwitchLib.Logging.ILogger Logger {
        get { return client.Logger; }
    }

    public bool Logging {
        get { return client.Logging; }
        set { client.Logging = value; }
    }
    #endregion


    #region Events
    /// <summary>
    /// Fires whenever a log write happens.
    /// </summary>
    public event EventHandler<OnLogArgs> OnLog;

    /// <summary>
    /// Fires when client connects to Twitch.
    /// </summary>
    public event EventHandler<OnConnectedArgs> OnConnected;

    /// <summary>
    /// Fires when client joins a channel.
    /// </summary>
    public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;

    /// <summary>
    /// Fires on logging in with incorrect details, returns ErrorLoggingInException.
    /// </summary>
    public event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;

    /// <summary>
    /// Fires when connecting and channel state is changed, returns ChannelState.
    /// </summary>
    public event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;

    /// <summary>
    /// Fires when a user state is received, returns UserState.
    /// </summary>
    public event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;

    /// <summary>
    /// Fires when a user state is received, returns UserState.
    /// </summary>
    public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

    /// <summary>
    /// Fires when a new whisper arrives, returns WhisperMessage.
    /// </summary>
    public event EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;

    /// <summary>
    /// Fires when a chat message is sent, returns username, channel and message.
    /// </summary>
    public event EventHandler<OnMessageSentArgs> OnMessageSent;

    /// <summary>
    /// Fires when a whisper message is sent, returns username and message.
    /// </summary>
    public event EventHandler<OnWhisperSentArgs> OnWhisperSent;

    /// <summary>
    /// Fires when command (uses custom chat command identifier) is received, returns channel, command, ChatMessage, arguments as string, arguments as list.
    /// </summary>
    public event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;

    /// <summary>
    /// Fires when command (uses custom whisper command identifier) is received, returns command, Whispermessage.
    /// </summary>
    public event EventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;

    /// <summary>
    /// Fires when a new viewer/chatter joined the channel's chat room, returns username and channel.
    /// </summary>
    public event EventHandler<OnUserJoinedArgs> OnUserJoined;

    /// <summary>
    /// Fires when a moderator joined the channel's chat room, returns username and channel.
    /// </summary>
    public event EventHandler<OnModeratorJoinedArgs> OnModeratorJoined;

    /// <summary>
    /// Fires when a moderator joins the channel's chat room, returns username and channel.
    /// </summary>
    public event EventHandler<OnModeratorLeftArgs> OnModeratorLeft;

    /// <summary>
    /// Fires when new subscriber is announced in chat, returns Subscriber.
    /// </summary>
    public event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;

    /// <summary>
    /// Fires when current subscriber renews subscription, returns ReSubscriber.
    /// </summary>
    public event EventHandler<OnReSubscriberArgs> OnReSubscriber;

    /// <summary>
    /// Fires when a hosted streamer goes offline and hosting is killed.
    /// </summary>
    public event EventHandler OnHostLeft;

    /// <summary>
    /// Fires when Twitch notifies client of existing users in chat.
    /// </summary>
    public event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;

    /// <summary>
    /// Fires when a PART message is received from Twitch regarding a particular viewer
    /// </summary>
    public event EventHandler<OnUserLeftArgs> OnUserLeft;

    /// <summary>
    /// Fires when the joined channel begins hosting another channel.
    /// </summary>
    public event EventHandler<OnHostingStartedArgs> OnHostingStarted;

    /// <summary>
    /// Fires when the joined channel quits hosting another channel.
    /// </summary>
    public event EventHandler<OnHostingStoppedArgs> OnHostingStopped;

    /// <summary>
    /// Fires when bot has disconnected.
    /// </summary>
    public event EventHandler<OnDisconnectedArgs> OnDisconnected;

    /// <summary>
    /// Forces when bot suffers conneciton error.
    /// </summary>
    public event EventHandler<OnConnectionErrorArgs> OnConnectionError;

    /// <summary>
    /// Fires when a channel's chat is cleared.
    /// </summary>
    public event EventHandler<OnChatClearedArgs> OnChatCleared;

    /// <summary>
    /// Fires when a viewer gets timedout by any moderator.
    /// </summary>
    public event EventHandler<OnUserTimedoutArgs> OnUserTimedout;

    /// <summary>
    /// Fires when client successfully leaves a channel.
    /// </summary>
    public event EventHandler<OnLeftChannelArgs> OnLeftChannel;

    /// <summary>
    /// Fires when a viewer gets banned by any moderator.
    /// </summary>
    public event EventHandler<OnUserBannedArgs> OnUserBanned;

    /// <summary>
    /// Fires when a list of moderators is received.
    /// </summary>
    public event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;

    /// <summary>
    /// Fires when confirmation of a chat color change request was received.
    /// </summary>
    public event EventHandler<OnChatColorChangedArgs> OnChatColorChanged;

    /// <summary>
    /// Fires when data is either received or sent.
    /// </summary>
    public event EventHandler<OnSendReceiveDataArgs> OnSendReceiveData;

    /// <summary>
    /// Fires when client receives notice that a joined channel is hosting another channel.
    /// </summary>
    public event EventHandler<OnNowHostingArgs> OnNowHosting;

    /// <summary>
    /// Fires when the library detects another channel has started hosting the broadcaster's stream. MUST BE CONNECTED AS BROADCASTER.
    /// </summary>
    public event EventHandler<OnBeingHostedArgs> OnBeingHosted;

    /// <summary>
    /// Fires when a raid notification is detected in chat
    /// </summary>
    public event EventHandler<OnRaidNotificationArgs> OnRaidNotification;

    /// <summary>
    /// Fires when a subscription is gifted and announced in chat
    /// </summary>
    public event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;

    /// <summary>Fires when TwitchClient attempts to host a channel it is in.</summary>
    public EventHandler OnSelfRaidError;

    /// <summary>Fires when TwitchClient receives generic no permission error from Twitch.</summary>
    public EventHandler OnNoPermissionError;

    /// <summary>Fires when newly raided channel is mature audience only.</summary>
    public EventHandler OnRaidedChannelIsMatureAudience;

    /// <summary>Fires when a ritual for a new chatter is received.</summary>
    public EventHandler<OnRitualNewChatterArgs> OnRitualNewChatter;

    /// <summary>Fires when the client was unable to join a channel.</summary>
    public EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;

    /// <summary>Fires when data is received from Twitch that is not able to be parsed.</summary>
    public EventHandler<OnUnaccountedForArgs> OnUnaccountedFor;
    #endregion

    private TwitchClient client;

    /// <summary>
    /// Initializes the TwitchChatClient class.
    /// </summary>
    /// <param name="channel">The channel to connect to.</param>
    /// <param name="credentials">The credentials to use to log in.</param>
    /// <param name="chatCommandIdentifier">The identifier to be used for reading and writing commands from chat.</param>
    /// <param name="whisperCommandIdentifier">The identifier to be used for reading and writing commands from whispers.</param>
    /// <param name="logging">Whether or not loging to console should be enabled.</param>
    /// <param name="logger">Logger Type.</param>
    /// <param name="autoReListenOnExceptions">By default, TwitchClient will silence exceptions and auto-relisten for overall stability. For debugging, you may wish to have the exception bubble up, set this to false.</param>
    public TwitchClientUnity(ConnectionCredentials credentials, string channel = null, char chatCommandIdentifier = '!', char whisperCommandIdentifier = '!',
        bool logging = false, TwitchLib.Logging.ILogger logger = null, bool autoReListenOnExceptions = true) {
        CoroutineHost.PrepareCrossThread();
        client = new TwitchClient(credentials, channel, chatCommandIdentifier,whisperCommandIdentifier,logging,logger,autoReListenOnExceptions);
        client.OnLog += ((object sender, OnLogArgs e) => { CoroutineHost.Host(() => OnLog?.Invoke(sender, e)); });
        client.OnConnected += ((object sender, OnConnectedArgs e) => { CoroutineHost.Host(() => OnConnected?.Invoke(sender, e)); });
        client.OnJoinedChannel += ((object sender, OnJoinedChannelArgs e) => { CoroutineHost.Host(() => OnJoinedChannel?.Invoke(sender, e)); });
        client.OnIncorrectLogin += ((object sender, OnIncorrectLoginArgs e) => { CoroutineHost.Host(() => OnIncorrectLogin?.Invoke(sender, e)); });
        client.OnChannelStateChanged += ((object sender, OnChannelStateChangedArgs e) => { CoroutineHost.Host(() => OnChannelStateChanged?.Invoke(sender, e)); });
        client.OnUserStateChanged += ((object sender, OnUserStateChangedArgs e) => { CoroutineHost.Host(() => OnUserStateChanged?.Invoke(sender, e)); });
        client.OnMessageReceived += ((object sender, OnMessageReceivedArgs e) => { CoroutineHost.Host(() => OnMessageReceived?.Invoke(sender, e)); });
        client.OnWhisperReceived += ((object sender, OnWhisperReceivedArgs e) => { CoroutineHost.Host(() => OnWhisperReceived?.Invoke(sender, e)); });
        client.OnMessageSent += ((object sender, OnMessageSentArgs e) => { CoroutineHost.Host(() => OnMessageSent?.Invoke(sender, e)); });
        client.OnWhisperSent += ((object sender, OnWhisperSentArgs e) => { CoroutineHost.Host(() => OnWhisperSent?.Invoke(sender, e)); });
        client.OnChatCommandReceived += ((object sender, OnChatCommandReceivedArgs e) => { CoroutineHost.Host(() => OnChatCommandReceived?.Invoke(sender, e)); });
        client.OnWhisperCommandReceived += ((object sender, OnWhisperCommandReceivedArgs e) => { CoroutineHost.Host(() => OnWhisperCommandReceived?.Invoke(sender, e)); });
        client.OnUserJoined += ((object sender, OnUserJoinedArgs e) => { CoroutineHost.Host(() => OnUserJoined?.Invoke(sender, e)); });
        client.OnModeratorJoined += ((object sender, OnModeratorJoinedArgs e) => { CoroutineHost.Host(() => OnModeratorJoined?.Invoke(sender, e)); });
        client.OnModeratorLeft += ((object sender, OnModeratorLeftArgs e) => { CoroutineHost.Host(() => OnModeratorLeft?.Invoke(sender, e)); });
        client.OnNewSubscriber += ((object sender, OnNewSubscriberArgs e) => { CoroutineHost.Host(() => OnNewSubscriber?.Invoke(sender, e)); });
        client.OnReSubscriber += ((object sender, OnReSubscriberArgs e) => { CoroutineHost.Host(() => OnReSubscriber?.Invoke(sender, e)); });
        client.OnHostLeft += ((object sender, EventArgs arg) => { CoroutineHost.Host(() => OnHostLeft(sender, arg)); });
        client.OnExistingUsersDetected += ((object sender, OnExistingUsersDetectedArgs e) => { CoroutineHost.Host(() => OnExistingUsersDetected?.Invoke(sender, e)); });
        client.OnUserLeft += ((object sender, OnUserLeftArgs e) => { CoroutineHost.Host(() => OnUserLeft?.Invoke(sender, e)); });
        client.OnHostingStarted += ((object sender, OnHostingStartedArgs e) => { CoroutineHost.Host(() => OnHostingStarted?.Invoke(sender, e)); });
        client.OnHostingStopped += ((object sender, OnHostingStoppedArgs e) => { CoroutineHost.Host(() => OnHostingStopped?.Invoke(sender, e)); });
        client.OnDisconnected += ((object sender, OnDisconnectedArgs e) => { CoroutineHost.Host(() => OnDisconnected?.Invoke(sender, e)); });
        client.OnConnectionError += ((object sender, OnConnectionErrorArgs e) => { CoroutineHost.Host(() => OnConnectionError?.Invoke(sender, e)); });
        client.OnChatCleared += ((object sender, OnChatClearedArgs e) => { CoroutineHost.Host(() => OnChatCleared?.Invoke(sender, e)); });
        client.OnUserTimedout += ((object sender, OnUserTimedoutArgs e) => { CoroutineHost.Host(() => OnUserTimedout?.Invoke(sender, e)); });
        client.OnLeftChannel += ((object sender, OnLeftChannelArgs e) => { CoroutineHost.Host(() => OnLeftChannel?.Invoke(sender, e)); });
        client.OnUserBanned += ((object sender, OnUserBannedArgs e) => { CoroutineHost.Host(() => OnUserBanned?.Invoke(sender, e)); });
        client.OnModeratorsReceived += ((object sender, OnModeratorsReceivedArgs e) => { CoroutineHost.Host(() => OnModeratorsReceived?.Invoke(sender, e)); });
        client.OnChatColorChanged += ((object sender, OnChatColorChangedArgs e) => { CoroutineHost.Host(() => OnChatColorChanged?.Invoke(sender, e)); });
        client.OnSendReceiveData += ((object sender, OnSendReceiveDataArgs e) => { CoroutineHost.Host(() => OnSendReceiveData?.Invoke(sender, e)); });
        client.OnNowHosting += ((object sender, OnNowHostingArgs e) => { CoroutineHost.Host(() => OnNowHosting?.Invoke(sender, e)); });
        client.OnBeingHosted += ((object sender, OnBeingHostedArgs e) => { CoroutineHost.Host(() => OnBeingHosted?.Invoke(sender, e)); });
        client.OnRaidNotification += ((object sender, OnRaidNotificationArgs e) => { CoroutineHost.Host(() => OnRaidNotification?.Invoke(sender, e)); });
        client.OnGiftedSubscription += ((object sender, OnGiftedSubscriptionArgs e) => { CoroutineHost.Host(() => OnGiftedSubscription?.Invoke(sender, e)); });
        client.OnRaidedChannelIsMatureAudience += ((object sender, EventArgs arg) => { CoroutineHost.Host(() => OnRaidedChannelIsMatureAudience(sender, arg)); });
        client.OnRitualNewChatter += ((object sender, OnRitualNewChatterArgs e) => { CoroutineHost.Host(() => OnRitualNewChatter?.Invoke(sender, e)); });
        client.OnFailureToReceiveJoinConfirmation += ((object sender, OnFailureToReceiveJoinConfirmationArgs e) => { CoroutineHost.Host(() => OnFailureToReceiveJoinConfirmation?.Invoke(sender, e)); });
        client.OnUnaccountedFor += ((object sender, OnUnaccountedForArgs e) => { CoroutineHost.Host(() => OnUnaccountedFor?.Invoke(sender, e)); });
    }


    /// <summary>
    /// Sends a RAW IRC message.
    /// </summary>
    /// <param name="message">The RAW message to be sent.</param>
    public void SendRaw(string message) {
        client.SendRaw(message);
    }

    #region SendMessage
    /// <summary>
    /// Sends a formatted Twitch channel chat message.
    /// </summary>
    /// <param name="message">The message to be sent.</param>
    /// <param name="dryRun">If set to true, the message will not actually be sent for testing purposes.</param>
    /// <param name="channel">Channel to send message to.</param>
    public void SendMessage(JoinedChannel channel, string message, bool dryRun = false) {
        client.SendMessage(channel, message, dryRun);
    }

    /// <summary>
    /// SendMessage wrapper that accepts channel in string form.
    /// </summary>
    public void SendMessage(string channel, string message, bool dryRun = false) {
        client.SendMessage(channel, message, dryRun);
    }

    /// <summary>
    /// SendMessage wrapper that sends message to first joined channel.
    /// </summary>
    public void SendMessage(string message, bool dryRun = false) {
        client.SendMessage(message, dryRun);
    }
    #endregion

    #region Whispers
    /// <summary>
    /// Sends a formatted whisper message to someone.
    /// </summary>
    /// <param name="receiver">The receiver of the whisper.</param>
    /// <param name="message">The message to be sent.</param>
    /// <param name="dryRun">If set to true, the message will not actually be sent for testing purposes.</param>
    public void SendWhisper(string receiver, string message, bool dryRun = false) {
        client.SendWhisper(receiver, message, dryRun);
    }
    #endregion

    #region Connection Calls
    /// <summary>
    /// Start connecting to the Twitch IRC chat.
    /// </summary>
    public void Connect() {
        client.Connect();
    }

    /// <summary>
    /// Start disconnecting from the Twitch IRC chat.
    /// </summary>
    public void Disconnect() {
        client.Disconnect();
    }
    #endregion

    #region Command Identifiers
    /// <summary>
    /// Adds a character to a list of characters that if found at the start of a message, fires command received event.
    /// </summary>
    /// <param name="identifier">Character, that if found at start of message, fires command received event.</param>
    public void AddChatCommandIdentifier(char identifier) {
        client.AddChatCommandIdentifier(identifier);
    }

    /// <summary>
    /// Removes a character from a list of characters that if found at the start of a message, fires command received event.
    /// </summary>
    /// <param name="identifier">Command identifier to removed from identifier list.</param>
    public void RemoveChatCommandIdentifier(char identifier) {
        client.RemoveChatCommandIdentifier(identifier);
    }

    /// <summary>
    /// Adds a character to a list of characters that if found at the start of a whisper, fires command received event.
    /// </summary>
    /// <param name="identifier">Character, that if found at start of message, fires command received event.</param>
    public void AddWhisperCommandIdentifier(char identifier) {
        client.AddWhisperCommandIdentifier(identifier);
    }

    /// <summary>
    /// Removes a character to a list of characters that if found at the start of a whisper, fires command received event.
    /// </summary>
    /// <param name="identifier">Command identifier to removed from identifier list.</param>
    public void RemoveWhisperCommandIdentifier(char identifier) {
        client.RemoveWhisperCommandIdentifier(identifier);
    }
    #endregion

    #region Channel Calls
    /// <summary>
    /// Join the Twitch IRC chat of <paramref name="channel"/>.
    /// </summary>
    /// <param name="channel">The channel to join.</param>
    /// <param name="overrideCheck">Override a join check.</param>
    public void JoinChannel(string channel, bool overrideCheck = false) {
        client.JoinChannel(channel, overrideCheck);
    }

    /*public void JoinRoom(string channelId, string roomId, bool overrideCheck = false) {
        client.JoinRoom(channelId, roomId, overrideCheck);
    }*/

    /// <summary>
    /// Returns a JoinedChannel object using a passed string/>.
    /// </summary>
    /// <param name="channel">String channel to search for.</param>
    public JoinedChannel GetJoinedChannel(string channel) {
        return client.GetJoinedChannel(channel);
    }

    /// <summary>
    /// Leaves (PART) the Twitch IRC chat of <paramref name="channel"/>.
    /// </summary>
    /// <param name="channel">The channel to leave.</param>
    /// <returns>True is returned if the passed channel was found, false if channel not found.</returns>
    public void LeaveChannel(string channel) {
        client.LeaveChannel(channel);
    }

    /*public void LeaveRoom(string channelId, string roomId) {
        client.LeaveRoom(channelId, roomId);
    }*/

    /// <summary>
    /// Leaves (PART) the Twitch IRC chat of <paramref name="channel"/>.
    /// </summary>
    /// <param name="channel">The JoinedChannel object to leave.</param>
    /// <returns>True is returned if the passed channel was found, false if channel not found.</returns>
    public void LeaveChannel(JoinedChannel channel) {
        client.LeaveChannel(channel);
    }

    /// <summary>
    /// Sends a request to get channel moderators. You MUST listen to OnModeratorsReceived event./>.
    /// </summary>
    /// <param name="channel">JoinedChannel object to designate which channel to send request to.</param>
    public void GetChannelModerators(JoinedChannel channel) {
        client.GetChannelModerators(channel);
    }

    /// <summary>
    /// Sends a request to get channel moderators. You MUST listen to OnModeratorsReceived event./>.
    /// </summary>
    /// <param name="channel">String representing channel to designate which channel to send request to.</param>
    public void GetChannelModerators(string channel) {
        client.GetChannelModerators(channel);
    }

    /// <summary>
    /// Sends a request to get channel moderators. Request sent to first joined channel. You MUST listen to OnModeratorsReceived event./>.
    /// </summary>
    public void GetChannelModerators() {
        client.GetChannelModerators();
    }
    #endregion

    public void Log(string message, bool includeDate = false, bool includeTime = false) {
        client.Log(message, includeDate, includeTime);
    }

    /// <summary>
    /// This method allows firing the message parser with a custom irc string allowing for easy testing
    /// </summary>
    /// <param name="rawIrc">This should be a raw IRC message resembling one received from Twitch IRC.</param>
    public void OnReadLineTest(string rawIrc) {
        client.OnReadLineTest(rawIrc);
    }

    #region Client Events

    public void Reconnect() {
        client.Reconnect();
    }

    #endregion

    public void SendQueuedItem(string message) {
        client.SendQueuedItem(message);
    }

}
