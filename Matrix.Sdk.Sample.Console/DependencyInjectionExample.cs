namespace Matrix.Sdk.Sample.Console
{
    using System.Threading.Tasks;

    public class DependencyInjectionExample
    {
        public async Task Run()
        {
        }
    }
}

// try
// {
//     ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
//
//     logger.LogInformation("START");
//
//     await RunAsync(host.Services);
// }
// catch (Exception ex)
// {
//     ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
//
//     logger.LogError(ex, "An error occurred");
// }


// private static async Task RunAsync(IServiceProvider serviceProvider)
// {
// (IMatrixClient firstClient, TextMessageListener firstListener) =
//     await SetupClientWithTextListener(serviceProvider);
//
// (IMatrixClient secondClient, TextMessageListener secondListener) =
//     await SetupClientWithTextListener(serviceProvider);
//
// MatrixRoom firstClientMatrixRoom = await firstClient.CreateTrustedPrivateRoomAsync(new[]
// {
//     secondClient.UserId
// });
//
// MatrixRoom matrixRoom = await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);
//
// var spin = new SpinWait();
// while (secondClient.JoinedRooms.Length == 0)
//     spin.SpinOnce();
//
// await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "Hello");
// await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, ", ");
//
// await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "World");
// await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, "!");
//
// Console.ReadLine();
//
// firstClient.Stop();
// secondClient.Stop();
//
// firstListener.Unsubscribe();
// secondListener.Unsubscribe();
// }

// private static async Task<(IMatrixClient, TextMessageListener)> SetupClientWithTextListener(
//     IServiceProvider serviceProvider)
// {
//     // IMatrixClient matrixClient = serviceProvider.GetRequiredService<IMatrixClient>();
//     // ICryptographyService cryptographyService = serviceProvider.GetRequiredService<ICryptographyService>();
//     //
//     // var seed = Guid.NewGuid().ToString();
//     // KeyPair keyPair = cryptographyService.GenerateEd25519KeyPair(seed);
//     // var nodeAddress = new Uri(Constants.FallBackNodeAddress);
//     //
//     // await matrixClient.LoginAsync(nodeAddress, keyPair); //Todo: generate once and then store seed?
//     // matrixClient.Start();
//     //
//     // var textMessageListener = new TextMessageListener(matrixClient.UserId, (listenerId, textMessageEvent) =>
//     // {
//     //     (string roomId, string senderUserId, string message) = textMessageEvent;
//     //     if (listenerId != senderUserId)
//     //         Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
//     // });
//     //
//     // textMessageListener.ListenTo(matrixClient.MatrixEventNotifier);
//     //
//     // return (matrixClient, textMessageListener);
// }