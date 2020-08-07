# UnityEventBus

## 描述
借鉴了下的EventBus。对于订阅方，只需要实现事件处理方法后，调用一下订阅方法就可以了。对于发送方，打包好对象，直接发送就行。过程十分简单便捷

## 使用说明

### 事件
- 任何class都可作为事件发送
- class名为事件类型
- class内部成员即为事件数据

### 事件注册

#### 在事件处理方法上添加特性
特性有如下几种：
- 普通事件：
    - 普通事件可用于数据处理，可在任意线程执行：
        - 特性[Subscriber(CallbackPriority = )] 指定回调优先级 0-99 数字越小优先级越高
            

- 主线程事件：
     - 主线程事件用于处理必须在U3D主线程执行的回调
        - 特性[SubscriberMain]
            - CallbackPriority 指定回调优先级 0-99 数字越小优先级越高
            - CallbackExecuteType 特性枚举有：
                - ExecuteType.UPDATE 在Update执行回调
                - ExecuteType.FIXEDUPDATE 在FixedUpdate执行回调

例子如下：
```
[Subscriber(CallbackPriority = 99)]
public void Rec4(EventBB e)
{
    Debug.Log("BB 优先级99，普通订阅事件， 变量a:" + e.a);

}


[SubscriberMain(CallbackPriority = 30, CallbackExecuteType = ExecuteType.UPDATE)]
public void recm2(EventBB e)
{
     Debug.Log("BB 优先级30，在update里调用此条事件，变量：a" + e.a);
}



[SubscriberMain(CallbackPriority = 95, CallbackExecuteType = ExecuteType.FIXEDUPDATE)]
public void recf3(EventBB e)
{
    Debug.Log("BB 优先级95，在fixedupdate里调用此条事件 变量：a" + e.a);
}  
```

#### 在Awake或者Start内部调用register，注册事件：
- 注册事件：

例子如下：
```
UnityEventBus.Register(this);
```

### 事件发送：
- 拿到EventBus实例，发送事件：

例子如下：
```
var bus = UnityEventBus.Instance;
EventAA e = new EventAA();
EventBB e2 = new EventBB();
e.a = 10;
e2.a = 20;
bus.PostEvent(e);//同步发送事件（对主线程事件类型的回调无作用）
bus.PostEventAsync(e2); //异步发送事件（对主线程事件类型的回调无作用）
```

### 事件取消注册：
- 调用unregister,unregisterMainThread,unregisterAll取消注册

例子如下：
```
UnityEventBus.Unregister(this, typeof(EventAA)); //取消注册的普通事件
UnityEventBus.UnregisterMainThread(this, typeof(EventAA));//取消主线程事件
UnityEventBus.UnregisterAll(this, typeof(EventAA));//取消所有事件
```