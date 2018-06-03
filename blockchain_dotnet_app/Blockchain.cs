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
        public List<Transaction> current_transactions;
        

        public Blockchain()
        {
            this.chain = new List<Block>();
            this.current_transactions = new List<Transaction>();
            this.nodes = new List<string>();

            //create genisis block
            this.newBlock(100, "none");
        }

        public List<Block> getChain()
        {
            return this.chain;
        }

        public List<string> getNodes()
        {
            return this.nodes;
        }

        public void registerNode(string address)
        {
            if (!this.nodes.Contains(address))
            {
                this.nodes.Add(address);
            }
            
        }

        public Block newBlock(int proof, string hash)
        {
            //Create new Block
            Block block = new Block(this.current_transactions, hash, proof);

            //Add Block to the chain
            this.chain.Add(block);

            //Reset Current transactions
            this.current_transactions = new List<Transaction>();

            return block;
        }

        public int newTransaction(Transaction transaction)
        {
            this.current_transactions.Add(transaction);

            return this.chain.Last().id + 1;
        }

        public Block getLastBlock()
        {
            return this.chain.Last();
        }

        public string hash(Block block)
        {
            SHA256 sha256 = SHA256Managed.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(block));

            byte[] hash = sha256.ComputeHash(bytes);

            string result = BitConverter.ToString(hash,0).Replace("-","");

            return result;
        }

        public static bool validProof(int last_proof, int proof)
        {
            //create new sha256 object
            SHA256 sha256 = SHA256Managed.Create();

            //create a string from the last proof against plus new new inserted proof
            string guess = last_proof.ToString() +  proof.ToString();

            // encode the UTF-8 string to a bytes array
            byte[] bytes = Encoding.UTF8.GetBytes(guess);

            // get a hash from the encoded bytes
            byte[] hash = sha256.ComputeHash(bytes);

            // convert the hash to a hexadecimal string
            string result = BitConverter.ToString(hash, 0).Replace("-","");

            //check if the first 4 characters of the hexadecimal hash are 0000
            return result.Substring(0, 4) == "0000";
        }

        public int proofOfWork(int last_proof)
        {
            int proof = 0;
            
            while(!validProof(last_proof, proof))
            {
                proof++;
            }

            return proof;
        }

        public bool validChain(List<Block> chain)
        {
            Block last_block = this.getLastBlock();
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
                if(!validProof(last_block.proof, block.proof))
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
            Console.WriteLine("yes im in");
            foreach(string node in neighbors)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + node + "/Blockchain/chain");
                Console.WriteLine("request made");
                request.Accept = "application/json; charset=utf-8";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Console.WriteLine("compressed data");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    Console.WriteLine("insade streamreader");
                    if ((int)response.StatusCode == 200)
                    {
                        Console.WriteLine("statsu code 200");
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
