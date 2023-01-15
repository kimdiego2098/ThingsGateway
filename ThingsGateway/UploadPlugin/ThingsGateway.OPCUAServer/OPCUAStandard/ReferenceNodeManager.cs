using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Opc.Ua;
using Opc.Ua.Server;

using System.Text.RegularExpressions;

using ThingsGateway.Application.Core;

namespace ThingsGateway.OPCUAServer;
public class ReferenceNodeManager : CustomNodeManager2
{

    public const string ReferenceServer = "http://opcfoundation.org/Quickstarts/ReferenceServer";

    protected Dictionary<string, BaseDataVariableState> dict_BaseDataVariableState = new();

    private AllDeviceData _allDeviceData;
    private DeviceCollectService deviceCollectService;
    private ILogger _logger;
    private IServiceProvider _serviceProvider;
    public ReferenceNodeManager(ILogger logger, IServiceProvider serviceProvider, IServerInternal server, ApplicationConfiguration configuration)
                                : base(server, configuration, ReferenceServer)
    {

        _serviceProvider = serviceProvider;
        deviceCollectService = _serviceProvider.GetBackgroundService<DeviceCollectService>();
        _allDeviceData = _serviceProvider.GetService<AllDeviceData>();
        _logger = logger;
        SystemContext.NodeIdFactory = this;
    }

    /// <summary>
    /// 在服务器端直接更改对应数据节点的值，并通知客户端
    /// </summary>
    /// <param name="nodeId">节点的名称</param>
    /// <param name="value">值，需要类型匹配</param>
    public void ChangeNodeData(string nodeId, object value, DateTime dateTime, int quality)
    {
        if (dict_BaseDataVariableState.ContainsKey(nodeId))
        {
            lock (Lock)
            {
                dict_BaseDataVariableState[nodeId].Value = value;
                dict_BaseDataVariableState[nodeId].Timestamp = dateTime;
                dict_BaseDataVariableState[nodeId].ClearChangeMasks(SystemContext, false);
            }
        }
        else
        {
            _logger?.LogError(nodeId + "节点不存在，更新失败！");
        }
    }

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        lock (Lock)
        {
            //base.CreateAddressSpace(externalReferences);
            IList<IReference> references = null;
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
            {
                externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
            }
            try
            {

                // 开始寻找设备信息，并计算一些节点信息
                var devarray = _allDeviceData.Devices;
                FolderState rootFolder = CreateFolder(null, "ThingsGateway", "ThingsGateway");
                rootFolder.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, rootFolder.NodeId));
                rootFolder.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(rootFolder);
                foreach (var item in devarray)
                {
                    FolderState root = CreateFolder(rootFolder, item.Name, item.Description);
                    root.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                    root.EventNotifier = EventNotifiers.SubscribeToEvents;
                    //AddRootNotifier(root);
                    foreach (var selected in item.DeviceVariables)
                    {
                        var dataUrl = item.Name + @"." + selected.Name;
                        BaseDataVariableState scalarInstructions = CreateVariable(root, selected.Name, selected.Description, DataNodeType(selected), ValueRanks.Scalar, ProtectTypeTrans(selected.ProtectType), selected.Value);
                        scalarInstructions.Specification = dataUrl;
                        dict_BaseDataVariableState.Add(scalarInstructions.NodeId.ToString(), scalarInstructions);
                        scalarInstructions.OnWriteValue += OnWriteValue;
                    }
                }

