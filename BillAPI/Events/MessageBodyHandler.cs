using RabbitMQ.EventBus.AspNetCore.Events;

namespace BillAPI.Events
{
    //public class MessageBodyHandler : IEventResponseHandler<MessageBody, string>, IDisposable
    //{
    //    private Guid id;
    //    private readonly ILogger<MessageBodyHandler> _logger;

    //    public MessageBodyHandler(ILogger<MessageBodyHandler> logger)
    //    {
    //        id = Guid.NewGuid();
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }
    //    public void Dispose()
    //    {
    //        _logger.LogInformation("MessageBodyHandle Disposable.");
    //    }


    //    public Task<string> HandleAsync(HandlerEventArgs<MessageBody> args)
    //    {
    //        return Task.FromResult("收到消息，已确认" + DateTimeOffset.Now);
    //    }
    //}
}
