namespace Matrix.Sdk.Core.Infrastructure.Dto.ClientVersion
{
    using Newtonsoft.Json;

    public class UnstableFeatures
    {
        [JsonProperty("org.matrix.label_based_filtering")]
        public bool OrgMatrixLabelBasedFiltering { get; set; }

        [JsonProperty("org.matrix.e2e_cross_signing")]
        public bool OrgMatrixE2eCrossSigning { get; set; }

        [JsonProperty("org.matrix.msc2432")] public bool OrgMatrixMsc2432 { get; set; }

        [JsonProperty("uk.half-shot.msc2666")] public bool UkHalfShotMsc2666 { get; set; }

        [JsonProperty("io.element.e2ee_forced.public")]
        public bool IoElementE2eeForcedPublic { get; set; }

        [JsonProperty("io.element.e2ee_forced.private")]
        public bool IoElementE2eeForcedPrivate { get; set; }

        [JsonProperty("io.element.e2ee_forced.trusted_private")]
        public bool IoElementE2eeForcedTrustedPrivate { get; set; }

        [JsonProperty("org.matrix.msc3026.busy_presence")]
        public bool OrgMatrixMsc3026BusyPresence { get; set; }
    }
}