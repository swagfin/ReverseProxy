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
    public class PartitionKeyQueryValuePolicy : ILoadBalancingPolicy
    {
        private readonly ILogger<IPAddressPolicy> logger;

        public string Name => "PartitionKeyQueryValue";

        public PartitionKeyQueryValuePolicy(ILogger<IPAddressPolicy> logger)
        {
            this.logger = logger;

        }

        public DestinationState PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
        {

            try
            {
                if (context.Request.Query.TryGetValue("partitionKey", out var value))
                {
                    int partitionIndex = Partitioner.CalculatePartitionIndex(value.ToString(), availableDestinations.Count);
                    logger.LogInformation($"Request will be routed to: {availableDestinations[partitionIndex].Model.Config.Address}");
                    return availableDestinations.OrderBy(d => d.Model.Config.Address).ElementAt(partitionIndex);
                }
                //Error
                throw new Exception($"partitionKey GET parameter was not provided.");
            }
            catch (Exception ex)
            {
                return new DestinationState($"could-not-partition-request-by-PartitionKeyRouteValue|{ex.Message}");
            }

        }
    }
}
