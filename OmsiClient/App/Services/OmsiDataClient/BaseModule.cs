namespace OmsiClient.App.Services.OmsiDataClient;

public abstract class BaseModule
{
    protected BaseModule(OmsiDataService client)
    {
        Client = client;
    }

    public OmsiDataService Client { get; set; }

    public abstract Task Run();
    public abstract Task MapChange();
}