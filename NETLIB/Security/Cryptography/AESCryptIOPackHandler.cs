using System.IO;
using System.Security.Cryptography;

namespace NETLIB.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class AESCryptIOPackHandler : CryptIOPackHandler
    {
        #region Variables

        AesCryptoServiceProvider aesProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        public AESCryptIOPackHandler(Publisher publisher) : base(publisher) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="initialProtocol"></param>
        public AESCryptIOPackHandler(Publisher publisher, Protocol<CryptPack> initialProtocol) : base(publisher, initialProtocol) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="initialProtocol"></param>
        /// <param name="iv"></param>
        /// <param name="key"></param>
        public AESCryptIOPackHandler(Publisher publisher, Protocol<CryptPack> initialProtocol, byte[] iv, byte[] key)
            : base(publisher, initialProtocol)
        {
            aesProvider = new AesCryptoServiceProvider
            {
                IV = iv,
                Key = key,
                Padding = PaddingMode.None
            };
        }

        #endregion

        #region Attributes



        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iv"></param>
        /// <param name="key"></param>
        public void InitializeAES(byte[] iv, byte[] key)
        {
            aesProvider = new AesCryptoServiceProvider
            {
                IV = iv,
                Key = key,
                Padding = PaddingMode.None
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected override void Decrypt(byte[] pack, int offset, int count)
        {
            var decryptor = aesProvider.CreateDecryptor();

            using (MemoryStream ms = new MemoryStream(pack, offset, count))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    cs.Read(pack, offset, count);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override byte[] Encrypt(byte[] pack, int offset, int count)
        {
            var encryptor = aesProvider.CreateEncryptor();

            byte[] buffer;
            using (MemoryStream ms = new MemoryStream(pack.Length))
            {
                ms.Write(pack, 0, offset);
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(pack, offset, count);
                }
                buffer = ms.ToArray();
            }
            return buffer;
        }

        #endregion
    }
}
