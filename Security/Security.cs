using System;
using System.IO;
using System.Text;

namespace Security
{
    public partial class User
    {
        public void SetPassword(string password)
        {
            var key = CryptoHelper.GenerateKey(password);
            var stream = new MemoryStream();

            var length = CryptoHelper.EncryptToStream(
                stream, key, cs =>
                {
                    var writer = new StreamWriter(cs);
                    writer.WriteLine(password);
                    writer.Flush();
                });

            var encryptedPassword = stream.GetBuffer();
            Array.Resize(ref encryptedPassword, length);

            Password = encryptedPassword.ToString();
        }

        public bool Authenticate(string password)
        {



            //return true;   // TODO
            try
            {
              byte[] hash = CryptoHelper.GenerateKey(password);
              string encryptedPassword = Encoding.UTF8.GetString(hash, 0, hash.Length); 
              
              //var encryptedPassword = Encoding.UTF8.GetString(CryptoHelper.GenerateKey(password), 0, Password.Length);
              return Password == encryptedPassword;
            }
            catch (Exception e)
            {
              var x = e.Message;
                return false;
            }
        }
    }
}
