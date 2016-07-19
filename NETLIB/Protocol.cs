using System;

namespace NETLIB
{
    /// <summary>
    /// Responsible for managing a communication protocol, in other words, analyze an incoming
    /// packet, check for a method of treatment registered for that type of package,
    /// if any, the method is called to handle the package, if not a generic event
    /// is called to handle the incoming pack. Idealised to facilitate handling packages and management
    /// protocols, especially in cases where the client continuously migrates between different protocols.
    /// </summary>
    /// <typeparam name="TPack">Pack class derived from <see cref="BasePack"/>
    /// that the Protocol will manage.
    /// </typeparam>
    public class Protocol<TPack> where TPack : BasePack
    {
        #region Variables

        /// <summary>
        /// Name used to identify this protocol.
        /// </summary>
        /// <seealso cref="IOPackHandler{TPack}.AddProtocol(Protocol{TPack})"/>
        /// <see cref="IOPackHandler{TPack}.ExchangeProtocol(string)"/>
        string name;

        /// <summary>
        /// Dictionary of methods used to bind a method of treating to particular ID.
        /// <para>
        /// Must have 256 or fewer positions because the package ID is represented by a byte.
        /// </para>
        /// </summary>
        /// <seealso cref="IOPackHandler{TPack}"/>
        private ThrowPackEventHandler<TPack>[] triggers;

        /// <summary>
        /// Generic method used to treat packets without associated methods.
        /// </summary>
        /// <seealso cref="IOPackHandler{TPack}"/>
        public event ThrowPackEventHandler<TPack> ReceivedPack;

        #endregion

        #region Constructor

        /// <summary>
        /// initializes the Protocol with a empty dictionary of codes.
        /// </summary>
        /// <param name="name"> Name used to identify this protocol.</param>
        public Protocol(string name)
        {
            this.name = name;
            triggers = new ThrowPackEventHandler<TPack>[256];
        }

        /// <summary>
        /// initializes the Protocol with a existing dictionary of methods.
        /// </summary>
        /// <param name="name"> Name used to identify this protocol.</param>
        /// <param name="triggers">
        /// Dictionary methods that will be imported.
        /// Must have 256 or fewer positions because the package ID is represented by a byte.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="triggers"/>.Length is greater than 256.
        /// </exception>
        public Protocol(string name, ThrowPackEventHandler<TPack>[] triggers)
        {
            this.name = name;

            if (triggers.Length > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException("Vector higher than the allowed of 256 positions.");
            }
            this.triggers = triggers;
        }


        #endregion

        #region Attributes

        /// <summary>
        /// Name used to identify this protocol.
        /// </summary>
        /// <seealso cref="IOPackHandler{TPack}.AddProtocol(Protocol{TPack})"/>
        /// <see cref="IOPackHandler{TPack}.ExchangeProtocol(string)"/>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Dictionary of methods used to bind a method of treating to particular ID.
        /// <para>
        /// Must have 256 or fewer positions because the package ID is represented by a byte.
        /// </para>
        /// </summary>
        public ThrowPackEventHandler<TPack>[] Triggers
        {
            get { return triggers; }
        }

        /// <summary>
        /// Gets the method reletad to a specific ID.
        /// </summary>
        /// <param name="index">ID of the pack.</param>
        /// <returns>Method reletad to <paramref name="index"/></returns>
        /// <exception cref="IndexOutOfRangeException">When the imput index do not exists.</exception>
        public ThrowPackEventHandler<TPack> this[byte index]
        {
            get { return triggers[index]; }

            set { triggers[index] = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set a new dictionary of methods.
        /// </summary>
        /// <param name="eventDict">New dictionary of methods.</param>
        /// <seealso cref="triggers"/>
        public void SetTriggers(ThrowPackEventHandler<TPack>[] eventDict)
        {
            if (eventDict.Length <= byte.MaxValue)
            {
                this.triggers = eventDict;
            }
            else
            {
                throw new ArgumentException("Vector higher than the allowed of 256 positions.");
            }
        }

        /// <summary>
        /// Add a method that will be called when the pack's ID is equal 'Key'.
        /// </summary>
        /// <param name="key">Pack's ID.</param>
        /// <param name="value">Method of treatment of the package.</param>
        public void AddTrigger(byte key, ThrowPackEventHandler<TPack> value)
        {
            triggers[key] += value;
        }

        /// <summary>
        /// Remove all triggers for a particular ID.
        /// </summary>
        /// <param name="key">Trigger ID to be removed.</param>
        public void RemoveTrigge(byte key)
        {
            triggers[key] = null;
        }

        /// <summary>
        /// Clear all the triggers.
        /// </summary>
        /// <seealso cref="triggers"/>
        public void ClearTriggers()
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i] = null;
            }
        }

        /// <summary>
        ///  Function used to call the method referring to a specific ID
        /// </summary>
        /// <param name="pack">Package to be treated.</param>
        /// <param name="consumer">Consumer that is consuming the pack.</param>
        public void OnReceivedPackCall(Consumer<TPack> consumer, TPack pack)
        {
            if (triggers[pack.ID] != null)
            {
                triggers[pack.ID](consumer, pack);
            }
            else
            {
                if (ReceivedPack != null)
                {
                    ReceivedPack(consumer, pack);
                }
            }
        }


        #endregion
    }
}
