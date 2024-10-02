namespace BankingApi.EventReceiver
{
    public class MessageWorker
    {
        private readonly IServiceBusReceiver _serviceBusReceiver;
        private bool continueConsuming;
        private const int TimeToWaitWhenNoMessagesInMiliSeconds = 10000;
        public MessageWorker(IServiceBusReceiver serviceBusReceiver)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            _serviceBusReceiver = serviceBusReceiver;
            var transactionProcessor = new TransactionProcessor();
            var eventMessageProcessor = new EventMessageProcessor(transactionProcessor, _serviceBusReceiver);

            Task.Run(async () =>
            {
                await eventMessageProcessor.StartProcessing(cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);
        }

        public Task Start()
        {
            // Implement logic to listen to messages here
            continueConsuming = true;
            while (continueConsuming)
            {
                var eventMessage = _serviceBusReceiver.Peek();
                if(eventMessage != null)
                {
                    var eventMessageDbContext = new EventMessageDbContext();
                    eventMessageDbContext.Add(eventMessage);
                    eventMessageDbContext.SaveChanges();
                }
                else 
                {
                    Task.Delay(TimeToWaitWhenNoMessagesInMiliSeconds);
                }
            }
            return Task.CompletedTask;  
        }

        public Task Stop()
        {
            continueConsuming = false;

            return Task.CompletedTask;
        }
    }
}
