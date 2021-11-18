namespace Matrix.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;

    internal interface ISeedRepository
    {
        Task<Guid> GetSeed();
    }

    internal sealed class MemorySeedRepository : ISeedRepository
    {
        private static readonly Guid Seed = Guid.NewGuid(); // Todo: refactor

        public Task<Guid> GetSeed() => Task.FromResult(Seed);
    }
}