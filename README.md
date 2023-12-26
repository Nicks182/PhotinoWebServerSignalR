# PhotinoWebServerSignalR

Shows how to embed HTML, JS, and CSS files into exe and serve them using a custom built in web server which you can access over the network as well.

Using [Photino.io ](https://www.tryphotino.io/)  allows us to create cross platform desktop applications using HTML, javascript, and CSS with C# as your backend.

# But why not just use [Photino.NET.Server](https://www.nuget.org/packages/Photino.NET.Server)?

Generally you should be using Photino.NET.Server to deal with your embedded files, but rolling your own allows for more options. One such option is being able to access your app over the network from any device with a browser like your phone.

I wrote [MixerFixerV1](https://github.com/Nicks182/MixerFixerV1) a while ago to help control my audio in Windows. It’s a WPF app using the WebView2 control and the app includes a web server. Yes, photino would have been better. While the app is running on my desktop, I can access the app over the network using the browser on my phone and get full control of the volume levels of all open apps. Sure, it’s not something you would use often and I’ve not used my phone to control the volume much at all. However, I can also open it using the browser on my desktop and this has been pretty useful.

Depending on the type of app you are building and the requirements, it may be useful (or just cool…) to be able to access the app over the network using another device with a browser.


# The How
### 1. Create a new project

Start by creating a new simple Console Application. I’m using DotNet 8.0, but you can use 7.0 if you prefer. I also ticked the “Do not use top-level statements” under Additional Information, but you don’t have to.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/13eb7419-d033-4078-9844-cba4ba2c3c30)

### 2. Nuget packages

We’ll first set up only what is needed to get the basic app going and then add SignalR later. For now we only need to install 2 packages, marked in red in the image below. The rest will be added automatically.

Photino.NET

Microsoft.Extensions.FileProviders.Embedded

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/12453451-a07d-4e31-ac29-f11db371dc4b)

### 3. Add assets.

We need a folder to hold our web assets. For this example we will call it wwwroot.

You can add your HTML, Script, and CSS files.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/73720387-fa25-447a-89a4-64fffcc74f05)

Edit the CSS file and add the following:

```
body{
	background-color: dimgray;
	color: cornsilk;
}
```

Edit the script.js file and add the following:

```
setTimeout(function ()
{
	alert("Hellow From script!");
}, 50);


function callDotNet()
{
	window.external.sendMessage("Hi from browser.");
}

window.external.receiveMessage(message => receiveHandler(message));

function receiveHandler(message)
{
	alert(message);
}
```

Now edit the index.html file and add the following:

```
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="UTF-8">
	<meta http-equiv="X-UA-Compatible" content="IE=edge">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<title>PhotinoEmbeddedFiles</title>

	<link href="style.css" rel="stylesheet" />
	<script src="script.js"></script>

</head>
<body>

	<h1>
    	some text for the html so we can see stuff and things
	</h1>

	<button type="button" onclick="callDotNet();">Send request to C#!</button>

</body>
</html>



```

Because of our file server we will be setting up soon, we can reference our JS and CSS files in our HTML file like normal.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/43d0d5df-365f-45f9-b745-93edba4b45b3)

### 4. Edit project file

Now we need to edit our project. We need to specify which directory in our project we want to embed and we need to add a reference to the ‘Microsoft.AspNetCore.App’ framework which will give us access to the stuff we need to create our own web server.

Start by right clicking the project and then click on Edit Project File.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/9439c177-1def-4261-8213-46c0dd5fa97e)

Next, edit the file to look like the image below.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/55ac2d6d-a78d-4e1f-b1df-4f44d592d897)

Save the changes and you can now check under the Dependencies/Frameworks if it matches the image below.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/7d7f9181-5f5a-40c7-a503-2fcb65c2649f)

NOTE: We change the OuputType to WinExe otherwise we will have the console window open every time we run the app. Keeping it as ‘Exe’ can be very useful for debugging as errors will get logged to the console, but make sure it’s set to WinExe when you want to publish your app.

### 5. Custom File Server

Time for some C# code. This is based on [Photino.NET.Server](https://github.com/tryphotino/photino.NET.Server) which is why the code is very similar looking. Main difference is I’m using an Options object and a different File Provider. I also have an extra property to allow access to the page over the network. Make sure this is set to True if you want to be able to access the app over the network using a browser.

I’m not going to go into too much detail here. Only 2 things here are of note for our use case which is the File Provider and allowing access over the network.

