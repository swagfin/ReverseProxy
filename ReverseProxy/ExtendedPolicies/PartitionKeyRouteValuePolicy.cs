using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Logging;
using ReverseProxy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

namespace ReverseProxy.ExtendedPolicies
{
    public class PartitionKeyRouteValuePolicy : ILoadBalancingPolicy
    {
        private readonly ILogger<IPAddressPolicy> logger;

        public string Name => "PartitionKeyRouteValue";

        public PartitionKeyRouteValuePolicy(ILogger<IPAddressPolicy> logger)
        {
            this.logger = logger;

        }

        public DestinationState PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
        {
            try
            {
                var routePathTemplate = "/{partitionKey}/{**catch-all}"; //e.g. test.com/9999/cars || test.com/5555/cars
                var partitionBasedOn = "partitionKey";
                var routeDictionary = Match(routePathTemplate, context.Request.Path.Value);
                if (routeDictionary.ContainsKey(partitionBasedOn))
                {
                    var valueToPartitionOn = routeDictionary[partitionBasedOn].ToString(); //Unique Value of Partition Key e.g. 9999 \\use unique data like deviceCode, Machine Code etc
                    int partitionIndex = Partitioner.CalculatePartitionIndex(valueToPartitionOn, availableDestinations.Count);
                    logger.LogInformation($"Request will be routed to: {availableDestinations[partitionIndex].Model.Config.Address}");
                    return availableDestinations.OrderBy(d => d.Model.Config.Address).ElementAt(partitionIndex);
                }
                //Error
                throw new Exception($"Could not partition request with path: {context.Request.Path.Value} against template {routePathTemplate} for key {partitionBasedOn}");
            }
            catch (Exception ex)
            {
                return new DestinationState($"could-not-partition-request-by-PartitionKeyQueryValue|{ex.Message}");
            }
        }


        private RouteValueDictionary Match(string routeTemplate, string requestPath)
        {
            var template = TemplateParser.Parse(routeTemplate);
            var matcher = new TemplateMatcher(template, GetDefaults(template));
            RouteValueDictionary routeValues = new RouteValueDictionary();
            bool routeMatchesTemplate = matcher.TryMatch(requestPath, routeValues);
            return routeValues;
        }

        private RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();
            foreach (var parameter in parsedTemplate.Parameters)
                if (parameter.DefaultValue != null)
                    result.Add(parameter.Name, parameter.DefaultValue);
            return result;
        }


    }
}
