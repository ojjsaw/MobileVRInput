namespace MVRInput
{
    using UnityEngine.Networking;
    using UnityEngine;

    public class MVRInputConnection
    {
        #region Vars
        private ConnectionConfig config = null;
        private ConnectionType connection = ConnectionType.GAMEPAD;
        private int channelId = -1;
        private HostTopology topology = null;
        private int sockedId = -1;
        private int socketPort = 8888;
        private MVRInputStatus status = MVRInputStatus.DISCONNECTED;

        public string ServerIP
        {
            get {
                if (connection == ConnectionType.APP) return Network.player.ipAddress;
                else return "None";
            }
        }

        private int otherConnectionId = -1;
        private int otherChannelId = -1;
        private int otherHostId = -1;
        private bool isConnected = false;
        #endregion

        public MVRInputConnection(ConnectionType connection)
        {
            this.connection = connection;

            config = new ConnectionConfig();
            channelId = config.AddChannel(QosType.Unreliable);
            topology = new HostTopology(config, 1);

            NetworkTransport.Init();

            sockedId = (connection == ConnectionType.APP) ? NetworkTransport.AddHost(topology, socketPort) : NetworkTransport.AddHost(topology);

            isConnected = false;

        }

        public MVRInputStatus CheckConnectionStatus()
        {
            //TODO: Allocate Buffer
            byte[] _recbuffer = null;
            int _recdataSize;
            byte _recerror;
            NetworkEventType networkEvent = NetworkTransport.Receive(out otherHostId, out otherConnectionId, out otherChannelId, _recbuffer, _recbuffer.Length, out _recdataSize, out _recerror);
            switch (networkEvent)
            {
                case NetworkEventType.Nothing:
                    if (isConnected) status = MVRInputStatus.CONNECTED;
                    else status = MVRInputStatus.DISCONNECTED;
                    break;
                case NetworkEventType.ConnectEvent:
                    isConnected = true;
                    status = MVRInputStatus.CONNECTED;
                    break;
                case NetworkEventType.DataEvent:
                    //TODO: Deserealize Code
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    status = MVRInputStatus.DISCONNECTED;
                    break;
                default:
                    if (isConnected) status = MVRInputStatus.CONNECTED;
                    else status = MVRInputStatus.DISCONNECTED;
                    break;
            }
            return status;
        }

        public MVRInputStatus SendToOther(/* TODO: Accept Generic Data*/)
        {
            //TODO: Allocate Buffer
            byte[] _sendbuffer = null;
            byte _senderror;
            if (NetworkTransport.Send(sockedId, otherConnectionId, otherChannelId, _sendbuffer, _sendbuffer.Length, out _senderror))
            {
                return MVRInputStatus.DATASENT;
            }

            return MVRInputStatus.FAILEDTOSEND;
        }


    }
}