**File Provider:**

We need this provider so we can serve up any embedded file to our web page. Our web page has no idea where or even how to get things like our script file or css file since they are part of our exe file. The File Provider will serve all our static files including our HTML page. This allows us to embed these files into our exe instead of having them easily accessible in our app directory.

I placed the file provider code in its own method. Note the string: PhotinoWebServerSignalR.wwwroot

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/5f296b4a-daf0-4b63-b08f-a528144c5c7c)

The ‘PhotinoWebServerSignalR’ part of the string refers to the namespace of your project. In this case, it’s PhotinoWebServerSignalR.

The ‘wwwroot’ refers to your folder containing your embedded web assets. In this case, it’s just wwwroot.

Telling our web server to use our File Provider:

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/cac1c2ca-44ba-4ad0-b35f-e48559cd695f)

**Local Network Access:**

I added a property to the Options class called ‘AllowLocalAccess’. If it’s set to true, you can see below that we use * to allow any IP to connect to the web server. You can limit this to certain IP ranges if you want.

Read more about this here: [Server URLs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-8.0#server-urls)

By default access will be limited to localhost only. Meaning nothing will be able to connect from outside the machine the app is running on.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/486f545c-77ed-45f3-b18b-11590b41d7db)

**The code:**

Add a new class.cs file and name it what you like. For this example, I named it CustomFileServer.cs.

Add the following code to it.

Our CustomFileServer class:

```

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Net.NetworkInformation;

namespace PhotinoWebServerSignalR
{
	public class CustomFileServer
	{
    	public static WebApplication CreateStaticFileServer(CustomFileServerOptions options, out string baseUrl)
    	{
        	var builder = WebApplication
        	.CreateBuilder(new WebApplicationOptions()
        	{
            	Args = options.Args,
        	});

        	int port = options.PortStart;

        	// Try ports until available port is found
        	while (IPGlobalProperties
            	.GetIPGlobalProperties()
            	.GetActiveTcpListeners()
            	.Any(x => x.Port == port))
        	{
            	if (port > options.PortStart + options.PortRange)
            	{
                	throw new SystemException($"Couldn't find open port within range {options.PortStart} - {options.PortStart + options.PortRange}.");
            	}

            	port++;
        	}

        	baseUrl = $"http://localhost:{port}";

        	if (options.AllowLocalAccess)
        	{
            	builder.WebHost.UseUrls($"http://*:{port}");
        	}
        	else
        	{
            	builder.WebHost.UseUrls(baseUrl);
        	}

        	WebApplication app = builder.Build();


        	EmbeddedFileProvider fp = _Get_FileProvider();

        	app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fp });

        	app.UseStaticFiles(new StaticFileOptions
        	{
            	FileProvider = fp,
        	});


        	return app;
    	}

    	private static EmbeddedFileProvider _Get_FileProvider()
    	{
        	return new EmbeddedFileProvider(
            	assembly: typeof(CustomFileServer).Assembly,
            	baseNamespace: "PhotinoWebServerSignalR.wwwroot");
    	}

	}

	public class CustomFileServerOptions
	{
    	public string[] Args { get; set; }
    	public int PortStart { get; set; }
    	public int PortRange { get; set; }
    	public string WebRootFolder { get; set; }

    	public bool AllowLocalAccess { get; set; }
	}
}


```

### 6. Starting the server.

In our Program.cs file in our Main method we need to use our new class to start our new server.

NOTE: We have to add [STAThread] just above our Main method.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/6abbc7ff-3e84-4dd3-8605-93e4d7be2e6e)

Add the following code so our server will start when our app loads.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/d473e800-9fbb-4580-beff-516b786b22ff)

Our new class will try to find an available port for our web server, but we need to tell it in which range to look.

**PortStart:** This tells it where to start searching.

**PortRange:** This tells it how far to search.

In this example we start searching at port 8000 and it checks every port until 8100. If none of those ports are open, which should be highly unlikely, then it will throw an exception.

**WebRootFolder:** This is just the folder holding our embedded files.

**AllowLocalAccess:** If set to true, you can access the web page over the network.

**baseURL:** This will be the base url with whatever port is available and we need this to tell the Photino Window what URL to load after the server has figured out a port to use and has started.

### 7. Photino Window

Now we can create our photino window and pass in our baseURL.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/7f0e7b8e-6eb6-4bbc-9f63-f7172e8c927b)

For this example I’m also setting a handler for WindowCreated and WebMessageReceived.

