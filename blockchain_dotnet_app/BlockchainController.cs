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
            Transaction block_reward = new Transaction(new List<Product>(), "chainBlock foundation", Program.node_identifier, 10, 0);
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

        [HttpPost("transaction/new")]
        public IActionResult newTransaction([FromBody]JObject value)
        {
            //set the rules of the expected json thorugh http request
            string[] required = new string[] { "products", "sender", "recipient", "chainTokens", "transactionFee" };

            //check if the post request has the right json keys
            foreach (string rule in required)
            {
                if (!value.ContainsKey(rule))
                {
                    return BadRequest("Missing values");
                }
            }

            // init a list for new products
            List<Product> products = new List<Product>();

            //check if the product is new or an excisting
            foreach (var product in value["products"])
            {
                // check if a new product is made
                if (product["id"].ToString() == "new")
                {
                    // TODO: check if the the node that made a new product is a diamond miner

                    // create new product
                    products.Add(new Product(product["weight"].ToObject<int>()));
                }
                // check if the product excists on the blockchain
                else if (Program.blockchain.getProductIds().Contains(product["id"].ToObject<int>()))
                {
                    // TODO: check if the sender if the product with id thats being send, was the reciever of the product in its latest transaction

                    // add the product to the list
                    products.Add(Program.blockchain.GetProduct(product["id"].ToObject<int>()));
                }
                else
                {
                    return BadRequest("A product in your transaction is not present on the blockchain");
                }

            }

            // make new transaction object
            Transaction new_transaction = new Transaction(products, value["sender"].ToString(), value["recipient"].ToString(), value["chainTokens"].ToObject<double>(), value["transactionFee"].ToObject<double>());

            // add the new transaction to the blockchain transaction list and return the block index of the next block
            var index = Program.blockchain.newTransaction(new_transaction);
            
            // make the response message
            string response = "Your transaction will be added to block " + index;

            // return a 200 http request with the reponse
            return Ok(response);
            
        }

        [HttpGet("chain")]
        public JsonResult chain()
        {
            //return the blockchain and the amount of blocks
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
        public IActionResult trace(int id)
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
                Program.blockchain.registerNode(node.ToString());
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
