namespace Ketchup.Core.Kong.Attribute
{
    public class KongRouteAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string[] Hosts { get; set; }
        public string[] Methods { get; set; } = new[] { "POST" };
        public string[] Protocols { get; set; } = new[] { "http" };
        public int Https_redirect_status_code { get; set; } = 426;
        public string KongServiceName { get; set; } = "gateway";
        public string[] Paths { get; set; }
        public string[] Tags { get; set; }
    }
}
