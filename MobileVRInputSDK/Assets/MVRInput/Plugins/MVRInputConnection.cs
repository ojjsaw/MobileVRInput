namespace MVRInput
{
    using UnityEngine.Networking;
    using UnityEngine;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

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

        public bool IsConnected
        {
            get
            {
                return isConnected;
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

        public void Close()
        {
            byte error;
            NetworkTransport.Disconnect(sockedId, otherConnectionId, out error);
            NetworkTransport.RemoveHost(sockedId);
            otherConnectionId = otherChannelId = otherHostId = -1;
            isConnected = false;
        }

        public MVRInputStatus ConnectToServer(string serverAddress)
        {
            byte error;
            otherConnectionId = NetworkTransport.Connect(sockedId, serverAddress, socketPort, 0, out error);
            if (error != (byte)NetworkError.Ok)
            {
                NetworkError nerror = (NetworkError)error;
                Debug.Log("Error: " + nerror.ToString());
                isConnected = false;
                return MVRInputStatus.FAILEDTOCONNECT;
            }

            isConnected = true;

            return MVRInputStatus.CONNECTED;
        }

        public MVRInputStatus CheckConnectionStatus(out byte[] buffer)
        {
            //TODO: Allocate Buffer
            byte[] _recbuffer = new byte[1024];
            buffer = null;
            int _recdataSize;
            byte _recerror;
            status = MVRInputStatus.NONE;
            NetworkEventType networkEvent = NetworkTransport.Receive(out otherHostId, out otherConnectionId, out otherChannelId, _recbuffer, _recbuffer.Length, out _recdataSize, out _recerror);
            switch (networkEvent)
            {
                case NetworkEventType.Nothing:
                    if (isConnected) status = MVRInputStatus.NONE;
                    else status = MVRInputStatus.DISCONNECTED;
                    break;
                case NetworkEventType.ConnectEvent:
                    isConnected = true;
                    status = MVRInputStatus.CONNECTED;
                    break;
                case NetworkEventType.DataEvent:
                    //TODO: Deserealize Code
                    buffer = new byte[_recdataSize];
                    buffer = _recbuffer;
                    status = MVRInputStatus.DATARECEIVED;
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Close();
                    status = MVRInputStatus.DISCONNECTED;
                    break;
                default:
                    if (isConnected) status = MVRInputStatus.NONE;
                    else status = MVRInputStatus.DISCONNECTED;
                    break;
            }
            return status;
        }

        public MVRInputStatus SendToOther(byte[] _sendbuffer = null)
        {
            if (!isConnected) return MVRInputStatus.DISCONNECTED;

            byte _senderror;
            if (NetworkTransport.Send(sockedId, otherConnectionId, otherChannelId, _sendbuffer, _sendbuffer.Length, out _senderror))
            {
                return MVRInputStatus.DATASENT;
            }

            return MVRInputStatus.FAILEDTOSEND;
        }

        public byte[] ObjectToByteArray(System.Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public System.Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                System.Object obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

    }
}


