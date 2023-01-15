namespace ThingsGateway.Foundation
{

    public abstract class ReadWriteDevicesUdpClientBase : ReadWriteDevicesClientBase
    {

        public UdpSession UdpSession { get; }

        public ReadWriteDevicesUdpClientBase(UdpSession udpSession)
        {
            UdpSession = udpSession;
            SetDataAdapter();
        }
        public override void Connect()
        {
            UdpSession.Start();
        }

        public override void Disconnect()
        {
            UdpSession.Stop();
        }
        public override string ToString()
        {
            return UdpSession.RemoteIPHost.ToString();
        }
        public override OperResult<byte[]> Send(byte[] data, WaitingOptions waitingOptions = null)
        {
            try
            {
                if (waitingOptions == null) { waitingOptions = new WaitingOptions(); waitingOptions.ThrowBreakException = true; waitingOptions.AdapterFilter = AdapterFilter.NoneAll; }
                ResponsedData result = UdpSession.GetWaitingClient(waitingOptions).SendThenResponse(data, TimeOut, CancellationToken.None);
                return OperResult.CreateSuccessResult(result.Data);
            }
            catch (Exception ex)
            {
                return new OperResult<byte[]>(ex);
            }
        }


    }
}
