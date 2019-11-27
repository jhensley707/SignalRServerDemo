# SignalRServerDemo

A Visual Studio solution of ASP.NET Core 2.2 for a SignalR Server application hosted in a Web API. All clients that connect to the server will receive progress notifications.

The default API method is /api/values. When invoked, it triggers a background task queue

The purpose of this application was to establish a) a WebAPI web service framework, b) a background process for long-running tasks, and c) a SignalR push notification method to keep clients appraised of the progress of the task.

This example is an adaptation of Microsoft tutorials.

[SignalR Javascript Client Tutorial](https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-2.2)

[SignalR Hubs](https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-2.2)

[SignalR Outside Hubs Messaging](https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-2.2)

[ASP.NET Core Hosted Background Service](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2&tabs=visual-studio)

[SignalR Samples](https://github.com/aspnet/SignalR-samples)

ASP.NET Core 2.2 was the first version with SignalR integrated in the SDK.