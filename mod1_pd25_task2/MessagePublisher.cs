namespace mod1_pd25_task2;

public class MessagePublisher
{
    public event Action<string>? MessageSent;

    public void Send(string message)
    {
        MessageSent?.Invoke(message);
    }
}
