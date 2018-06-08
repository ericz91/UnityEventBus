# UnityEventBus

借鉴了下的EventBus。对于订阅方，只需要实现事件处理方法后，调用一下订阅方法就可以了。对于发送方，打包好对象，直接发送就行。过程十分简单便捷

发送方：

```
public class Sender : MonoBehaviour {


        // Use this for initialization
        void Start () {
        StartCoroutine(sendMessage());
        }
     //定义消息的类型和参数，这种方法十分简便，类就是消息类型，成员就是消息参数
    public class MailA
    {
        public int a;
    }


    public class MailB
    {
       public int b;
    }


    IEnumerator sendMessage()
    {
        while (true)
        {
            MailA messageA = new MailA();
            messageA.a = 5;
            MailB messageB = new MailB();
            messageB.b = 10;
            //把消息发送出去
            UnityEventBus.getInstance().post(messageA);
            yield return new WaitForSeconds(2);
            //把消息发送出去
            UnityEventBus.getInstance().post(messageB);
            yield return new WaitForSeconds(2);


        }


    }
}
```




订阅方：

```
public class Reciver : MonoBehaviour {


    // Use this for initialization


    private void Awake()
    {
        //一步订阅所有的消息类型
        UnityEventBus.getInstance().register(this);
    }


    void Start () {
        StartCoroutine(unregister());
        }
        
    //收到消息A的处理方法，传参类型即为消息类型，处理方法要加“eventBusMethod_”
    public void eventBusMethod_ReciverA(Sender.MailA message)
    {
        Debug.Log("SendA:" + message.a);
    }
    //收到消息B的处理方法，//收到消息B的处理方法，传参类型即为消息类型
    public void eventBusMethod_ReciverB(Sender.MailB message)
    {
        Debug.Log("SendB:" + message.b);
    }


    IEnumerator unregister()
    {


        yield return new WaitForSeconds(5);
        //取消特定消息的订阅
        UnityEventBus.getInstance().unregister(this, typeof(Sender.MailB));
        
    }


}
```
