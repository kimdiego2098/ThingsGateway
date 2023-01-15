namespace ThingsGateway.Foundation
{
    /// <summary>
    /// TCP读写设备
    /// </summary>
    public abstract class ReadWriteDevicesTcpClientBase : ReadWriteDevicesClientBase
    {

        public TcpClient TcpClient { get; }

        public ReadWriteDevicesTcpClientBase(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            TcpClient.Connecting += Connecting;
            TcpClient.Connected += Connected;
            TcpClient.Disconnected += Disconnected;
            Logger = TcpClient.Logger;
        }


        public override void Connect()
        {
            TcpClient.Connect(ConnectTimeOut);
        }


        public override void Disconnect()
        {
            TcpClient.Close();
        }

        public override OperResult<byte[]> Send(byte[] data, WaitingOptions waitingOptions = null)
        {
            try
            {
                if (waitingOptions == null) { waitingOptions = new WaitingOptions(); waitingOptions.ThrowBreakException = true; waitingOptions.AdapterFilter = AdapterFilter.NoneAll; }
                Connect();
                ResponsedData result = TcpClient.GetWaitingClient(waitingOptions).SendThenResponse(data, TimeOut, CancellationToken.None);
                return OperResult.CreateSuccessResult(result.Data);

            }
            catch (Exception ex)
            {
                return new OperResult<byte[]>(ex);
            }
        }

        public override string ToString()
        {
            return TcpClient.RemoteIPHost.ToString();
        }

        private void Connected(ITcpClient client, MsgEventArgs e)
        {
            Logger?.Debug(client.GetIPPort() + "连接成功");
        }

        private void Connecting(ITcpClient client, ConnectingEventArgs e)
        {
            Logger?.Debug(client.GetIPPort() + "正在连接");
            SetDataAdapter();
        }

        private void Disconnected(ITcpClientBase client, DisconnectEventArgs e)
        {
            Logger?.Debug(client.GetIPPort() + "断开连接-" + e.Message);
        }

    }
}
