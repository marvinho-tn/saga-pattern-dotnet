namespace Orchestrator;

public class ApisConfig
{
    public Api Order { get; set; }
    public Api Inventory { get; set; }
    public Api Payment { get; set; }

    public class Api
    {
        public string Url { get; set; }
    }
}