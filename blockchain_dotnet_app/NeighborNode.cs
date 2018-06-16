using System;
using System.Collections.Generic;
using System.Text;

namespace blockchain_dotnet_app
{
    class NeighborNode : Node
    {
        public NeighborNode(string ip, string id, string type)
        {
            this.ip = ip;
            this.id = id;
            if(type == "normal" | type == "supplychain" | type == "supplychain_miner")
            {
                this.type = type;
            }

        }
    }
}
