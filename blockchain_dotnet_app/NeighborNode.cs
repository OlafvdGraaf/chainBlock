using System;
using System.Collections.Generic;
using System.Text;

namespace blockchain_dotnet_app
{
    public class NeighborNode : Node
    {
        public NeighborNode(string ip, string id, string type)
        {
            this.ip = ip;
            this.id = id;
            if(type == "supplychain" | type == "supplychain_miner")
            {
                this.type = type;
            }
            else
            {
                this.type = "normal";
            }

        }
    }
}
