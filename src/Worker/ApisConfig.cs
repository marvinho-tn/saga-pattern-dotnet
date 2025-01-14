namespace Worker;

public class ApisConfig
{
    public required Api Order { get; set; }
    public required Api Inventory { get; set; }
    public required Api Payment { get; set; }

    public class Api
    {
        public required string Url { get; set; }
    }
}