Setting the window to be maximised during creation is bugged in version 2.5.2, but has been reported and will be fixed in newer versions. For now I use this event to set the window to be maximised.

The WebMessageReceive event is to handle messages being sent from javascript to the photino window. This will not work if you access the page in your browser, but we’ll fix that once we add SignalR later.

Complete main method code with handler:

```
[STAThread]
static void Main(string[] args)
{
	CustomFileServer
	.CreateStaticFileServer(new CustomFileServerOptions
	{
    	Args = args,
    	PortStart = 8000,
    	PortRange = 100,
    	WebRootFolder = "wwwroot",
    	AllowLocalAccess = false,
	},
	out string baseURL)
	.RunAsync();


	PhotinoWindow Wind = new PhotinoWindow();
	Wind.Load(baseURL);

	#region Addition functionality
	Wind.RegisterWindowCreatedHandler(Win_WindowCreated);

	Wind.RegisterWebMessageReceivedHandler((object sender, string message) =>
	{
    	var window = (PhotinoWindow)sender;

    	// Send a message back the to JavaScript event handler.
    	window.SendWebMessage("C# reveived the following message from the browser: " + message);
	});
	#endregion Addition functionality

	Wind.WaitForClose();
}

private static void Win_WindowCreated(object? sender, EventArgs e)
{
	// Using this event seems to be the only way to get Maximized to work.
	// Trying to set it before the window is created is a bug in Photino 2.5.2 which will be fixed in newer versions.
	(sender as PhotinoWindow).SetMaximized(true);
}
```

### 8. Test it.

The app should run now if you hit F5 in Visual Studio. When the window first opens it will show an alert. Close the alert and click the button to send a message to the C# code. The C# code will just send the message straight back and you will see another alert.

## SignalR (WebSockets)

SignalR allows you to add WebSockets to your app in a fairly easy way and it allows for 2 way, real-time communication between the browser window and the server. Meaning, the client/browser can send messages and the server can respond much like a rest API, but with WebSockets the server can also push messages to the client at any time while the connection is active.

Read more about SignalR here: [Overview of ASP.NET Core SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-8.0)

**Is SignalR overkill for a desktop app?**

Yes… probably. You could get away with a simple REST endpoint to make the app work over the network, but being able to push updates in real-time to the UI as our C# code handles events makes for a much nicer and smoother experience. 

As an example, my MixerFixer app constantly pushes to the UI to update the volume visualizers so you can see which apps are making noise and see their volume relative to other apps that may be making noise at the same time. And it looks cool. This is done on a 100ms timer in the C# code which just pushes the noise level values to the UI and then I have some simple JS to set the values as widths on the correct Divs.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/fa2f17d5-1514-481f-aafc-cf81fe8be8f4)

See a video demo of MixerFixer here: [MixerFixerV1](https://github.com/Nicks182/MixerFixerV1)


Getting SignalR running is fairly simple. We need some Nuget packages, create a Message Object/class that will be used to send messages between the client/browser and server, create a new SignalR HUB to handle all our connections and messages, and then we need the SignalR javascript library to handle things client/browser side.

NOTE: The Photino Window loads an instance of the WebView2 browser component. This is why I use the word ‘browser’. Also, this tutorial is about making your app accessible and working with a normal browser.

### 9. Adding SignalR Nuget


First, we need the following Nuget packages:

Microsoft.AspNetCore.SignalR.Client

Newtonsoft.Json

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/07826e2f-57cd-4c21-82d1-ac832d679021)

**Why Newtonsoft?**

**I’ve tried using the built dotnet json serializer and it works fine… normally. I’ve run into a few cases where it didn’t like some JSON objects and also didn’t want to serialise from a string value to a C# enum. So far, I’ve gone back to Newtonsoft each time I tried the built in serializer.**

### 10. Adding SignalR JS

Next we need to add the client side javascript library. With a normal Web application, like ASP.NET, we can do this quite easily by Right Clicking the folder we want our library to live in and selecting the “Add Client side Library” option. But, this is not a Web Application… at least Visual Studio doesn’t see it that way and as such we don’t have this nice and convenient option available.

However, when adding Client Side libraries visual studio will create a json config file that lists all the libraries we want and where they should live. So we can actually skip the nice little UI window and just create this file manually. Visual Studio will then add our library.

Start by creating a new folder inside the wwwroot folder. I called it: Libs

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/39a05d7b-8819-47af-83f2-88596042a996)

