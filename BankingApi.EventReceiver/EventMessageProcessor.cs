using BankingApi.EventReceiver.Exceptions;
using BankingApi.EventReceiver.Utility;
using System.Text.Json;

namespace BankingApi.EventReceiver
{
    public class EventMessageProcessor : IEventMessageProcessor
    {
        private readonly ITransactionProcessor _transactionProcessor;
        private readonly IServiceBusReceiver serviceBusReceiver;

        public EventMessageProcessor(ITransactionProcessor transactionProcessor, IServiceBusReceiver serviceBusReceiver)
        {
            _transactionProcessor = transactionProcessor;
            this.serviceBusReceiver = serviceBusReceiver;
        }


        public Task StartProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) 
            { 
                var eventMessageDbContext = new EventMessageDbContext();
                var messagesToProcess = eventMessageDbContext.EventMessages.Where( message => message.IsProcessed == false && message.ProcessingCount ==0 );
                foreach ( var message in messagesToProcess ) 
                {
                    if(message.MessageBody != null)
                    {
                        try
                        {
                            Process(message, eventMessageDbContext);
                        }
                        catch (JsonException)
                        {
                            MoveMessageToDeadLetter(message, eventMessageDbContext);
                            //log mesage
                        }
                        catch (BusinessLogicException)
                        {
                            MoveMessageToDeadLetter(message, eventMessageDbContext);
                            //log mesage
                        }
                        catch (Exception)
                        {
                            message.ProcessingCount += 1;
                            eventMessageDbContext.Update(message);
                            eventMessageDbContext.SaveChanges();
                            Retry(message, eventMessageDbContext);
                            // Log this message
                        }
                    }
                    
                }
            }
            return Task.CompletedTask;  
        }

        private void Process(EventMessage message, EventMessageDbContext eventMessageDbContext)
        {
            var transaction = JsonSerializer.Deserialize<Transaction>(message.MessageBody);
            if (transaction != null)
            {
                _transactionProcessor.ProcessTransaction(transaction);
            }
            message.IsProcessed = true;
            eventMessageDbContext.Update(message);
            eventMessageDbContext.SaveChanges();

        }

        private void Retry(EventMessage message, EventMessageDbContext eventMessageDbContext)
        {
            var timeInSec = 5;
            for(int i=0; i<3;)
            {
                var timeSpan = TimeSpan.FromSeconds(timeInSec);
                try
                {
                    message.ProcessingCount += 1;
                    var task = Task.Run(() => { RetryHelper.Retry(() => Process(message, eventMessageDbContext), timeSpan); return Task.CompletedTask; });
                    task.Wait();
                    i = 3;
                }
                catch (Exception)
                {
                    i++;
                    timeInSec = timeInSec * timeInSec;
                    
                    eventMessageDbContext.Update(message);
                    eventMessageDbContext.SaveChanges();
                }
            }
        }

        private void MoveMessageToDeadLetter(EventMessage message, EventMessageDbContext eventMessageDbContext)
        {
            serviceBusReceiver.MoveToDeadLetter(message);
            eventMessageDbContext.Remove(message);
            eventMessageDbContext.SaveChanges();
        }
        
    }
}
