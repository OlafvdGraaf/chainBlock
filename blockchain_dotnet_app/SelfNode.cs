using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace blockchain_dotnet_app
{
    public class SelfNode : Node
    {
        private RSAParameters publicKey;
        private RSAParameters privateKey;
        private static Random random = new Random();

        public SelfNode(string ip, bool is_supplychain, bool is_supplychain_miner)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                publicKey = rsa.ExportParameters(false);
                privateKey = rsa.ExportParameters(true);
            }

            // set ip
            this.ip = ip;

            // set the type of node
            if (is_supplychain)
            {
                if (is_supplychain_miner)
                {
                    this.type = "supplychain_miner";
                }
                else
                {
                    this.type = "supplychain";
                }
            }
            else
            {
                this.type = "normal";
            }

            // set random string on id to make the hash random
            this.id = RandomString(20);

            // generate and set the hash as an identifier
            this.setHash();
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void setHash()
        {
            this.id = Program.blockchain.hash(this);
        }
    }
}