Then create a new empty file in the main project folder and call it: libman.json

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/1f982556-38e4-4693-a58c-caf6105482dd)

Next, open the file and add the following JSON to it. 

```
{
  "version": "1.0",
  "defaultProvider": "cdnjs",
  "libraries": [
	{
  	"library": "microsoft-signalr@8.0.0",
  	"destination": "wwwroot/Libs/",
  	"files": [
    	"signalr.js"
  	]
	}
  ]
}
```

Once you save the changes, the SignalR library version 8.0.0 will be downloaded and added to your project.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/a89e8109-c712-412c-bd05-9ad5dad885c0)

Now add a reference to the library in your HTML file.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/3edf6e1e-9736-4521-a628-236897d271da)

### 11. Message Object

*I realise this is a little off topic, but I want to give people at least some way of building an app once you have the basics up and running. When I did my first WebSocket tutorial many years ago, I was kinda lost on how to actually build something using WebSockets as the existing tutorials only show you how to get the basic thing going and passing simple strings between the frontend and backend. I had to figure out on my own, through trial and error, how to add some structure and cohesion when building something more complex than a chat app. My MessageObject way is far from perfect, but it is simple, quite flexible and I think is a decent starting point for newer developers trying SignalR/WebSockets for the first time.*

We want to create a kind of standard way to send messages between our javascript and C#. I call this the MessageObject and it contains a few simple properties that informs our app of what to do in our JS and C# sides.


This object can be as simple or as complex as you need it to be. You can send a list of IDs with their values to update UI elements or you can even generate HTML in C# and pass that as part of your message.

I realise that this might not make that much sense and for this tutorial it will be quite simple, but my [MixerFixerV1](https://github.com/Nicks182/MixerFixerV1) app uses this MessageObject concept and maybe I will make a simpler example showing just the MessageObject and how you could use it.

The point is to have some kind of standard and to avoid having all sorts of different types of messages that need to be handled differently. For a small project this shouldn’t be much of an issue, but I prefer having at least some kind of standard message object as I find it really simplifies the communications between my JS and C#. It also simplifies my code as I generally just add a MessageType that represents some functionality and a method/function to deal with that new type.

Did the user click a button? Send a MessageObject…

Did the user enter some text in an input? Send a MessageObject…

Did my C# code catch an event? Push a MessageObject…

Did my C# code catch an error? Push a MessageObject…

Did the user click a button to show a Modal? Send a MessageObject… and respond with a MessageObject containing the info for the Modal…

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/bed38000-882a-451d-826e-6c1f73fecfa7)

This will make more sense once we start adding code.

Let’s start by adding a new blank C# class file. I called mine: MessageObject.cs

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/e9cbda47-695e-4cc1-85ea-7fcbfc688aeb)

Then add the following code to it and save it.

```
namespace PhotinoWebServerSignalR
{
	public class MessageObject
	{
    	public string Message { get; set; }
    	public MessageObjectType MessageType { get; set; }
	}

	public enum MessageObjectType
	{
    	_DoNothing,
    	Greetings
	}
}
```

For this example I’m going with something extremely basic. Our string Message can be whatever you want here, but the MessageType I normally make an enum due to how fast a switch on an enum is in C#.

The MessageType is how you know what the INTENT of the message is. Here we have 2 “intentions”: _DoNothing and Greetings.

‘_DoNothing’ will be our default. It could also be to throw an error as a Message should pretty much always have a defined intent, but I will often send a message to the server which does not require a response and so once the server has dealt with the message I will set it to _DoNothing so no further processing is done.

‘Greetings’ is our only real intent here. If this is set then our code will do something different to our default. You can have as many “intentions” as you want.

For now, this is all we need. We will see how we work with this next.

### 12. Setup SignalR backend 

In our backend we need to set up some stuff to handle our web socket connections and handlers for our messages. With SignalR this is done by creating a HUB.

Start by create a new blank C# file. I called mine: SignalRHub

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/bbb63fb3-7f90-483e-ba5e-24d0c3c17da4)

You can copy the code below and I’ll go through it after.

