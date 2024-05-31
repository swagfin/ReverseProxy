using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReverseProxy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

namespace ReverseProxy.ExtendedPolicies
{
    public sealed class IPAddressPolicy : ILoadBalancingPolicy
    {
        private readonly ILogger<IPAddressPolicy> logger;
        public string Name => "IPAddress";

        public IPAddressPolicy(ILogger<IPAddressPolicy> logger)
        {
            this.logger = logger;
        }

        public DestinationState PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
        {
            try
            {
                string ipAddress = context.Connection.RemoteIpAddress.ToString();
                int partitionIndex = Common.CalculatePartitionIndex(ipAddress, availableDestinations.Count);
                //Some Tests
                /*
                var test = CalculatePartitionIndex("192.168.1.10", availableDestinations.Count);
                test = CalculatePartitionIndex("192.168.1.10", availableDestinations.Count);
                test = CalculatePartitionIndex("192.168.1.11", availableDestinations.Count);
                test = CalculatePartitionIndex("192.168.1.12", availableDestinations.Count);

                test = CalculatePartitionIndex("41.90.64.33", availableDestinations.Count);
                test = CalculatePartitionIndex("10.20.20.1", availableDestinations.Count);
                test = CalculatePartitionIndex("41.90.64.33", availableDestinations.Count);
                test = CalculatePartitionIndex("41.90.64.34", availableDestinations.Count);
                */
                logger.LogInformation($"partitionigLoadbalancer chose partition {partitionIndex} for IP Address {ipAddress}");
                logger.LogInformation($"request will be routed to: {availableDestinations[partitionIndex].Model.Config.Address}");
                return availableDestinations.OrderBy(d => d.Model.Config.Address).ElementAt(partitionIndex);
            }
            catch (Exception ex)
            {
                return new DestinationState($"could-not-partition-request-by-ip|{ex.Message}");
            }
        }
    }
}
