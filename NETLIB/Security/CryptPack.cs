namespace NETLIB.Security
{
    /// <summary>
    /// Is the package that travels over the network with encryption.
    /// </summary>
    /// <seealso cref="BasePack"/>
    /// <seealso cref="CryptIOPackHandler"/>
    public class CryptPack : BasePack
    {
        #region Variables

        /// <summary>
        /// Index of <see cref="BasePack.buffer"/> which locates the byte that indicates whether the pack
        /// is encrypted.
        /// </summary>
        protected const int IS_ENCRYPTED_INDEX = 1;

        /// <summary>
        /// Index of <see cref="BasePack.buffer"/> which locates the ID copy used to determine if there was
        /// any error at the time of decryption, that is, if the package is corrupted.
        /// </summary>
        protected const int ENCRYPTED_ID_INDEX = 2;

        /// <summary>
        /// Number of bytes that will be used in the pack integrity checking ID. The larger number of bytes,
        /// the smaller the chance of error in the scan time.
        /// </summary>
        protected const int ENCRYPTED_ID_SIZE = sizeof(int);

        /// <summary>
        /// Indicates whether the pack is encrypted.
        /// </summary>
        bool isEncrypted = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the inner buffer with <see cref="BasePack.packSize"/>.
        /// </summary>
        /// <seealso cref="BasePack(bool)"/>
        /// <seealso cref="BasePack.packSize"/>
        public CryptPack()
            : base()
        {
            this.PutBool(this.isEncrypted, IS_ENCRYPTED_INDEX);
            this.readPosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
            this.writePosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
        }

        /// <summary>
        /// Initialize the inner buffer with <see cref="BasePack.packSize"/> and set if the pack will be encrypted.
        /// </summary>
        /// <param name="isEncrypted">Indicates whether the pack is encrypted.</param>
        /// <seealso cref="BasePack(bool)"/>
        /// <seealso cref="BasePack.packSize"/>
        public CryptPack(bool isEncrypted)
            : base()
        {
            this.isEncrypted = isEncrypted;
            this.PutBool(isEncrypted, IS_ENCRYPTED_INDEX);
            this.readPosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
            this.writePosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
        }

        /// <summary>
        /// Takes the <paramref name="basePack"/> inner buffer as its own inner beffer.
        /// The <see cref="BasePack.readPosition"/> and the <see cref="BasePack.writePosition"/> are not copied
        /// </summary>
        /// <param name="basePack">BasePack that will be copied.</param>
        /// <seealso cref="BasePack(BasePack, bool)"/>
        public CryptPack(BasePack basePack)
            : base(basePack)
        {
            this.isEncrypted = GetBool(IS_ENCRYPTED_INDEX);
            this.readPosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
            this.writePosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
        }

        /// <summary>
        /// Initialize the BasePack taking <paramref name="buffer"/> as your own inner buffer
        /// </summary>
        /// <param name="buffer">Source buffer.</param>
        /// <seealso cref="BasePack(byte[], bool)"/>
        public CryptPack(byte[] buffer)
            : base(buffer)
        {
            this.isEncrypted = GetBool(IS_ENCRYPTED_INDEX);
            this.readPosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
            this.writePosition = ENCRYPTED_ID_INDEX + ENCRYPTED_ID_SIZE;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Overload the set method so that it inserts the ID checker in the area that
        /// will be encrypted;
        /// </summary>
        public override byte ID
        {
            set
            {
                base.ID = value;
                //PutInt because ENCRYPTED_ID_SIZE has the size of an int
                this.PutInt(this.ID, ENCRYPTED_ID_INDEX);
            }
        }

        /// <summary>
        /// Returns if the pack was decrypted properly or is corrupted.
        /// </summary>
        public virtual bool IsCorrupted
        {
            get
            {
                //GetInt because ENCRYPTED_ID_SIZE has the size of an int
                return isEncrypted && (ID != this.GetInt(ENCRYPTED_ID_INDEX));
            }
        }

        /// <summary>
        /// Indicates whether the pack is encrypted.
        /// </summary>
        public virtual bool IsEncrypted
        {
            get { return isEncrypted; }

            set
            {
                isEncrypted = value;
                this.PutBool(value, IS_ENCRYPTED_INDEX);
            }
        }

        #endregion

        #region Methods



        #endregion
    }
}
