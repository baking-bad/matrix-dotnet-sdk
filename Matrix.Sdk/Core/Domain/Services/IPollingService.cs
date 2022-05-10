namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using MatrixRoom;
    
    public interface IPollingService : IDisposable
    {
        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }
        
        public bool IsSyncing { get; }
        
        public event EventHandler<SyncBatchEventArgs> OnSyncBatchReceived;

        void Init(Uri nodeAddress, string accessToken);

        void Start(string? nextBatch = null);

        void Stop();

        MatrixRoom? GetMatrixRoom(string roomId);
    }
}