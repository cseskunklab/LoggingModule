namespace LoggingModule
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using LogModule;
    using LogModuleWrapper;
    using Microsoft.Azure.Devices.Client;

    class Program
    {

        static string storageAccountName = "loggingmodulestore";
        static string storageAccountKey = "PEY5ADgPE4Fg57f17jJyVRQJvbNU9CCrc2cf0x/8JfoNVDbuqZkkDr0A9Os2X27FNFfzv3dAirSra5pOBkzjEw==";

        static void Main(string[] args)
        {
            EdgeHubWrapper.Init(storageAccountName, storageAccountKey).Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}