```
using Microsoft.AspNetCore.SignalR;

namespace PhotinoWebServerSignalR
{
	public class SignalRHub : Hub
	{
    	public override async Task OnConnectedAsync()
    	{
        	// You can send to ALL and then any open window will receive the same message and show the same results.
        	await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    	}

    	public async Task ServerMessageHandler(string Message)
    	{
        	MessageObject L_MessageObject = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageObject>(Message);

        	await _ProcessMessage(L_MessageObject);

        	// Here we just respond to the window that made the call to the server.
        	await Clients.Caller.SendAsync("ClientMessageHandler", L_MessageObject);
    	}

    	private async Task _ProcessMessage(MessageObject P_MessageObject)
    	{
        	switch (P_MessageObject.MessageType)
        	{
            	case MessageObjectType.Greetings:
                	P_MessageObject.Message = "Message received from script. It said: " + P_MessageObject.Message;
                	break;

            	case MessageObjectType._DoNothing:
                	P_MessageObject.Message = "Message received from script, but it said to do nothing :(";
                	break;
        	}
    	}
	}
}
```

I’m going to keep this simple, but basically we create a new class that inherits from Hub. Part of ‘Hub’ we get access to the Clients object. This contains a list of all the current active connections and with this object we can push messages to all the clients at the same time if we wanted. Here I override the OnConnectedAsync method and then I push a message to all connected clients. This is not needed, it’s just for demo purposes.

Next I have a ServerMessageHandler method. This method will handle any messages coming from our client/javascript and we have to specify the name of this handler in our javascript. This means you could have multiple handlers to call from the frontend, but because we are using a MessageObject in this example it’s not really needed. Our MessageObject contains the MessageType which allows us to have a single Handler Method that will process our messages and pass them on to the correct method to deal with the message based on the Type.

You can see the first thing we do here is deserialize our JSON string to our MessageObject. 

Now we pass our message to a new method that will process the message and handle it according to the MessageType/Intention and then modify our message. This ProcessMessage method can be anywhere in your app. For this example I just placed it in the Hub itself.

Then we just push our modified message back to the calling client/browser/window. Or you can push it to all clients if that is something you require. You can also sometimes push to all and sometimes only to the caller. Up to you and your requirements.

NOTE: A class object will be passed to a method via its reference. This means our ProcessMessage method will be working directly with the object we created in the handler method instead of making a copy and so we don’t need to return anything from our ProcessMessage method.

Now we need to modify our CustomFileServer class a little so our Hub is actually started when the app starts up. At this point we will also be specifying our JSON settings.

First add the following to the top of the file:

```
using Microsoft.Extensions.DependencyInjection;
```

Now we can add the following code to inject SignalR and set our JSON options.

```
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
	options.PayloadSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(null, true));
	options.PayloadSerializerOptions.PropertyNamingPolicy = null;
	options.PayloadSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});
```

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/aadd8e85-c011-480c-83eb-9aac319b615f)

Now we need to Map our endpoint for our WebSocket Hub right at the end.

```
app.MapHub<SignalRHub>("MessageHub");
```

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/13cd07ec-8bff-4fc1-9b41-14e490b05b1b)

Note: The string “MessageHub” is the name of our Endpoint and is what we will be using to set up the client side (javascript) to create a WebSocket connection to our back end.

That should be all that is needed to setup SignalR in our backend along with our MessageObject.

### 13. SignalR frontend

To make our WebSocket connection, we need some javascript. Fortunately, the SignalR js library does most of the work for us, but we do need to specify the endpoint to connect to and a handler to deal with messages coming from the backend.

We will first create a new js file in our wwwroot folder. I called mine: script_Hub.js

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/097899e5-1164-4b4e-94f3-bd6e44c6bb48)

In our new js file, we need to create a new SignalR object and specify the end point to use.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/898b2d7f-9f6e-4c82-866d-b30a6d983024)

Then we need to add a handler for our messages coming from the backend.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/b4c32049-87fe-4235-b924-f4d03bdf9b73)

We also create a generic _Message_Send function which is what we will be calling from other parts of our code rather than referencing our SignalR object directly.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/83c92ecc-fc95-4f7a-82aa-fdf26a9c3b2d)

Then once our page is loaded, we need to call Start on our object to establish the connection to our Hub in our backend.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/571c3832-46cf-4af3-a4ed-510ac2375995)

Last thing is to add it to our HTML file.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/269ffeda-7988-44c9-97dc-2a4adb54b86a)

And the full code:

