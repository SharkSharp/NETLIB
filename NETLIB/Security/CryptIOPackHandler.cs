using System.Net;
using System.Security.Cryptography;

namespace NETLIB.Security
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CryptIOPackHandler : IOPackHandler<CryptPack>
    {
        #region Variables



        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler with a publisher who will publish the
        /// packages and a protocol that will be used initially by this connection.
        /// </summary>
        /// <param name="publisher">Publisher who will publish the packages to be managed.</param>
        /// <param name="initialProtocol">Initial <see cref="Protocol{TPack}"/> to be used by this connection.</param>
        /// <see cref="IOPackHandler{TPack}"/>
        /// <see cref="IOPackHandler{TPack}(Publisher, Protocol{TPack})"/>
        public CryptIOPackHandler(Publisher publisher, Protocol<CryptPack> initialProtocol) : base(publisher, initialProtocol) { }

        #endregion

        #region Attributes



        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        protected abstract byte[] Encrypt(byte[] pack);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        protected abstract void Decrypt(byte[] pack);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="ip"></param>
        public override void SendPack(byte[] pack, IPEndPoint ip = null)
        {
            base.SendPack(Encrypt(pack), ip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="ip"></param>
        public override void SendPack(CryptPack pack, IPEndPoint ip = null)
        {
            base.SendPack(Encrypt(pack.Buffer), ip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        protected override void OnReceivedPackCall(CryptPack pack)
        {
            Decrypt(pack.Buffer);
            base.OnReceivedPackCall(pack);
        }



        /// <summary>
        /// Create a new instance of CryptPack that initializes the inner buffer with
        /// <see cref="BasePack.packSize"/>.
        /// </summary>
        /// <seealso cref="CryptPack()"/>
        /// <seealso cref="BasePack.packSize"/>
        public override CryptPack PackFactory()
        {
            return new CryptPack();
        }

        /// <summary>
        /// Create a new instance of CryptPack that initializes the BasePack taking
        /// <paramref name="buffer"/> as your own inner buffer.
        /// </summary>
        /// <param name="buffer">Source buffer.</param>
        /// <seealso cref="BasePack(byte[], bool)"/>
        public override CryptPack PackFactory(byte[] buffer)
        {
            return new CryptPack(buffer);
        }

        /// <summary>
        /// Create a new instance of CryptPack that takes the <paramref name="basePack"/>
        /// inner buffer as its own inner beffer.
        /// The <see cref="BasePack.readPosition"/> and the <see cref="BasePack.writePosition"/>
        /// are not copied.
        /// </summary>
        /// <param name="basePack">BasePack that will be copied.</param>
        /// <seealso cref="BasePack(BasePack, bool)"/>
        public override CryptPack PackFactory(BasePack basePack)
        {
            return new CryptPack(basePack);
        }

        #endregion
    }
}
