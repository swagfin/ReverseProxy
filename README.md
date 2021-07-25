# ReverseProxy
Simple Reverse Proxy Using Yarp

YARP ships with the following built-in policies:

    First

    Select the alphabetically first available destination without considering load. This is useful for dual destination fail-over systems.

    Random

    Select a destination randomly.

    PowerOfTwoChoices (default)

    Select two random destinations and then select the one with the least assigned requests. This avoids the overhead of LeastRequests and the worst case for Random where it selects a busy destination.

    RoundRobin

    Select a destination by cycling through them in order.

    LeastRequests

    Select the destination with the least assigned requests. This requires examining all destinations.
