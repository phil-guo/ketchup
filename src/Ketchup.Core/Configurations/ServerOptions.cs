namespace Ketchup.Core.Configurations
{
    public class ServerOptions
    {
        public string Ip { get; set; }

        public string Name { get; set; }

        public int Port { get; set; }

        /// <summary>
        ///     是否使用http
        /// </summary>
        public bool EnableHttp { get; set; } = true;
    }
}