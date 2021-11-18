# **Matrix .NET SDK**

This open-source library allows you to build .NET apps compatible with [Matrix Protocol](http://www.matrix.org).
It has support for a limited subset of the APIs presently. 

This SDK was built for interaction with the [Beacon Node](https://github.com/airgap-it/beacon-node). It supports login through the [`crypto_auth_provider.py`](https://github.com/airgap-it/beacon-node/blob/master/docker/crypto_auth_provider.py).  


# Use the SDK in your app
For a complete example, refer to [`SimpleExample.cs`](https://github.com/baking-bad/matrix-dotnet-sdk/blob/main/Matrix.Sdk.Sample.Console/SimpleExample.cs).
You can also clone this repository and run `Matrix.Examples.ConsoleApp`.

Here is step by step guide:

## 1. Create 
Use MatrixClientFactory to create an instance of `MatrixClient`
```cs
var factory = new MatrixClientFactory();
IMatrixClient client = factory.Create();
```

## 2. Login
Currently, `MatrixClient` supports only [password login](https://spec.matrix.org/v1.1/client-server-api/#password-based).

```cs
await client.LoginAsync(matrixNodeAddress, username, password, deviceId);
```

## 3. Start listening for incoming events
To listen for the incoming Matrix room events you need to subscribe to `OnMatrixRoomEventsReceived;`

```cs
client.OnMatrixRoomEventsReceived += (sender, eventArgs) =>
{
    foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
    {
        if (roomEvent is not TextMessageEvent textMessageEvent)
            continue;

        (string roomId, string senderUserId, string message) = textMessageEvent;
        if (client.UserId != senderUserId)
            Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
    }
};
```

I recommend that you don't use anonymous functions to subscribe to events if you have to unsubscribe from the event at some later point in your code.

Then you should **start** `MatrixClient`
```cs
client.Start();
```
If you need to **stop** listening, for example, when the app is suspended, then do the following
```cs
client.Stop();
```
## 4. Basic functions

```cs
// Create room
MatrixRoom matrixRoom = await client.CreateTrustedPrivateRoomAsync(new[]
{
    anotherClient.UserId
});

// Join room
await anotherClient.JoinTrustedPrivateRoomAsync(matrixRoom.Id);

// Send message
await client.SendMessageAsync(matrixRoom.Id, "some message");

//Get joined rooms ids
await client.GetJoinedRoomsIdsAsync();

// Leave room
await client.LeaveRoomAsync(roomId);
```
