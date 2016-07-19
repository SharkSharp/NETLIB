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
    /// <example>
    /// The following example shows how to implement a chat client using <see cref="IOBasePackHandler"/> to connect
    /// to the server and manage packages.
    /// <code>
    ///using NETLIB;
    ///using NETLIB.TCP;
    ///using System;
    ///
    ///namespace ChatExempleClient
    ///{
    ///    class Program
    ///    {
    ///        static IOBasePackHandler client;
    ///        static Protocol&lt;BasePack&gt; chatProtocol;
    ///        static string name;
    ///
    ///        static void Main(string[] args)
    ///        {
    ///            chatProtocol = new Protocol&lt;BasePack&gt;("chatProtocol");
    ///            chatProtocol[0] += MessagePackHandle;
    ///
    ///            client = new IOBasePackHandler(new TCPPublisher("127.0.0.1", 1975), chatProtocol);
    ///            client.Start();
    ///
    ///            Console.WriteLine("Your name please:");
    ///            name = Console.ReadLine();
    ///
    ///            string aux = Console.ReadLine();
    ///            while (aux != "exit")
    ///            {
    ///                var pack = new BasePack();
    ///                pack.ID = 0;
    ///                pack.PutString(name + ": " + aux);
    ///                client.SendPack(pack);
    ///                aux = Console.ReadLine();
    ///            }
    ///
    ///            client.CloseConnection();          
    ///        }
    ///
    ///        private static void MessagePackHandle(Consumer&lt;BasePack&gt; consumer, BasePack receivedPack)
    ///        {
    ///            Console.WriteLine(receivedPack.GetString());
    ///        }
    ///    }
    ///}
    /// </code>
    /// </example>
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
