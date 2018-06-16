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
        //Instantiate The Chain
        public static Blockchain blockchain = new Blockchain();

        // Create node
        public static Node node_identifier = new SelfNode("127.0.0.1:5000",true,true);

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
