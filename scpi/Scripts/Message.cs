namespace scpi;

public class Message
{
    private string text;
    private string signature;
    private string path;

    public Message(string text, string signature, string path)
    {
        this.text = text;
        this.signature = signature;
        this.path = path;
    }

    public string GetText()
    {
        return text;
    }
}