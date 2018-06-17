using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace blockchain_dotnet_app
{
    [Route("[controller]")]
    public class BlockchainController : Controller
    {
        [HttpGet("mine")]
        public JsonResult mine()
        {
            //get the next proof for the new block
            var last_block = Program.blockchain.getLastBlock();
            var last_proof = last_block.proof;
            var proof = Program.blockchain.proofOfWork(last_proof);

            //transaction for the block reward
            Transaction block_reward = new Transaction(new List<Product>(), "The ChainBlock foundation", Program.node_identifier.id, 10, 0, DateTime.Now);
            Program.blockchain.newTransaction(block_reward);

            //forge a new block and add it to the chain
            var hash = Program.blockchain.hash(last_block);
            var block = Program.blockchain.newBlock(proof, hash);

            //create response message with new block details
            var response = new Dictionary<string, dynamic>()
            {
                { "message", "New Block Forged" },
                { "index" , block.id },
                { "transactions", block.transactions },
                { "proof", block.proof },
                { "hash", block.hash },
            };

            return Json(response);
        }

        [HttpPost("transaction/distribute")]
        public IActionResult distribute([FromBody]JObject value)
        {
            // initialize response
            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();

            // validate the transaction
            var validation = Program.blockchain.validateTransaction(value["transaction"].ToObject<JObject>());

            // get the list of nodes to exclude in the distribution
            List<string> skipnodes = value["nodes"].ToObject<List<string>>();

            if (!validation["validation"].ToObject<bool>())
            {
                response.Add("validation", "failed");
                response.Add("response", validation["response"]);
                if(validation["response"].ToString() == "No information about sender available, cannot validate")
                {
                    Program.blockchain.distributeTransaction(value["transaction"].ToObject<JObject>(), skipnodes);
                }
                return Ok(response);
            }

            // spread the transaction to neighboring nodes
            Program.blockchain.distributeTransaction(value["transaction"].ToObject<JObject>(), skipnodes);

            // fill in the response with the transaction
            response.Add("validation", "success");
            response.Add("message", validation["response"]);
            response.Add("transaction", value["transaction"]);
            response.Add("distributed", value["nodes"]);

            return Ok(response);
        }

        [HttpPost("transaction/new")]
        public IActionResult newTransaction([FromBody]JObject value)
        {
            // add time of the transaction to the JObject
            value["time"] = DateTime.Now;

            // validate and add transaction from request
            var validation = Program.blockchain.validateTransaction(value);
            if (!validation["validation"].ToObject<bool>())
            {
                return BadRequest(validation["response"]);
            }

            // distribute the transaction over the network
            Program.blockchain.distributeTransaction(value, new List<string>() { Program.node_identifier.ip });

            // return a 200 http request with the reponse
            return Ok(validation["response"]);
            
        }

        [HttpGet("chain")]
        public JsonResult chain()
        {
            // return the blockchain and the amount of blocks
            var response = new Dictionary<string, dynamic>();
            response.Add("chain", Program.blockchain.getChain());
            response.Add("length", Program.blockchain.getChain().Count);
            return Json(response);
        }

        [HttpGet("products")]
        public JsonResult products()
        {
            //return the products on the blockchain and the amount of them
            var response = new Dictionary<string, dynamic>();
            response.Add("products", Program.blockchain.getProducts());
            response.Add("amount", Program.blockchain.getProducts().Count);
            return Json(response);
        }

        [HttpGet("trace/{id}")]
        public IActionResult trace(string id)
        {
            foreach(var product in Program.blockchain.getProducts())
            {
                if(product.id == id)
                {
                    return Ok(Program.blockchain.getProductTransactions(id));
                }
            }
            return NotFound("The product Id was not found on the blockchain");
        }

        [HttpGet("address")]
        public JsonResult address()
        {
            //return the hash of the node
            var response = new Dictionary<string, dynamic>();
            response.Add("node_hash", Program.node_identifier.id);
            return Json(response);
        }

        [HttpPost("nodes/register")]
        public IActionResult registerNodes([FromBody]JObject value)
        {
            var nodes = value["nodes"];

            if(nodes == null)
            {
                return BadRequest("Error: please supply a valid list of nodes");
            }

            foreach(var node in nodes)
            {
                Program.blockchain.registerNode(node.ToObject<NeighborNode>());
            }

            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>()
            {
                { "message", "new nodes have been added" },
                { "total_nodes" , Program.blockchain.getNodes() },
            };

            return Ok(response);
            
        }

        [HttpGet("nodes/resolve")]
        public JsonResult consensus()
        {
            var replaced = Program.blockchain.resolveConflicts();
            Dictionary<string, dynamic> response;

            if (replaced)
            {
                response = new Dictionary<string, dynamic>()
                {
                    { "message", "Your chain was replaced by a newer chain" },
                    { "new_chain", Program.blockchain.getChain() }
                };
            }
            else
            {
                response = new Dictionary<string, dynamic>()
                {
                    { "message", "Your chain is authoritative" },
                    { "chain", Program.blockchain.getChain() }
                };
            }

            return Json(response);
        }
    }
}
