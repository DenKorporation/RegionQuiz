namespace ClientLib;

public class Message
{ 
    public string Sender { get; set; }
    public string Text { get; set; }
    public string Time { get; set; }
    public bool IsService { get; set; }

    public Message()
    { }

    public Message(string sender, string text) : this(sender, text, DateTime.Now.ToShortTimeString())
    {
    }

    public Message(string sender, string text, string time, bool isService = false)
    {
        Sender = sender;
        Text = text;
        Time = time;
        IsService = isService;
    }
}