# RequestsScheduler

### This is a new implementation of Requests Service https://github.com/vangaru/RequestService/tree/master

## Overview
Requests scheduler generates applications for a seats in transport. Application contains datetime of a request,
origin, destination, unique identifier and number of seats. Origin and destination are numbers from defined range.
Application generation intensity may vary depending on the current time.

## Architecture in brief
Basically, there is 2 parts of the project:
1. **Requests scheduler**. Worker service that generates requests with intensity defined in config file and sends
requests to the RabbitMQ queue. Basically, request is just a application for a seat in transport. 
Request model is defined below as a JSON schema:
> { <br>
>   &nbsp;&nbsp;"id": "string", <br>
>   &nbsp;&nbsp;"seatsCount": "integer", <br>
>   &nbsp;&nbsp;"origin": "integer", <br>
>   &nbsp;&nbsp;"destination": "integer", <br>
>   &nbsp;&nbsp;"dateTime": "string", <br>
>   &nbsp;&nbsp;"status": "string" <br>
> }
2. **Requests receiver**. Worker service that subscribes to scheduler's queue and receives new requests from it.
After request is received, it is marked as received, and datetime is updated. After request is updated, it is added to
Postgres database and new RabbitMQ queue.