namespace GraphShield.Proxy.Models
{
    /// <summary>
    /// Represents the configuration of an instance.
    /// </summary>
    internal class InstanceConfiguration
    {
        /// <summary>
        /// Represents the cluster type of the instance.
        /// </summary>
        public enum ClusterType
        {
            /// <summary>
            /// The instance is standalone.
            /// </summary>
            Standalone,

            /// <summary>
            /// The instance is part of a cluster.
            /// </summary>
            Cluster
        }

        /// <summary>
        /// Gets or sets the cluster type of the instance.
        /// </summary>
        public ClusterType Type { get; set; }

        /// <summary>
        /// Gets or sets the bus address for the instance.
        /// </summary>
        public string BusAddress { get; set; }

        /// <summary>
        /// Gets or sets the bus port for the instance.
        /// </summary>
        public int BusPort { get; set; }

        /// <summary>
        /// Gets or sets the list of bus peers for the instance.
        /// </summary>
        public List<Uri> BusPeers { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceConfiguration"/> class.
        /// </summary>
        public InstanceConfiguration()
        {
            Type = ClusterType.Cluster;
            BusAddress = "127.0.0.1";
            BusPort = 19796;
            BusPeers = new List<Uri>();
        }
    }
}