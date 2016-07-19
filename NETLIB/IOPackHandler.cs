using System.Collections.Generic;

namespace NETLIB
{
    /// <summary>
    /// Better manage the incoming and outgoing <typeparamref name="TPack"/> using a <see cref="Protocol{TPack}"/>
    /// to redistribute the packs. It has an internal dictionary of <see cref="Protocol{TPack}"/>
    /// that can be exchanged for the currently used.
    /// </summary>
    /// <typeparam name="TPack">Pack class derived from <see cref="BasePack"/> that the IOPackHandler will manage.</typeparam>
    public abstract class IOPackHandler<TPack> : Consumer<TPack> where TPack : BasePack
    {
        #region Variables

        /// <summary>
        /// Dictionary protocols that will store all protocols used by <see cref="IOPackHandler{TPack}"/>.
        /// </summary>
        /// <seealso cref="AddProtocol(Protocol{TPack})"/>
        /// <seealso cref="ExchangeProtocol(string)"/>
        Dictionary<string, Protocol<TPack>> protocols;

        /// <summary>
        /// Protocol currently used.
        /// </summary>
        /// <seealso cref="ExchangeProtocol(string)"/>
        Protocol<TPack> currentProtocol;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler with a publisher who will publish the
        /// packages and a protocol that will be used initially by this connection.
        /// </summary>
        /// <param name="publisher">Publisher who will publish the packages to be managed.</param>
        /// <param name="initialProtocol">Initial <see cref="Protocol{TPack}"/> to be used by this connection.</param>
        public IOPackHandler(Publisher publisher, Protocol<TPack> initialProtocol) 
            : base(publisher)
        {
            this.protocols = new Dictionary<string, Protocol<TPack>>();
            this.protocols.Add(initialProtocol.Name, initialProtocol);
            this.currentProtocol = initialProtocol;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Name of the current protocol.
        /// </summary>
        public string CurrentProtocolName
        {
            get { return currentProtocol.Name; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a new protocol in the protocols dictionary.
        /// </summary>
        /// <param name="newProtocol">Protocol to be added to the dictionary.</param>
        /// <seealso cref="protocols"/>
        public void AddProtocol(Protocol<TPack> newProtocol)
        {
            protocols.Add(newProtocol.Name, newProtocol);
        }

        /// <summary>
        /// Change the current protocol for one stored in the dictionary.
        /// </summary>
        /// <param name="protocolName">Protocol name to be used.</param>
        /// <exception cref="KeyNotFoundException">
        /// Throws when the required protocol does not exist in the dictionary.
        /// </exception>
        public void ExchangeProtocol(string protocolName)
        {
            currentProtocol = protocols[protocolName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        protected override void OnReceivedPackCall(TPack pack)
        {
            currentProtocol.OnReceivedPackCall(this, pack);
            base.OnReceivedPackCall(pack);
        }

        #endregion
    }
}
