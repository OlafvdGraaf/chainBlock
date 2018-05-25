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
        private List<Transaction> current_transactions;
        private Block last_block;


        public Blockchain()
        {
            this.chain = new List<Block>();
            this.current_transactions = new List<Transaction>();
            this.last_block = chain.Last();
        }

        public Block newBlock(int proof, string previous_hash)
        {
            //Create new Block
            Block block = new Block(this.current_transactions, previous_hash, proof);

            //Reset Current transactions
            this.current_transactions.Clear();

            //Add Block to the chain
            this.chain.Append(block);

            return block;
        }

        public int newTransaction(string sender, string recipient, List<Product> products, double amount, double fee)
        {
            this.current_transactions.Append(new Transaction(products, sender, recipient, amount, fee));

            return this.last_block.getId() + 1;
        }

        public Block getLastBlock()
        {
            return this.last_block;
        }

        public static string hash(Block block)
        {
            SHA256 sha256 = SHA256Managed.Create();
            SHA256 test = SHA256Managed.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(block));

            byte[] hash = sha256.ComputeHash(bytes);

            string result = BitConverter.ToString(hash, 0);

            return result;
        }
    }
}
