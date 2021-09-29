﻿using System.Collections.Generic;

namespace Horse.Messaging.Server.Cluster
{
    /// <summary>
    /// Horse Clustering Mode
    /// </summary>
    public enum ClusterMode
    {
        /// <summary>
        /// Cluster works for reliability.
        /// One mode is choosen as main, others replica.
        /// All clients are connected to main node and queue messages are replicated.
        /// If main is down, another main node is choosen.
        /// </summary>
        Reliable,
        
        /// <summary>
        /// Cluster works for horizontal scaling.
        /// Clients are connected to different nodes in cluster.
        /// Queue messages are kept in only one node.
        /// Other messaging operations are distributed among all nodes. 
        /// </summary>
        Scaled
    }

    /// <summary>
    /// Horse Messaging Cluster Options
    /// </summary>
    public class ClusterOptions
    {
        /// <summary>
        /// Cluster mode
        /// </summary>
        public ClusterMode Mode { get; set; }
        
        /// <summary>
        /// Node name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Node internal host name such as horse://localhost:12345
        /// </summary>
        public string NodeHost { get; set; }
        
        /// <summary>
        /// Node public host. Clients are connected to the node with that endpoint.
        /// </summary>
        public string PublicHost { get; set; }
        
        /// <summary>
        /// Shared secret between nodes to authenticate them each other.
        /// </summary>
        public string SharedSecret { get; set; }

        /// <summary>
        /// Other node definitions in the cluster.
        /// </summary>
        public List<NodeInfo> Nodes { get; } = new();
    }
}