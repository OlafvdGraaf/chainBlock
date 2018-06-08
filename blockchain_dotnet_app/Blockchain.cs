using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace blockchain_dotnet_app
{
    class Blockchain
    {
        private List<Block> chain;
        private List<string> nodes;
        private List<Product> products;
        public List<Transaction> current_transactions;
        
        /* CONSTRUCTOR */

        public Blockchain()
        {
            this.chain = new List<Block>();
            this.current_transactions = new List<Transaction>();
            this.nodes = new List<string>();
            this.products = new List<Product>();

            // create genisis block
            this.newBlock(100, "none");
        }

        /* GETTERS */

        public List<Block> getChain()
        {
            return this.chain;
        }

        public List<string> getNodes()
        {
            return this.nodes;
        }

        public Block getLastBlock()
        {
            return this.chain.Last();
        }

        public List<Product> getProducts()
        {
            return this.products;
        }

        public Product GetProduct(int id)
        {
            foreach(Product product in this.products)
            {
                if(product.id == id)
                {
                    return product;
                }
            }
            return null;
        }

        public List<int> getProductIds()
        {
            // init a list of id's
            List<int> product_ids = new List<int>();

            foreach(var product in this.products)
            {
                product_ids.Add(product.id);
            }

            return product_ids;
        }

        public List<Transaction> getProductTransactions(int id)
        {
            // init a new list of transactions
            List<Transaction> product_transactions = new List<Transaction>();

            foreach(var block in this.chain)
            {
                foreach(var transaction in block.transactions)
                {
                    foreach(var product in transaction.products)
                    {
                        if(product.id == id)
                        {
                            product_transactions.Add(transaction);
                        }
                    }
                }
            }

            // return the transactions where the product was in
            return product_transactions;
        }

        /* CREATION OF ENTITIES */

        public void registerNode(string address)
        {
            if (!this.nodes.Contains(address))
            {
                this.nodes.Add(address);
            }
            
        }

        public Block newBlock(int proof, string hash)
        {
            // create new Block
            Block block = new Block(this.current_transactions, hash, proof);

            // add Block to the chain
            this.chain.Add(block);

            // reset Current transactions
            this.current_transactions = new List<Transaction>();

            return block;
        }

        public int newTransaction(Transaction transaction)
        {
            // add transaction to transactionlist
            this.current_transactions.Add(transaction);

            // add every product to the list of products
            foreach (Product product in transaction.products)
            {
                this.products.Add(product);
            }

            return this.chain.Last().id + 1;
        }

        public async Task distributeTransaction(JObject transaction, List<string> skipnodes)
        {
            List<string> neighbors = this.nodes.Except(skipnodes).ToList();

            // add the neighbors to the nodes to skip
            skipnodes.AddRange(neighbors);

            // create the data to be sent to neighbors
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>()
            {
                {"transaction", transaction},
                {"nodes", skipnodes}
            };

            byte[] dataBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));

            foreach (string node in neighbors)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + node + "/Blockchain/transaction/distribute");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ContentLength = dataBytes.Length;
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                using (Stream requestBody = request.GetRequestStream())
                {
                    await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    if ((int)response.StatusCode == 200)
                    {
                        Console.WriteLine("Transaction distributed to node: " + node);
                        Console.WriteLine("Response: " + await reader.ReadToEndAsync());
                        Console.WriteLine("--------------");
                    }
                    else
                    {
                        Console.WriteLine("error connecting to node");
                    }
                    
                }

            }

        }

        /* VALIDATION */

        public JObject validateTransaction(JObject request)
        {
            // set the rules of the expected request
            string[] required = new string[] { "products", "sender", "recipient", "chainTokens", "transactionFee" };

            // initialize response
            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();

            // check if the request has the right json keys
            foreach (string rule in required)
            {
                if (!request.ContainsKey(rule))
                {
                    response.Add("validation", false);
                    response.Add("response", "Missing values");
                    return JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(response));
                }
            }

            // init a list for new products
            List<Product> products = new List<Product>();

            //check if the product is new or an excisting
            foreach (var product in request["products"])
            {
                // check if a new product is made
                if (product["id"].ToString() == "new")
                {
                    // TODO: check if the the node that made a new product is a diamond miner

                    // create new product
                    products.Add(new Product(product["weight"].ToObject<int>()));
                }
                // check if the product excists on the blockchain
                else if (this.getProductIds().Contains(product["id"].ToObject<int>()))
                {
                    // TODO: check if the sender if the product with id thats being send, was the reciever of the product in its latest transaction

                    // add the product to the list
                    products.Add(this.GetProduct(product["id"].ToObject<int>()));
                }
                else
                {
                    response.Add("validation", false);
                    response.Add("response", "A product in your transaction is not present on the blockchain");
                    return JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(response));
                }

            }

            // make a new transaction add it to the blockchain transaction list
            int index = this.newTransaction(new Transaction(products, request["sender"].ToString(), request["recipient"].ToString(), request["chainTokens"].ToObject<double>(), request["transactionFee"].ToObject<double>()));

            // return the validated request
            response.Add("validation", true);
            response.Add("response", "Your transaction will be added to block " + index);
            return JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(response));

        }

        /* HASHING AND MINING */

        public string hash(Object blockchainObject)
        {
            SHA256 sha256 = SHA256Managed.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(blockchainObject));

            byte[] hash = sha256.ComputeHash(bytes);

            string result = BitConverter.ToString(hash,0).Replace("-","");

            return result;
        }

        public static bool validProof(int last_proof, int proof, string prev_hash)
        {
            // create new sha256 object
            SHA256 sha256 = SHA256Managed.Create();

            // create a string from the last proof against plus new new inserted proof
            string guess = last_proof.ToString() +  proof.ToString() + prev_hash;

            // encode the UTF-8 string to a bytes array
            byte[] bytes = Encoding.UTF8.GetBytes(guess);

            // get a hash from the encoded bytes
            byte[] hash = sha256.ComputeHash(bytes);

            // convert the hash to a hexadecimal string
            string result = BitConverter.ToString(hash, 0).Replace("-","");

            // check if the first 4 characters of the hexadecimal hash are 0000
            return result.Substring(0, 4) == "0000";
        }

        public int proofOfWork(int last_proof)
        {
            int proof = 0;
            
            while(!validProof(last_proof, proof, this.getLastBlock().hash))
            {
                proof++;
            }

            return proof;
        }

        public bool validChain(List<Block> chain)
        {
            Block last_block = this.chain[0];
            int current_index = 1;

            while(current_index < chain.Count)
            {
                Block block = chain[current_index];
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(last_block));
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(block));
                Console.WriteLine("----------");

                //check the hash of the block for correctness
                if(block.hash != this.hash(last_block))
                {
                    Console.WriteLine("hash does not match");
                    return false;
                }

                //check the proof of work correctness
                if(!validProof(last_block.proof, block.proof, last_block.hash))
                {
                    Console.WriteLine("proof of work does not match");
                    return false;
                }

                last_block = block;
                current_index++;
            }

            return true;
        }

        public bool resolveConflicts()
        {
            List<string> neighbors = this.nodes;
            List<Block> new_chain = null;

            var max_length = this.chain.Count;
            foreach(string node in neighbors)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + node + "/Blockchain/chain");
                request.Accept = "application/json; charset=utf-8";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    if ((int)response.StatusCode == 200)
                    {
                        var json_text = reader.ReadToEnd();
                        JObject json = JObject.Parse(json_text);
                        var json_length = json["length"];
                        var json_chain = json["chain"];

                        List<Block> chain = json_chain.ToObject<List<Block>>();
                        int length = json_length.ToObject<int>();

                        //check if the chain we got from another node is longer ours and valid
                        if (length > max_length && validChain(chain))
                        {
                            max_length = length;
                            new_chain = chain;
                        }

                    }
                }
            }

            if(new_chain != null)
            {
                this.chain = new_chain;
                return true;
            }

            return false;
        }
    }
}