```

var G_ConnectionUrl = "/MessageHub";

var G_HubConnection = new signalR.HubConnectionBuilder().withUrl(G_ConnectionUrl).build();

// Setup handler for message from server...    
G_HubConnection.on("ClientMessageHandler", _Message_Receive);

function _Message_Receive(P_MessageObject)
{
	console.log(P_MessageObject);

	switch (P_MessageObject.MessageType)
	{
    	case "Greetings":
        	_Handle_Greetings(P_MessageObject);
        	break;

    	case "_DoNothing":
        	_Handle_DoNothing(P_MessageObject);
        	break;
	}
}


function _Message_Send(P_MessageObject)
{
	document.getElementById("Div_Greetings").innerHTML = "";

	G_HubConnection.invoke("ServerMessageHandler", _ToJSONString(P_MessageObject));

}

function _ToJSONString(P_MessageObject)
{
	return JSON.stringify(P_MessageObject);
}



document.addEventListener("DOMContentLoaded", () =>
{
	G_HubConnection.start().then(function ()
	{
    	console.log("Hub Status: " + G_HubConnection.state);
	});
});



```

At this point you will notice that our _Message_Receive handler is calling 2 other methods we have not defined yet. That’s next.

Open the existing ‘script.js’ file and replace everything in it with the code below.

```

function callDotNet()
{
	var Message =
	{
    	Message: "Hi from Script!",
    	MessageType: "Greetings"
	};

	_Message_Send(Message);
}

function _DoNothing()
{
	var Message =
	{
    	Message: "",
    	MessageType: "_DoNothing"
	};

	_Message_Send(Message);
}

function _Handle_Greetings(P_MessageObject)
{
    
	document.getElementById("Div_Greetings").innerHTML = P_MessageObject.Message;
}

function _Handle_DoNothing(P_MessageObject)
{
	alert(P_MessageObject.Message);
}
```

Here we have 4 functions. The first 2 will send a message to the backend while the last 2 are handlers for the response from the server which will be called by our _Message_Receive function in our Hub script file.

Last thing we need to do is update our HTML a little to make use of the code we just added. It’s quite simple, so just edit the file to look like below.

![image](https://github.com/Nicks182/PhotinoWebServerSignalR/assets/13113785/eeb24fc6-6355-4f39-b204-3acac9ce18cd)

### 14. Test it again

You should be able to run the app at this point and if AllowLocalAccess is set to true you should also be able to access the app from your local browser as well as the browser on your phone. Just use the IP address of the machine the app is running on along with the port. The port should normally just be 8000. So you should be able to use something like:

http://yourIP:8000

Note that the browser will complain about this not being a secure connection. This is because the connection is not encrypted. For that we will need to use HTTPS, but for your own app you connect to locally this should be fine.


### 15. Publish it.

Right click the project and select Publish.

![image](https://github.com/Nicks182/PhotinoCustomFileServer/assets/13113785/eb8436cd-dfd2-4143-9266-d4455b552475)

A new window will open up. Select Folder, click Next, select Folder again, click Next again. Now click the Browse button and choose where you would like to publish the files. Click Finish, then Close. You should see the image below…

![image](https://github.com/Nicks182/PhotinoCustomFileServer/assets/13113785/f9683faf-c14f-4c30-bfc9-964c92675c9b)

Now click Show all settings and copy the image below.
Deployment mode = Self-contained. This means you won’t need to install .net on the system you want to run the app on as it will contain all it needs to do so on its own.

Target Runtime = win-x64. This needs to match the OS you wish to run the app on. I’m on Windows 10 x64 so I need to select win-x64.

Produce single file = true. So we create a single exe file.

Trim unused code: For this project we want to leave this disabled. This will make our app larger, but we need the extra stuff in this case for our serialisations to work. There are ways to fix this and still use the Trim feature, but that is outside the scope of this tutorial.

![image](https://github.com/Nicks182/PhotinoCustomFileServer/assets/13113785/61d3c1f7-b5b1-446b-85cc-ac7a12e2d4f5)

Click save and then click the big Publish button at the top of the screen. Once done you should see the following 4 files in your publish folder.

![image](https://github.com/Nicks182/PhotinoCustomFileServer/assets/13113785/587aff1d-6b06-4fbb-b6ea-3830c8ed824c)

Yes… it’s not really a single file. Depending on the dll, it can’t always be put into a single file. In this case I believe it’s because they are C++. The .pdb file is not needed to run the app though.
Double click the exe to run the app.
## Additional info
I often end up having a lot of small individual script and style files which I end up bundling together.

To bundle your files you can use something like: [BundlerMinifier](https://github.com/madskristensen/BundlerMinifier)

I’m currently using a more up to date version: [BundlerMinifierPlus](https://github.com/salarcode/BundlerMinifierPlus)
