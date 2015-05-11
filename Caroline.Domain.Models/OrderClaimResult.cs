namespace Caroline.Domain.Models
{
    public struct OrderClaimResult
    {
        public VersionedUpdateResult Status { get; set; }
        public long NumItems { get; set; }
    }
}
