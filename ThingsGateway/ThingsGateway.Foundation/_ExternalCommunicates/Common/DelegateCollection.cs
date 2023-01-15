/// <summary>
/// 客户端正在断开连接
/// </summary>
/// <typeparam name="TClient"></typeparam>
/// <param name="client"></param>
/// <param name="e"></param>
public delegate void ClientDisconnectingEventHandler<TClient>(TClient client);

