using System;
using System.Collections.Generic;
using System.Net;

namespace NETLIB.UDP
{
    /// <summary>
    /// 
    /// </summary>
    public class IOUDPPackHandler : IOPackHandler<UDPPack>
    {
        #region Variables

        Queue<IPEndPoint> hosts;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="initialProtocol"></param>
        public IOUDPPackHandler(UDPPublisher publisher, Protocol<UDPPack> initialProtocol)
            : base(publisher, initialProtocol)
        {
            hosts = publisher.Hosts;
        }

        #endregion

        #region Attributes



        #endregion

        #region Methods


        /// <summary>
        /// 
        /// </summary>
        protected override void Consume()
        {
            while (publisher.IsEnabled && isEnabled)
            {
                while (packQueue.Count > 0)
                {
                    OnReceivedPackCall(new UDPPack(packQueue.Dequeue(), hosts.Dequeue()));
                }

                incomingPackEvent.WaitOne();
            }

            while (packQueue.Count > 0)
            {
                OnReceivedPackCall(new UDPPack(packQueue.Dequeue(), hosts.Dequeue()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override UDPPack PackFactory()
        {
            return new UDPPack();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public override UDPPack PackFactory(byte[] pack)
        {
            return new UDPPack(pack);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public override UDPPack PackFactory(BasePack pack)
        {
            return new UDPPack(pack);
        }

        #endregion
    }
}
