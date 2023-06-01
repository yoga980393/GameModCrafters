
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;
namespace GameModCrafters.Encryption
{
    public class HashService:IHashService
    {
      
        public string SHA512Hash(string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
            {
                return "";
            }

            StringBuilder sb;

            using (SHA512 sha512 = SHA512.Create())
            {
                //將字串轉為Byte[]
                byte[] byteArray = Encoding.UTF8.GetBytes(rawString);

                byte[] encryption = sha512.ComputeHash(byteArray);


                sb = new StringBuilder();

                //for (int i = 0; i < encryption.Length; i++)
                //{
                //    sb.Append(encryption[i].ToString("x2"));
                //}

                foreach (byte bt in encryption)
                {
                    sb.Append(bt.ToString("x2"));
                }
            }

            return sb.ToString(); ;
        }
    }
}
