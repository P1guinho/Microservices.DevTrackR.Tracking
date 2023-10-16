namespace DevTrackR.Tracking.Infra.Messaging
{
    public interface IMessageBusService
    {
        void Publish(object data, string routingKey);
    }
}
