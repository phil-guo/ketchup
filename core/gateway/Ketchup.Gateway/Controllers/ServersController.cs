using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Gateway.Internal;
using Ketchup.Gateway.Internal.Filter;
using Ketchup.Gateway.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ketchup.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    public class ServersController : Controller
    {
        private readonly IServerProvider _server;
        private readonly IGatewayProvider _gatewayProvider;

        public ServersController(IServerProvider server, IGatewayProvider gatewayProvider)
        {
            _server = server;
            _gatewayProvider = gatewayProvider;
        }

        [HttpGet("api/servers/getAllServer")]
        [KetchupExceptionFilter]
        public async Task<object> GetAllServer()
        {
            return new KetchupResponse(await _server.GetAllServer());
        }

        [HttpGet("api/servers/getAllServerEntry")]
        [KetchupExceptionFilter]
        public async Task<object> GetAllServerEntry(string server, string service)
        {
            var nodes = GetAllXmlDocNodes();
            var entries = await _server.GetAllServerEntry(server, service);
            foreach (var item in entries)
            {
                var entry = _gatewayProvider.MethodDescriptors.FirstOrDefault(
                    method => method.FullName.ToLower() == item.FullName.ToLower());
                if (entry == null)
                    continue;

                var dictionary = new Dictionary<string, Dictionary<string, object>>
                {
                    {"request", new Dictionary<string, object>()},
                    {"response", new Dictionary<string, object>()}
                };


                entry.InputType.ClrType.GetProperties().ToList().ForEach(property =>
                {
                    if (property.Name == "Descriptor" || property.Name == "Parser")
                        return;

                    var nodeSummary = FindXmlDocNode(nodes, $"P:{entry.InputType.FullName}.{property.Name}");

                    if (property.PropertyType.ToString().Contains("Google.Protobuf.Collections.RepeatedField"))
                    {
                        var model = _gatewayProvider.MessageDescriptors.FirstOrDefault(item =>
                            item.FullName == property.PropertyType?.GetGenericArguments().FirstOrDefault()?.FullName);
                        if (model == null)
                            return;

                        var dic = new Dictionary<string, object>();

                        model.ClrType.GetProperties().ToList().ForEach(propertyInfo =>
                        {
                            if (propertyInfo.Name == "Descriptor" || propertyInfo.Name == "Parser")
                                return;

                            var childSummary = FindXmlDocNode(nodes, $"P:{model.FullName}.{propertyInfo.Name}");

                            dic.Add(propertyInfo.Name?.Substring(0, 1).ToLower() + propertyInfo.Name?.Substring(1),
                                $"{propertyInfo.PropertyType.ToString()} //{childSummary}");
                        });

                        dictionary["request"].Add(model.Name?.Substring(0, 1).ToLower() + model.Name?.Substring(1),
                            new List<Dictionary<string, object>>()
                            {
                                dic
                            });
                    }
                    else
                    {
                        dictionary["request"].Add(property.Name?.Substring(0, 1).ToLower() + property.Name?.Substring(1),
                           $"{property.PropertyType.ToString()} //{nodeSummary}");
                    }
                });

                entry.OutputType.ClrType.GetProperties().ToList().ForEach(property =>
                {
                    if (property.Name == "Descriptor" || property.Name == "Parser")
                        return;

                    var nodeSummary = FindXmlDocNode(nodes, $"P:{entry.InputType.FullName}.{property.Name}");

                    if (property.PropertyType.ToString().Contains("Google.Protobuf.Collections.RepeatedField"))
                    {
                        var model = _gatewayProvider.MessageDescriptors.FirstOrDefault(item =>
                            item.FullName == property.PropertyType?.GetGenericArguments().FirstOrDefault()?.FullName);
                        if (model == null)
                            return;

                        var dic = new Dictionary<string, object>();

                        model.ClrType.GetProperties().ToList().ForEach(propertyInfo =>
                        {
                            if (propertyInfo.Name == "Descriptor" || propertyInfo.Name == "Parser")
                                return;

                            var childSummary = FindXmlDocNode(nodes, $"P:{model.FullName}.{propertyInfo.Name}");


                            dic.Add(propertyInfo.Name?.Substring(0, 1).ToLower() + propertyInfo.Name?.Substring(1),
                                $"{propertyInfo.PropertyType.ToString()} //{childSummary}");
                        });

                        dictionary["response"].Add(model.Name?.Substring(0, 1).ToLower() + model.Name?.Substring(1),
                            new List<Dictionary<string, object>>()
                            {
                                dic
                            });
                    }
                    else
                    {
                        dictionary["response"].Add(property.Name?.Substring(0, 1).ToLower() + property.Name?.Substring(1),
                            $"{property.PropertyType.ToString()} //{nodeSummary}");
                    }
                });


                item.Parameter = JsonConvert.SerializeObject(dictionary);
            }

            return new KetchupResponse(entries);
        }

        private string FindXmlDocNode(IEnumerable<XElement> nodes, string member)
        {
            var node = nodes?.FirstOrDefault(item => item.FirstAttribute.Value.ToLower() == member.ToLower());

            if (node == null)
                return "";
            return node.Descendants("summary").FirstOrDefault()?.Value.Replace("/n", "").Trim();
        }

        private IEnumerable<XElement> GetAllXmlDocNodes()
        {
            var xml = XDocument.Load("Ketchup.Gateway.xml");
            var nodes = xml.Root?.Descendants("member");
            return nodes;
        }
    }
}
