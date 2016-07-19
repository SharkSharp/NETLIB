using System;

namespace NETLIB
{
    /// <summary>
    /// Better manage the incoming and outgoing <see cref="BasePack"/> using a <see cref="Protocol{TPack}"/>
    /// to redistribute the packs. It has an internal dictionary of <see cref="Protocol{TPack}"/>
    /// that can be exchanged for the currently used.
    /// </summary>
    /// <seealso cref="IOPackHandler{TPack}"/>
    /// <seealso cref="Consumer{TPack}"/>
    /// <seealso cref="BasePack"/>
    /// <seealso cref="Protocol{TPack}"/>
    public class IOBasePackHandler : IOPackHandler<BasePack>
    {
        #region Variables



        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler with a publisher who will publish the
        /// packages and a protocol that will be used initially by this connection.
        /// </summary>
        /// <param name="publisher">Publisher who will publish the packages to be managed.</param>
        /// <param name="inicialProtocol">Initial <see cref="Protocol{TPack}"/> to be used by this connection.</param>
        /// <seealso cref="IOPackHandler{TPack}"/>
        /// <seealso cref="Consumer{TPack}"/>
        /// <seealso cref="BasePack"/>
        /// <seealso cref="Protocol{TPack}"/>
        public IOBasePackHandler(Publisher publisher, Protocol<BasePack> inicialProtocol) : base(publisher, inicialProtocol) { }


        #endregion

        #region Attributes



        #endregion

        #region Methods

        /// <summary>
        /// Used by the <see cref="Consumer{TPack}.Consume"/> to obtain an instance of the <see cref="BasePack"/>
        /// through <see cref="BasePack(BasePack)"/>  constructor.
        /// </summary>
        /// <param name="pack"><see cref="BasePack"/> to be based on.</param>
        /// <returns>New <see cref="BasePack"/> based on <paramref name="pack"/></returns>
        public override BasePack PackFactory(BasePack pack)
        {
            return pack;
        }

        /// <summary>
        /// Used by the <see cref="Consumer{TPack}.Consume"/> to obtain an instance of the <see cref="BasePack"/>
        /// through <see cref="BasePack(byte[])"/>  constructor.
        /// </summary>
        /// <param name="pack"><see cref="BasePack"/> to be based on.</param>
        /// <returns>New <see cref="BasePack"/> based on <paramref name="pack"/></returns>
        public override BasePack PackFactory(byte[] pack)
        {
            return pack;
        }

        /// <summary>
        /// Used by the <see cref="Consumer{TPack}.Consume"/> to obtain an instance of the <see cref="BasePack"/>
        /// through <see cref="BasePack()"/>  constructor.
        /// </summary>
        /// <returns>New <see cref="BasePack"/>.</returns>
        public override BasePack PackFactory()
        {
            return new BasePack();
        }

        #endregion
    }
}
