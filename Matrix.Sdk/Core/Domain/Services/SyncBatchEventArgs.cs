namespace Matrix.Sdk.Core.Domain.Services
{
    using System;

    public class SyncBatchEventArgs : EventArgs
    {
        public SyncBatchEventArgs(SyncBatch syncBatch)
        {
            SyncBatch = syncBatch;
        }

        public SyncBatch SyncBatch { get; }
    }
}