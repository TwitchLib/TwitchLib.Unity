<p align="center">
<img src="http://swiftyspiffy.com/img/twitchlib.png" style="max-height: 300px;">
</p>

# TwitchLib.Unity

## About
TwitchLib repository representing all code belonging to the implementation of TwitchLib for Unity. Maintained primarily by LuckyNoS7evin & GameDevCompany.

* **Client**: Handles chat and whisper Twitch services. Complete with a suite of events that fire for virtually every piece of data received from Twitch. Helper methods also exist for replying to whispers or fetching moderator lists.
* **Api**: Complete coverage of v3, v5, and Helix endpoints. The API is now a singleton class. This class allows fetching all publically accessable data as well as modify Twitch services like profiles and streams. Provides also service to check for new follows and if stream is up.
* **PubSub**: Supports all documented Twitch PubSub topics as well as a few undocumented ones.

## Implementing
Fantastic guide by [@HonestDanGames](https://twitter.com/HonestDanGames): **[The Beginners Guide to Starting a Twitch Stream Overlay Game using Unity & C#](https://docs.google.com/document/d/1GfYC3BGW2gnS7GmNE1TwMEdk0QYY2zHccxXp53-WiKM)**

Below are basic examples of how to utilize TwitchLib.Unity libraries including Api, Chat Client and PubSub

### Chat Client for Unity
```csharp

// If type or namespace TwitchLib could not be found. Make sure you add the latest TwitchLib.Unity.dll to your project folder
// Download it here: https://github.com/TwitchLib/TwitchLib.Unity/releases
// Or download the repository at https://github.com/TwitchLib/TwitchLib.Unity, build it, and copy the TwitchLib.Unity.dll from the output directory
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

public class TwitchClientExample : MonoBehaviour
{
	[SerializeField] //[SerializeField] Allows the private field to show up in Unity's inspector. Way better than just making it public
	private string _channelToConnectTo = Secrets.USERNAME_FROM_OAUTH_TOKEN;

	private Client _client;

	private void Start()
	{
		// To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
		// Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
		// This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
		// Application.runInBackground = true;

		//Create Credentials instance
		ConnectionCredentials credentials = new ConnectionCredentials(Secrets.USERNAME_FROM_OAUTH_TOKEN, Secrets.OAUTH_TOKEN);

		// Create new instance of Chat Client
		_client = new Client();

		// Initialize the client with the credentials instance, and setting a default channel to connect to.
		_client.Initialize(credentials, _channelToConnectTo);

		// Bind callbacks to events
		_client.OnConnected += OnConnected;
		_client.OnJoinedChannel += OnJoinedChannel;
		_client.OnMessageReceived += OnMessageReceived;
		_client.OnChatCommandReceived += OnChatCommandReceived;

		// Connect
		_client.Connect();
	}

	private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
	{
		Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");

		if (!string.IsNullOrWhiteSpace(e.AutoJoinChannel))
			Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
	}

	private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
	{
		Debug.Log($"The bot {e.BotUsername} just joined the channel: {e.Channel}");
		_client.SendMessage(e.Channel, "I just joined the channel! PogChamp");
	}

	private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
	{
		Debug.Log($"Message received from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
	}

	private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
	{
		switch (e.Command.CommandText)
		{
			case "hello":
				_client.SendMessage(e.Command.ChatMessage.Channel, $"Hello {e.Command.ChatMessage.DisplayName}!");
				break;
			case "about":
				_client.SendMessage(e.Command.ChatMessage.Channel, "I am a Twitch bot running on TwitchLib!");
				break;
			default:
				_client.SendMessage(e.Command.ChatMessage.Channel, $"Unknown chat command: {e.Command.CommandIdentifier}{e.Command.CommandText}");
				break;
		}
	}

	private void Update()
	{
		// Don't call the client send message on every Update,
		// this is sample on how to call the client,
		// not an example on how to code.
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_client.SendMessage(_channelToConnectTo, "I pressed the space key within Unity.");
		}
	}
}

```

### Api for Unity
```csharp

using System.Collections;
using System.Collections.Generic;
// If type or namespace TwitchLib could not be found. Make sure you add the latest TwitchLib.Unity.dll to your project folder
// Download it here: https://github.com/TwitchLib/TwitchLib.Unity/releases
// Or download the repository at https://github.com/TwitchLib/TwitchLib.Unity, build it, and copy the TwitchLib.Unity.dll from the output directory
using TwitchLib.Unity;
using UnityEngine;

public class TwitchApiExample : MonoBehaviour
{
	[SerializeField] //[SerializeField] Allows the private field to show up in Unity's inspector. Way better than just making it public
	private string _usernameToGetChannelVideosFrom = "LuckyNoS7evin";

	private Api _api;

	private void Start()
	{
		// To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
		// Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
		// This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
		// Application.runInBackground = true;

		// Create new instance of Api
		_api = new Api();

		// The api needs a ClientID or an OAuth token to start making calls to the api.

		// Set the client id
		_api.Settings.ClientId = Secrets.CLIENT_ID;

		// Set the oauth token.
		// Most requests don't require an OAuth token, in which case setting a client id would be sufficient.
		// Some requests require an OAuth token with certain scopes. Make sure your OAuth token has these scopes or the request will fail.
		_api.Settings.AccessToken = Secrets.OAUTH_TOKEN;
	}

	private void Update()
	{
		// Don't call the Api on every Update, this is sample on how to call the Api,
		// This is not an example on how to code.
		if (Input.GetKeyDown(KeyCode.Alpha1)) // Alpha1 = The number 1 key on your keyboard.
		{
			// Do what you want here, however if you want to call the twitch API this can be done as follows.
			// The following example is the GetChannelVideos if you want to call any TwitchLib.Api
			// endpoint replace the the following with your method call "_api.Channels.v5.GetChannelVideosAsync("{{CHANNEL_ID}}");"
			_api.Invoke(_api.Channels.v5.GetChannelVideosAsync("14900522"),
						GetChannelVideosCallback);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2)) // Alpha2 = The number 2 key on your keyboard.
		{
			if (string.IsNullOrWhiteSpace(_usernameToGetChannelVideosFrom))
				throw new System.Exception($"The provided channel doesn't exist: {_usernameToGetChannelVideosFrom}");

			StartCoroutine(GetChannelVideosByUsername(_usernameToGetChannelVideosFrom));
		}
	}

	private IEnumerator GetChannelVideosByUsername(string usernameToGetChannelVideosFrom)
	{
		// Lets get Lucky's id first
		TwitchLib.Api.Models.Helix.Users.GetUsers.GetUsersResponse getUsersResponse = null;
		yield return _api.InvokeAsync(_api.Users.helix.GetUsersAsync(logins: new List<string> { usernameToGetChannelVideosFrom }),
									  (response) => getUsersResponse = response);
		// We won't reach this point until the api request is completed, and the getUsersResponse is set.

		// We'll assume the request went well and that we made no typo's, meaning we should have 1 user at index 0, which is LuckyNoS7evin
		string luckyId = getUsersResponse.Users[0].Id;

		// Now that we have lucky's id, lets get his videos!
		TwitchLib.Api.Models.v5.Channels.ChannelVideos channelVideos = null;
		yield return _api.InvokeAsync(_api.Channels.v5.GetChannelVideosAsync(luckyId),
									  (response) => channelVideos = response);
		// Again, we won't reach this point until the request is completed!

		// Handle user's ChannelVideos
		// Using this way of calling the api, we still have access to usernameToGetChannelVideosFrom!

		var listOfVideoTitles = GetListOfVideoTitles(channelVideos);
		var printableListOfVideoTitles = string.Join("  |  ", listOfVideoTitles);

		Debug.Log($"Videos from user {usernameToGetChannelVideosFrom}: {printableListOfVideoTitles}");
	}

	private void GetChannelVideosCallback(TwitchLib.Api.Models.v5.Channels.ChannelVideos e)
	{
		var listOfVideoTitles = GetListOfVideoTitles(e);
		var printableListOfVideoTitles = string.Join("  |  ", listOfVideoTitles);

		Debug.Log($"Videos from 14900522: {printableListOfVideoTitles}");
	}

	private List<string> GetListOfVideoTitles(TwitchLib.Api.Models.v5.Channels.ChannelVideos channelVideos)
	{
		List<string> videoTitles = new List<string>();

		foreach (var video in channelVideos.Videos)
			videoTitles.Add(video.Title);

		return videoTitles;
	}
}

```

### PubSub for Unity
```csharp

// If type or namespace TwitchLib could not be found. Make sure you add the latest TwitchLib.Unity.dll to your project folder
// Download it here: https://github.com/TwitchLib/TwitchLib.Unity/releases
// Or download the repository at https://github.com/TwitchLib/TwitchLib.Unity, build it, and copy the TwitchLib.Unity.dll from the output directory
using TwitchLib.Unity;
using UnityEngine;

public class TwitchPubSubExample : MonoBehaviour
{
	private PubSub _pubSub;

	private void Start()
	{
		// To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
		// Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
		// This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
		// Application.runInBackground = true;

		// Create new instance of PubSub Client
		_pubSub = new PubSub();

		// Subscribe to Events
		_pubSub.OnBitsReceived += OnBitsReceived;
		_pubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;

		// Connect
		_pubSub.Connect();
	}

	private void OnPubSubServiceConnected(object sender, System.EventArgs e)
	{
		Debug.Log("PubSubServiceConnected!");

		// On connect listen to Bits event
		// Please note that to listen to the bits event requires the bits:read scope in the OAuth token.
		_pubSub.ListenToBitsEvents(Secrets.USERNAME_FROM_OAUTH_TOKEN);

		// SendTopics accepts an oauth optionally, which is necessary for some topics, such as bit events.
		_pubSub.SendTopics(Secrets.OAUTH_TOKEN);
	}

	private void OnBitsReceived(object sender, TwitchLib.PubSub.Events.OnBitsReceivedArgs e)
	{
		// Do your bits logic here.
	}
}

```

### A Secrets class created specifically for the examples above
```csharp

public static class Secrets
{
	public const string CLIENT_ID = "CLIENT_ID"; //Your application's client ID, register one at https://dev.twitch.tv/dashboard
	public const string OAUTH_TOKEN = "OAUTH_TOKEN"; //A Twitch OAuth token which can be used to connect to the chat
	public const string USERNAME_FROM_OAUTH_TOKEN = "USERNAME_FROM_OAUTH_TOKEN"; //The username which was used to generate the OAuth token
}

```