                AddPredefinedNode(SystemContext, rootFolder);

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ToString());
            }
        }

    }

    public byte ProtectTypeTrans(ProtectTypeEnum protectTypeEnum)
    {
        switch (protectTypeEnum)
        {
            case ProtectTypeEnum.ReadOnly: return AccessLevels.CurrentRead;
            case ProtectTypeEnum.ReadWrite:
                return AccessLevels.CurrentReadOrWrite;
            default:
                return AccessLevels.CurrentRead;
        }
    }
    public void UpVariable(DeviceVariable variable)
    {
        try
        {
            var uaTag = dict_BaseDataVariableState.Values.FirstOrDefault(it => it.SymbolicName == variable.Name);
            if (uaTag == null) return;
            object initialItemValue = null;
            initialItemValue = variable.Value;
            if (initialItemValue != null)
            {
                var code = variable.Quality == 192 ? StatusCodes.Good : StatusCodes.Bad;
                if (uaTag.Value != initialItemValue)
                    ChangeNodeData(uaTag.NodeId.ToString(), initialItemValue, variable.ChangeTime, variable.Quality);
                if (uaTag.StatusCode != code)
                    uaTag.SetStatusCode(SystemContext, code, variable.ChangeTime);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, ToString());
        }
    }

    protected virtual FolderState CreateFolder(NodeState parent, string name, string description)
    {
        FolderState folder = new FolderState(parent);

        folder.SymbolicName = name;
        folder.ReferenceTypeId = ReferenceTypes.Organizes;
        folder.TypeDefinitionId = ObjectTypeIds.FolderType;
        folder.Description = description;
        if (parent == null)
        {
            folder.NodeId = new NodeId(name, NamespaceIndex);
        }
        else
        {
            folder.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
        }
        folder.BrowseName = new QualifiedName(name, NamespaceIndex);
        folder.DisplayName = new LocalizedText(name);
        folder.WriteMask = AttributeWriteMask.None;
        folder.UserWriteMask = AttributeWriteMask.None;
        folder.EventNotifier = EventNotifiers.None;

        if (parent != null)
        {
            parent.AddChild(folder);
        }

        return folder;
    }

    /// <summary>
    /// 创建一个值节点，类型需要在创建的时候指定
    /// </summary>
    protected BaseDataVariableState CreateVariable(NodeState parent, string name, string description, NodeId dataType, int valueRank, byte accessLevels, object defaultValue = null)
    {
        BaseDataVariableState variable = new BaseDataVariableState(parent);

        variable.SymbolicName = name;
        variable.ReferenceTypeId = ReferenceTypes.Organizes;
        variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
        if (parent == null)
        {
            variable.NodeId = new NodeId(name, NamespaceIndex);
        }
        else
        {
            variable.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
        }
        variable.Description = description;
        variable.BrowseName = new QualifiedName(name, NamespaceIndex);
        variable.DisplayName = new LocalizedText(name);
        variable.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
        variable.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
        variable.DataType = dataType;
        variable.ValueRank = valueRank;
        variable.AccessLevel = accessLevels;
        variable.UserAccessLevel = accessLevels;
        variable.Historizing = false;
        variable.Value = defaultValue ?? Opc.Ua.TypeInfo.GetDefaultValue(dataType, valueRank, Server.TypeTree);
        variable.StatusCode = StatusCodes.Good;
        variable.Timestamp = DateTime.Now;

        if (parent != null)
        {
            parent.AddChild(variable);
        }

        return variable;
    }


    private NodeId DataNodeType(DeviceVariable deviceVariable)
    {
        var tp = deviceVariable.DataType;
        if (tp == typeof(bool))
            return DataTypeIds.Boolean;
        if (tp == typeof(byte))
            return DataTypeIds.Byte;

        if (tp == typeof(sbyte))
            return DataTypeIds.SByte;
        if (tp == typeof(Int16))
            return DataTypeIds.Int16;

        if (tp == typeof(UInt16))
            return DataTypeIds.UInt16;

        if (tp == typeof(Int32))
            return DataTypeIds.Int32;

        if (tp == typeof(UInt32))
            return DataTypeIds.UInt32;

        if (tp == typeof(Int64))
            return DataTypeIds.Int64;

        if (tp == typeof(UInt64))
            return DataTypeIds.UInt64;

        if (tp == typeof(float))
            return DataTypeIds.Float;

        if (tp == typeof(Double))
            return DataTypeIds.Double;

        if (tp == typeof(String))
            return DataTypeIds.String;

        if (tp == typeof(DateTime))
            return DataTypeIds.TimeString;

        return DataTypeIds.ObjectNode;
    }
    private ServiceResult OnWriteValue(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
    {
        BaseDataVariableState variable = node as BaseDataVariableState;
        // 验证数据类型。
        Opc.Ua.TypeInfo typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
            value,
            variable.DataType,
            variable.ValueRank,
            context.NamespaceUris,
            context.TypeTable);

        if (typeInfo == null || typeInfo == Opc.Ua.TypeInfo.Unknown)
        {
            return StatusCodes.BadTypeMismatch;
        }
        // 检查索引范围。
        if (dict_BaseDataVariableState.ContainsValue(variable))
        {
            if (StatusCode.IsGood(variable.StatusCode))
            {
                //仅当指定了值时才将值写入缓冲区
                if (variable.Value != null)
                {
                    string str = new Regex(@"[.]").Replace(variable.Specification.ToString(), @"/", 1);

                    var kv = new Dictionary<string, object>();
                    kv.Add(variable.SymbolicName, value);
                    var result = deviceCollectService.InvokeDeviceMed("OPCUASERVER", kv, "OPCUAServer").Result;
                    if (result.IsSuccess)
                    {
                        return StatusCodes.Good;
                    }
                }
            }

        }
        return StatusCodes.BadWaitingForResponse;

    }

}
