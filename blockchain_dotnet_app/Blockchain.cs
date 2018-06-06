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

            // return the transaction where the product was in
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

            // distribute the transaction over the network
            this.distributeTransaction(transaction);

            return this.chain.Last().id + 1;
        }

        //TODO: Make async, transaction part not working yet
        public void distributeTransaction(Transaction data)
        {
            List<string> neighbors = this.nodes;

            byte[] dataBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            Console.WriteLine("inside function, bytes are encoded");
            foreach (string node in neighbors)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + node + "/Blockchain/transaction/test");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ContentLength = dataBytes.Length;
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";
                Console.WriteLine("request made with post");
                using (Stream requestBody = request.GetRequestStream())
                {
                    requestBody.Write(dataBytes, 0, dataBytes.Length);
                    Console.WriteLine("writing request body");
                }
                Console.WriteLine("made it");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    if ((int)response.StatusCode == 200)
                    {
                        Console.WriteLine("Transaction distributed to node: " + node);
                        Console.WriteLine("Reponse: " + reader.ReadToEnd());
                        Console.WriteLine("--------------");
                    }
                    else
                    {
                        Console.WriteLine("error connecting to node");
                    }
                    
                }

            }

        }


        /*  HASHING AND MINING*/

        public string hash(Block block)
        {
            SHA256 sha256 = SHA256Managed.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(block));

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
