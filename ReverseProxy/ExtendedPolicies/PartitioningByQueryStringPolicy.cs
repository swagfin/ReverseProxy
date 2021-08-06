using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using ReverseProxy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

namespace ReverseProxy.ExtendedPolicies
{
    public class PartitioningByQueryStringPolicy : ILoadBalancingPolicy
    {
        private readonly ILogger<PartitioningByIPAddressPolicy> logger;

        public string Name => "PartitioningByQueryBased";

        public PartitioningByQueryStringPolicy(ILogger<PartitioningByIPAddressPolicy> logger)
        {
            this.logger = logger;

        }

        public DestinationState PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
        {

            try
            {
                var partitionIndex = 0; //initialize to zero


                var queryValue = context.Request.Query.TryGetValue("user", out var value); //using routevalues.TryGetValue
                if (queryValue)
                {
                    partitionIndex = Partitioner.CalculatePartitionIndex(value.ToString(), availableDestinations.Count);
                }

                logger.LogInformation($"Request will be routed to: {availableDestinations[partitionIndex].Model.Config.Address}");

                return availableDestinations.OrderBy(d => d.Model.Config.Address).ElementAt(partitionIndex);
            }

            catch (Exception ex)
            {
                return new DestinationState($"could-not-partition-request-by-ip|{ex.Message}");
            }
        }

    }
}
