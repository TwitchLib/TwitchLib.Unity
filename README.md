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
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Models;

public class TwitchClientExample : MonoBehaviour
{
    private Client _client;

    void Start()
    {
        //Set to run in minimized mode
        Application.runInBackground = true;
        //Create Credentials instance
        ConnectionCredentials credentials = new ConnectionCredentials("{{USER_TO_CONNECT_AS}}", "{{ACCESS_TOKEN}}");
        //Create new instance of Chat Client
        _client = new Client();
        _client.Initialize(credentials, "{{CHANNEL_TO_CONNECT_TO}}");
        _client.OnMessageReceived += _client_OnMessageReceived; ;
        _client.Connect();
       
    }

    private void _client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannel e) {
        _client.SendMessage(e.Channel, "I just joined the channel! PogChamp ");
    }
    
    private void _client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        //Your implementation here
    }
    
    private void _client_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceived e) {
        switch(e.Command.CommandText) {
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

    // Update is called once per frame
    void Update()
    {
        //Don't call the client send message on every Update, 
        //this is sample on how to call the client,
        //not an example on how to code.
        if(SomeConditionMet)
        {
            _client.SendMessage("");   
        }
    }

}
```

### Api for Unity
```csharp
using UnityEngine;
using TwitchLib.Unity;

public class TwitchApiExample : MonoBehaviour
{
    private Api _api;

    private void Start()
    {
        //Set to run in minimized mode
        Application.runInBackground = true;
        //Create new instance of Api
        _api = new Api();
        //Initialize your Api with credentials
        _api.InitializeAsync(Secrets.ClientId, Secrets.OAuth);
    }

    // Update is called once per frame
    private void Update()
    {
        //Don't call the Api on every Update, this is sample on how to call the Api,
        //this is not an example on how to code.
        if (SomeConditionMet)
        {
            //Do what you want here, however if you want to call the twitch API this can be done as follows. 
            //The following example is the GetChannelVideos if you want to call any TwitchLib.Api
            //endpoint replace the the following with your method call "_api.Channels.v5.GetChannelVideosAsync("{{CHANNEL_ID}}");"
            _api.Invoke(
                //Func<Task<ChannelVideos>>
                () => { return _api.Channels.v5.GetChannelVideosAsync("14900522"); },
                //Action<T>
                GetChannelVideosCallback);
        }

        if (SomeOtherConditionMet)
        {
            StartCoroutine(GetLuckyNoS7evinsChannelVideos("LuckyNoS7evin"));
        }
    }
    
    private System.Collections.IEnumerator GetLuckyNoS7evinsChannelVideos(string luckysUsername)
    {
        //Lets get Lucky's id first
        TwitchLib.Api.Models.Helix.Users.GetUsers.GetUsersResponse getUsersResponse = null;
        yield return _api.InvokeAsync(
               //Func<Task<ChannelVideos>>
               () => { return _api.Users.helix.GetUsersAsync(logins: new List<string> { luckysUsername }); },
               //Action<T>
               (response) => getUsersResponse = response);
        //We won't reach this point until the api request is completed, and the getUsersResponse is set.

        //We'll assume the request went well and that we made no typo's, meaning we should have 1 user at index 1, which is LuckyNoS7evin
        string luckyId = getUsersResponse.Users[0].Id;

        //Now that we have lucky's id, lets get his videos!
        TwitchLib.Api.Models.v5.Channels.ChannelVideos luckyNoS7evinsChannelVideos = null;
        yield return _api.InvokeAsync(
               //Func<Task<ChannelVideos>>
               () => { return _api.Channels.v5.GetChannelVideosAsync(luckyId); },
               //Action<T>
               (response) => luckyNoS7evinsChannelVideos = response);
        //Again, we won't reach this point until the request is completed!

        //Handle luckyNoS7evin's ChannelVideos
        //Using this way of calling the api, we still have access to luckysUsername!
    }

    private void GetChannelVideosCallback(TwitchLib.Api.Models.v5.Channels.ChannelVideos e)
    {

    }
}


```

### PubSub for Unity
```csharp
using UnityEngine;
using TwitchLib.Unity;

public class TwitchPubSubExample : MonoBehaviour
{
    private PubSub _client;

    void Start()
    {
        //Set to run in minimized mode
        Application.runInBackground = true;
        //Create new instance of PubSub Client
        _client = new PubSub();
        //Subscribe to Events
        _client.OnBitsReceived += _client_OnBitsReceived;
        _client.OnPubSubServiceConnected += _client_OnPubSubServiceConnected;
        //Connect
        _client.Connect();
       
    }

    private void _client_OnPubSubServiceConnected(object sender, System.EventArgs e)
    {
        //On connect listen to Bits event
        _client.ListenToBitsEvents("{{CHANNEL_ID}}");
        _client.SendTopics("{{ACCESS_TOKEN}}");
    }

    private void _client_OnBitsReceived(object sender, TwitchLib.PubSub.Events.OnBitsReceivedArgs e)
    {
        //Do your bits logic here.
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