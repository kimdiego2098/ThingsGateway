using Microsoft.Extensions.Logging;

using Opc.Ua;
using Opc.Ua.Configuration;

using ThingsGateway.Application.Core;

namespace ThingsGateway.OPCUAServer;
public partial class OPCUAServer : UpLoadBase
{
    private ApplicationInstance m_application;
    private ApplicationConfiguration m_configuration;
    private ReferenceServer m_server;

    /// <summary>
    /// 接口约定构造实现
    /// </summary>
    public OPCUAServer(ILogger logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    [DeviceProperty("是否匿名", "")]
    public bool IsAnonymous { get; set; }

    [DeviceProperty("URL", "")]
    public string OpcUaStringUrl { get; set; } = "opc.tcp://127.0.0.1:48321";
    [DeviceProperty("登录密码", "")]
    public string Password { get; set; } = "123456";
    [DeviceProperty("", "")]
    public bool SecurityPolicyBasic128_Sign { get; set; }

    [DeviceProperty("", "")]
    public bool SecurityPolicyBasic128_Sign_Encrypt { get; set; }

    [DeviceProperty("", "")]
    public bool SecurityPolicyBasic256_Sign { get; set; }

    [DeviceProperty("", "")]
    public bool SecurityPolicyBasic256_Sign_Encrypt { get; set; }

    [DeviceProperty("", "")]
    public bool SecurityPolicyNone { get; set; }

    [DeviceProperty("用户名", "")]
    public string UserName { get; set; }

    public override void Dispose()
    {
        m_server.Stop();
        m_server.Dispose();
        m_application.Stop();
    }
    public async Task StartAsync()
    {
        try
        {
            m_application = new ApplicationInstance();
            m_configuration = GetDefaultConfiguration();
            m_application.ApplicationConfiguration = m_configuration;
            m_server = new ReferenceServer(_logger, _serviceProvider);
            m_server.UserName = UserName; m_server.Password = Password;
            await m_application.CheckApplicationInstanceCertificate(false, 0);
            // 启动服务器。
            await m_application.Start(m_server);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "启动OPCUAServer失败");
        }
    }

    protected override void DeviceVariableValueChange(DeviceVariable variable)
    {
        if (!_uploadDevice.InvokeEnable) return;
        m_server?.ReferenceNodeManager?.UpVariable(variable);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await StartAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_uploadDevice.InvokeEnable) continue;
            _uploadDevice.UploadDeviceStatus.ActiveTime = DateTime.Now;
            _uploadDevice.UploadDeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.OnLine;

