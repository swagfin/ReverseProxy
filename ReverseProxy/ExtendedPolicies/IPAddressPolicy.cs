using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
                var partitionIndex = CalculatePartitionIndex(ipAddress, availableDestinations.Count);
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
                logger.LogInformation($"PartitioninLoadbalancer chose partition {partitionIndex} for IP Address {ipAddress}");
                logger.LogInformation($"Request will be routed to: {availableDestinations[partitionIndex].Model.Config.Address}");
                return availableDestinations.OrderBy(d => d.Model.Config.Address).ElementAt(partitionIndex);
            }
            catch (Exception ex)
            {
                return new DestinationState($"could-not-partition-request-by-ip|{ex.Message}");
            }
        }

        private int CalculatePartitionIndex(string valueToPartitionOn, int maxNumberOfPartitions)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(valueToPartitionOn);
                byte[] hashedBytes = md5.ComputeHash(inputBytes);
                //Use First Byte
                byte firstByte = hashedBytes[0];
                int unMappedPartitionNr = firstByte;
                unMappedPartitionNr = Math.Abs(unMappedPartitionNr);
                int partitionIndex = unMappedPartitionNr % maxNumberOfPartitions;
                return partitionIndex;
            }
        }
    }
}
