using cn.blockstudio.unityeventbus;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


    public class Reciver : MonoBehaviour
    {

        public UnityEventBus eventBus;

        void Start()
        {
            eventBus = UnityEventBus.GetInstance();
            eventBus.register(this);
            StartCoroutine(unre());
        }

        [Subscriber(CallbackPriority = 1)]
        public void Rec1(EventAA e)
        {
            Debug.Log("AA 优先级1，普通订阅事件， 变量a:"+e.a);

        }

        [Subscriber(CallbackPriority = 55)]
        public void Rec2(EventAA e)
        {
            Debug.Log("AA 优先级55，普通订阅事件， 变量a:" + e.a);

        }


        [Subscriber(CallbackPriority = 100)]
        public void Rec3(EventAA e)
        {
            Debug.Log("AA 优先级99，普通订阅事件， 变量a:" + e.a);

        }

    [Subscriber(CallbackPriority = 99)]
    public void Rec4(EventBB e)
    {
        Debug.Log("BB 优先级99，普通订阅事件， 变量a:" + e.a);

    }


    [SubscriberMain(CallbackPriority = 0, CallbackExecuteType = ExecuteType.UPDATE)]
        public void recm1(EventBB e)
        {
            Debug.Log("BB 优先级0，在update里调用此条事件，变量：a" + e.a);
        }

        [SubscriberMain(CallbackPriority = 30, CallbackExecuteType = ExecuteType.UPDATE)]
        public void recm2(EventBB e)
        {
            Debug.Log("BB 优先级30，在update里调用此条事件，变量：a" + e.a);
        }

        [SubscriberMain(CallbackPriority = 50, CallbackExecuteType = ExecuteType.UPDATE)]
        public void recm3(EventBB e)
        {
            Debug.Log("BB 优先级50，在update里调用此条事件，变量：a" + e.a);

        }


        [SubscriberMain(CallbackPriority = 6, CallbackExecuteType = ExecuteType.FIXEDUPDATE)]
        public void recf1(EventBB e)
        {
            Debug.Log("BB 优先级6，在fixedupdate里调用此条事件 变量：a" + e.a);
        }

        [SubscriberMain(CallbackPriority = 9, CallbackExecuteType = ExecuteType.FIXEDUPDATE)]
        public void recf2(EventBB e)
        {
            Debug.Log("BB 优先级9，在fixedupdate里调用此条事件 变量：a" + e.a);
        }


        [SubscriberMain(CallbackPriority = 95, CallbackExecuteType = ExecuteType.FIXEDUPDATE)]
        public void recf3(EventBB e)
        {
            Debug.Log("BB 优先级95，在fixedupdate里调用此条事件 变量：a" + e.a);
        }


        IEnumerator unre()
        {
            yield return new WaitForSeconds(10);
            eventBus.unregister(this, typeof(EventAA));
            Debug.Log("卸载AA");
            yield return new WaitForSeconds(10);
            eventBus.unregisterMainThread(this, typeof(EventBB));
            Debug.Log("主线程卸载BB");
            yield return new WaitForSeconds(10);
            eventBus.unregisterAll(this, typeof(EventBB));
            Debug.Log("卸载全部BB");
    }






    }