            await Task.Delay(1000, stoppingToken);

        }
    }

    private ApplicationConfiguration GetDefaultConfiguration()
    {
        ApplicationConfiguration config = new ApplicationConfiguration();
        string url = OpcUaStringUrl;
        // 签名及加密验证
        ServerSecurityPolicyCollection policies = new ServerSecurityPolicyCollection();
        if (SecurityPolicyNone)
        {
            policies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.None,
                SecurityPolicyUri = SecurityPolicies.None
            });
        }
        if (SecurityPolicyBasic128_Sign)
        {
            policies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.Sign,
                SecurityPolicyUri = SecurityPolicies.Basic128Rsa15
            });
        }
        if (SecurityPolicyBasic128_Sign_Encrypt)
        {
            policies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.SignAndEncrypt,
                SecurityPolicyUri = SecurityPolicies.Basic128Rsa15
            });
        }
        if (SecurityPolicyBasic256_Sign)
        {
            policies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.Sign,
                SecurityPolicyUri = SecurityPolicies.Basic256
            });
        }
        if (SecurityPolicyBasic256_Sign_Encrypt)
        {
            policies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.SignAndEncrypt,
                SecurityPolicyUri = SecurityPolicies.Basic256
            });
        }
        // 用户名验证
        UserTokenPolicyCollection userTokens = new UserTokenPolicyCollection();
        if (IsAnonymous)
        {
            userTokens.Add(new UserTokenPolicy(UserTokenType.Anonymous));
        }
        userTokens.Add(new UserTokenPolicy(UserTokenType.UserName));

        config.ApplicationName = "ThingsGateway";
        config.ApplicationType = ApplicationType.Server;
        config.ApplicationUri = OpcUaStringUrl;

        config.SecurityConfiguration = new SecurityConfiguration()
        {
            ApplicationCertificate = new CertificateIdentifier()
            {
                StoreType = "Directory",
                StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault",
                SubjectName = "CN=ThingsGateway, C=ZH, S=GuangZhou, O=OPC Foundation, DC=127.0.0.1",
            },

            TrustedPeerCertificates = new CertificateTrustList()
            {
                StoreType = "Directory",
                StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications",
            },

            TrustedIssuerCertificates = new CertificateTrustList()
            {
                StoreType = "Directory",
                StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities",
            },

            RejectedCertificateStore = new CertificateStoreIdentifier()
            {
                StoreType = "Directory",
                StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
            }
        };

        config.TransportConfigurations = new TransportConfigurationCollection();
        config.TransportQuotas = new TransportQuotas();
        config.TraceConfiguration = new TraceConfiguration()
        {
            OutputFilePath = "OPCUAServer.log",
            TraceMasks = 519,
        };
        config.ServerConfiguration = new ServerConfiguration()
        {
            // 配置登录的地址
            BaseAddresses = new string[]
            {
                    url
            },

            SecurityPolicies = policies,
            UserTokenPolicies = userTokens,
            ShutdownDelay = 1,

            DiagnosticsEnabled = false,           // 是否启用诊断
            MaxSessionCount = 1000,               // 最大打开会话数
            MinSessionTimeout = 10000,            // 允许该会话在与客户端断开时（单位毫秒）仍然保持连接的最小时间
            MaxSessionTimeout = 60000,            // 允许该会话在与客户端断开时（单位毫秒）仍然保持连接的最大时间
            MaxBrowseContinuationPoints = 1000,   // 用于Browse / BrowseNext操作的连续点的最大数量。
            MaxQueryContinuationPoints = 1000,    // 用于Query / QueryNext操作的连续点的最大数量
            MaxHistoryContinuationPoints = 500,   // 用于HistoryRead操作的最大连续点数。
            MaxRequestAge = 1000000,              // 传入请求的最大年龄（旧请求被拒绝）。
            MinPublishingInterval = 100,          // 服务器支持的最小发布间隔（以毫秒为单位）
            MaxPublishingInterval = 3600000,      // 服务器支持的最大发布间隔（以毫秒为单位）1小时
            PublishingResolution = 50,            // 支持的发布间隔（以毫秒为单位）的最小差异
            MaxSubscriptionLifetime = 3600000,    // 订阅将在没有客户端发布的情况下保持打开多长时间 1小时
            MaxMessageQueueSize = 100,            // 每个订阅队列中保存的最大消息数
            MaxNotificationQueueSize = 100,       // 为每个被监视项目保存在队列中的最大证书数
            MaxNotificationsPerPublish = 1000,    // 每次发布的最大通知数
            MinMetadataSamplingInterval = 1000,   // 元数据的最小采样间隔
            AvailableSamplingRates = new SamplingRateGroupCollection(new List<SamplingRateGroup>()
                {
                    new SamplingRateGroup(5, 5, 20),
                    new SamplingRateGroup(100, 100, 4),
                    new SamplingRateGroup(500, 250, 2),
                    new SamplingRateGroup(1000, 500, 20),
                }),
            MaxRegistrationInterval = 30000,   // 两次注册尝试之间的最大时间（以毫秒为单位）

        };
        config.CertificateValidator = new CertificateValidator();
        config.CertificateValidator.Update(config);
        config.Extensions = new XmlElementCollection();

        return config;
    }

}
