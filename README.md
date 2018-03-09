<p align="center"> 
<img src="http://swiftyspiffy.com/img/twitchlib.png" style="max-height: 300px;">
</p>

# TwitchLib.Unity

## About 
TwitchLib repository representing all code belonging to the implementation of TwitchLib for Unity. Maintained primarily by LuckyNoS7evin & GameDevCompany.

* **TwitchClientUnity**: Handles chat and whisper Twitch services. Complete with a suite of events that fire for virtually every piece of data received from Twitch. Helper methods also exist for replying to whispers or fetching moderator lists.
* **TwitchAPIUnity**: Complete coverage of v3, v5, and Helix endpoints. The API is now a singleton class. This class allows fetching all publically accessable data as well as modify Twitch services like profiles and streams.
* **TwitchPubSubUnity**: Supports all documented Twitch PubSub topics as well as a few undocumented ones.

## Implementing
Below are basic examples of how to utilize TwitchLib.Unity libraries including Api, Chat Client and PubSub. These are C# examples, but this library can also be used in Visual Basic.

### TwitchClientUnity
```csharp
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Models;

public class TwitchClientExample : MonoBehaviour
{
    private TwitchClientUnity _client;

    void Start()
    {
        //Set to run in minimized mode
        Application.runInBackground = true;
        //Create Credentials instance
        ConnectionCredentials credentials = new ConnectionCredentials("{{USER_TO_CONNECT_AS}}", "{{ACCESS_TOKEN}}");
        //Create new instance of Chat Client
        _client = new TwitchClientUnity();
        _client.Initialize(credentials, "{{CHANNEL_TO_CONNECT_TO}}");
        _client.OnMessageReceived += _client_OnMessageReceived; ;
        _client.Connect();
       
    }

    private void _client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        //Your implementation here
    }

    // Update is called once per frame
    void Update()
    {
        _client.SendMessage("");   
    }

}
```

### TwitchApiUnity
```csharp
using UnityEngine;
using TwitchLib.Unity;

public class TwitchApiExample : MonoBehaviour
{
    private TwitchApiUnity _api;

    void Start()
    {
        //Set to run in minimized mode
        Application.runInBackground = true;
        //Create new instance of Api
        _api = new TwitchApiUnity();
        //Initialize your Api with credentials
        _api.InitializeAsync(Secrets.ClientId, Secrets.OAuth);
    }
    
    // Update is called once per frame
    void Update()
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

    private void GetChannelVideosCallback(TwitchLib.Api.Models.v5.Channels.ChannelVideos e)
    {

    }
}


```

### TwitchPubSubUnity
```csharp
using UnityEngine;
using TwitchLib.Unity;

public class TwitchPubSubExample : MonoBehaviour
{
    private TwitchPubSubUnity _client;

    void Start()
    {
        //Set to run in minimized mode
        Application.runInBackground = true;
        //Create new instance of PubSub Client
        _client = new TwitchPubSubUnity();
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


    // Update is called once per frame
    void Update()
    {
    }

}

```

