using System;
using VirtualRtu.Common.Configuration;

namespace EdgeLoggingServerModule.Connections
{
    public class ModuleConfigurationEventArgs : EventArgs
    {
        public ModuleConfigurationEventArgs(IssuedConfig config)
        {
            Config = config;   
        }

        public IssuedConfig Config { get; internal set; }
    }
}
