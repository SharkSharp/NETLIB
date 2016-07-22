using System.Net;

namespace NETLIB.UDP
{
    /// <summary>
    /// 
    /// </summary>
    public class UDPPack : BasePack
    {
        #region Variables

        IPEndPoint source;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ignoreID"></param>
        public UDPPack(IPEndPoint source = null, bool ignoreID = false)
            : base(ignoreID)
        {
            this.source = source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="source"></param>
        /// <param name="ignoreID"></param>
        public UDPPack(byte[] buffer, IPEndPoint source = null, bool ignoreID = false)
            : base(buffer, ignoreID)
        {
            this.source = source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePack"></param>
        /// <param name="source"></param>
        /// <param name="ignoreID"></param>
        public UDPPack(BasePack basePack, IPEndPoint source = null, bool ignoreID = false)
            : base(basePack, ignoreID)
        {
            this.source = source;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint Source
        {
            get { return source; }
        }

        #endregion

        #region Methods



        #endregion
    }
}
