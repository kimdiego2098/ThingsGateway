import { MqttClient, OnMessageCallback } from 'mqtt';
import * as mqtt from "mqtt/dist/mqtt.min.js";
class MQTT {
  url: string; // mqtt地址
  topic: string; //订阅地址
  client!: MqttClient;
  constructor(topic: string) {
    this.topic = topic;
    // 虽然是mqtt但是在客户端这里必须采用websock的链接方式
    this.url =(window as any).baseUrl.baseUrl.replace('http://',"ws://")+"/mqtt";
    
  }

  //初始化mqtt
  init() {
    const options = {
      clean: true, // 保留会话
      connectTimeout: 4000, // 超时时间
      reconnectPeriod: 4000, // 重连时间间隔
      // 认证信息
      clientId: 'thingsgatewayvue3',
      username: 'ThingsGateway',
      password: 'ThingsGateway',
    };
    this.client = mqtt.connect(this.url, options);
    this.client.on('error', (error: any) => {
      console.log(error);
    });
    this.client.on('reconnect', (error: Error) => {
      console.log(error);
    });
  }
  //取消订阅
  unsubscribes() {
    this.client.unsubscribe(this.topic, (error: Error) => {
      if (!error) {
        console.log(this.topic, '取消订阅成功');
      } else {
        console.log(this.topic, '取消订阅失败');
      }
    });
  }
  subscribes() {
  this.client.subscribe(this.topic, (error: any) => {
    if (!error) {
      console.log('订阅成功');
    } else {
      console.log('订阅失败');
    }
  });
}

  //连接
  link() {
    this.client.on('connect', () => {

    });
  }
  //收到的消息
  get(callback: OnMessageCallback) {
    this.client.on('message', callback);
    // console.log(callback,"1010")
  }
  //结束链接
  over() {
    this.client.end();
  }
}
export default MQTT;
