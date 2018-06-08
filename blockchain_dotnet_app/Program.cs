using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace blockchain_dotnet_app
{
    class Program
    {
        // Create name for node
        // replace with creating a new node, with a public key creation in its contructor
        public static string node_identifier = "127.0.0.1:5000";

        //Instantiate The Chain
        public static Blockchain blockchain = new Blockchain();

        static void Main(string[] args)
        {
            //Instantiate Node
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();

        }
        
    }
}
