namespace Matrix.Sdk.Core.Infrastructure.Dto.ClientVersion
{
    using System.Collections.Generic;

    public class MatrixServerVersionsResponse
    {
        public List<string> versions { get; set; } = default!;
        public UnstableFeatures unstable_features { get; set; } = default!;
    }
}