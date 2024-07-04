# ReverseProxy

A super blazing-fast reverse proxy built using Microsoft YARP (Yet Another Reverse Proxy).
Also includes custom load balancing policies in addition to the pre-built policies provided by YARP.

## Features

- **Blazing Fast:** High performance reverse proxy for your applications.
- **Custom Load Balancing Policies:** Tailor the load balancing to your specific needs.
- **Pre-Built Policies:** Use a variety of built-in load balancing strategies.

## Custom Load Balancing Policies

YARP allows you to define custom load balancing policies to suit your needs. This project includes:

- **IPAddress:** Balances requests based on the client's IP address.
- **PartitionKeyQueryValue:** Balances requests based on a query parameter (e.g., `?balanceBy={{value}}`).
- **PartitionKeyRouteValue:** Balances requests based on a route parameter (e.g., `/api/{{balanceBy}}`).

## Pre-Built Policies

YARP ships with the following built-in policies:

- **RoundRobin:** Selects a destination by cycling through them in order.
- **Random:** Selects a destination randomly.
- **First:** Selects the alphabetically first available destination without considering load. Useful for dual destination fail-over systems.
- **PowerOfTwoChoices (default):** Selects two random destinations and then picks the one with the least assigned requests. This avoids the overhead of LeastRequests and the worst case for Random where it selects a busy destination.
- **LeastRequests:** Selects the destination with the least assigned requests, which requires examining all destinations.

## Configuration

You can configure the reverse proxy by modifying the `appsettings.json` file. Refer to the [YARP documentation](https://microsoft.github.io/reverse-proxy/articles/config-files.html) for detailed configuration options.

```json
{
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}"
        }
      }
    },
    "Clusters": {
      "cluster1": {
        "LoadBalancingPolicy": "RoundRobin",
        "Destinations": {
          "cluster1/destination1": {
            "Address": "https://server-1/"
          },
          "cluster1/destination2": {
            "Address": "https://server-2/"
          },
          "cluster1/destination3": {
            "Address": "https://server-3/"
          },
          "cluster1/destination4": {
            "Address": "https://server-4/"
          }
        }
      }
    }
  }
}
```
## Contributing

Contributions are welcome! Please fork the repository and open a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
