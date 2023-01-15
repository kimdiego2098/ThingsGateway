using Microsoft.Extensions.Logging;

namespace ThingsGateway.Application.Core
{
    public abstract class UpLoadBase : IDisposable
    {

        protected ILogger _logger;
        protected IServiceProvider _serviceProvider;
        protected UploadDevice _uploadDevice;

        private bool isLogOut;
        private ILogger privateLogger;
        protected AlarmHostService alarmHostService;
        protected AllDeviceData allDeviceData;
        /// <summary>
        /// 约定构造实现
        /// </summary>
        public UpLoadBase(ILogger logger, IServiceProvider serviceProvider)
        {
            privateLogger = logger;
            _serviceProvider = serviceProvider;

        }

        [DeviceProperty("Debug日志", "")]
        public bool IsLogOut
        {
            get => isLogOut; set
            {
                isLogOut = value;
                if (value)
                    _logger = privateLogger;
            }
        }

        public abstract void Dispose();
        protected void Init()

        {
            alarmHostService = _serviceProvider.GetBackgroundService<AlarmHostService>();
            alarmHostService.OnAlarmChanged += AlarmChnage;
            alarmHostService.OnDeviceStatusChanged += DeviceStatusChnage;
            allDeviceData = _serviceProvider.GetService<AllDeviceData>();
            allDeviceData.DeviceVariables?.ForEach(v => { v.VariableValueChange += DeviceVariableValueChange; });
            allDeviceData.DeviceVariables?.ForEach(v => { v.VariableCollectChange += DeviceVariableCollectChange; });
        }
        /// <summary>
        /// 开始通讯前执行的方法
        /// </summary>
        /// <returns></returns>
        public virtual async Task StartAsync(UploadDevice uploadDevice, CancellationToken stoppingToken)
        {
            _uploadDevice = uploadDevice;
            try
            {
                Init();
                await ExecuteAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ToString());
            }

        }

        /// <summary>
        /// 开始通讯前执行的方法
        /// </summary>
        /// <returns></returns>
        public virtual void Stop()
        {
            var alarmHostService = _serviceProvider.GetBackgroundService<AlarmHostService>();
            alarmHostService.OnAlarmChanged -= AlarmChnage;
            alarmHostService.OnDeviceStatusChanged -= DeviceStatusChnage;
        }

        protected virtual void AlarmChnage(DeviceVariable alarm)
        {
            if (!_uploadDevice.InvokeEnable) return;

        }

        protected virtual void DeviceStatusChnage(Device device)
        {
            if (!_uploadDevice.InvokeEnable) return;
        }

        protected virtual void DeviceVariableCollectChange(DeviceVariable variable)
        {
            if (!_uploadDevice.InvokeEnable) return;

        }
        protected virtual void DeviceVariableValueChange(DeviceVariable variable)
        {
            if (!_uploadDevice.InvokeEnable) return;

        }
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    }
}
