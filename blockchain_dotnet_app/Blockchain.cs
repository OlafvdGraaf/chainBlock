using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;

namespace blockchain_dotnet_app
{
    class Blockchain
    {
        private List<Block> chain;
        public List<Transaction> current_transactions;


        public Blockchain()
        {
            this.chain = new List<Block>();
            this.current_transactions = new List<Transaction>();
            //create genisis block
            this.newBlock(100, "none");
        }

        public List<Block> getChain()
        {
            return this.chain;
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
    }
}
