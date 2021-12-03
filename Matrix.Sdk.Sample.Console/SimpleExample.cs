namespace Matrix.Sdk.Sample.Console
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Domain.MatrixRoom;
    using Core.Domain.RoomEvent;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;
    using Sodium;

    public class SimpleExample
    {
        private static readonly CryptographyService CryptographyService = new();

        public record LoginRequest(Uri BaseAddress, string Username, string Password, string DeviceId);

        private static LoginRequest CreateLoginRequest()
        {
            var seed = Guid.NewGuid().ToString();
            KeyPair keyPair = CryptographyService.GenerateEd25519KeyPair(seed);

            byte[] loginDigest = CryptographyService.GenerateLoginDigest();
            string hexSignature = CryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = CryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = CryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;

            var baseAddress = new Uri("https://beacon-node-0.papers.tech:8448/");

            
            LoginRequest loginRequest = new LoginRequest(baseAddress, hexId, password, deviceId);

            return loginRequest;
        }

        public async Task Run()
        {
            SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            var factory = new MatrixClientFactory();
            var anotherFactory = new MatrixClientFactory();

            IMatrixClient client = factory.Create();
            IMatrixClient anotherClient = anotherFactory.Create();

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

            anotherClient.OnMatrixRoomEventsReceived += (sender, eventArgs) =>
            {
                foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
                {
                    if (roomEvent is not TextMessageEvent textMessageEvent)
                        continue;

                    (string roomId, string senderUserId, string message) = textMessageEvent;
                    if (anotherClient.UserId != senderUserId)
                        Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
                }
            };

            (Uri matrixNodeAddress, string username, string password, string deviceId) = CreateLoginRequest();
            await client.LoginAsync(matrixNodeAddress, username, password, deviceId);
            
            LoginRequest request2 = CreateLoginRequest();
            await anotherClient.LoginAsync(request2.BaseAddress, request2.Username, request2.Password, request2.DeviceId);

            client.Start();
            anotherClient.Start();
            
            MatrixRoom matrixRoom = await client.CreateTrustedPrivateRoomAsync(new[]
            {
                anotherClient.UserId
            });
            
            await anotherClient.JoinTrustedPrivateRoomAsync(matrixRoom.Id);
            
            var spin = new SpinWait();
            while (anotherClient.JoinedRooms.Length == 0)
                spin.SpinOnce();
            
            await client.SendMessageAsync(matrixRoom.Id, "Hello");
            await anotherClient.SendMessageAsync(anotherClient.JoinedRooms[0].Id, ", ");
            
            await client.SendMessageAsync(matrixRoom.Id, "World");
            await anotherClient.SendMessageAsync(anotherClient.JoinedRooms[0].Id, "!");

            // await client.GetJoinedRoomsIdsAsync();
            // string roomId = string.Empty;
            // await client.LeaveRoomAsync(roomId);
            Console.ReadLine();
            
            client.Stop();
            anotherClient.Stop();
        }
    }
}