using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Kong.Models
{
    public class ConsumerJwt
    {
        public string algorithm { get; set; } = "HS256";
        public string rsa_public_key { get; set; }
    }
}
