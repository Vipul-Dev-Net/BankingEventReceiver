What things did you considered of during the implementation?
I considered to persist the event messages so that we dont loose those events. Since consuming and processing on data are two different
async process, so implemented inbox table pattern to read new events asynchronuously and consuming those events asynchronously. Kind of followed choreography pattern to achieve this solution.
Anything was unclear?