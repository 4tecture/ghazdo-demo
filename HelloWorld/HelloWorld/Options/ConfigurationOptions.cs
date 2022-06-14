using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorld.Options
{
    public class ConfigurationOptions
    {
        public string Message { get; internal set; }
        public string DeploymentEnvironment { get; internal set; }
    }
}
