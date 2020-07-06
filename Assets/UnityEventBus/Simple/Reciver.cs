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

        [Subscriber(CallbackPriority = ExecutePriority.HIGH)]
        public void Rec1(EventAA e)
        {
            Debug.Log("高优先级，普通订阅事件， 变量a:"+e.a);

        }

        [Subscriber(CallbackPriority = ExecutePriority.MIDDLE)]
        public void Rec2(EventAA e)
        {
            Debug.Log("中优先级，普通订阅事件， 变量a:" + e.a);

        }


        [Subscriber(CallbackPriority = ExecutePriority.LOW)]
        public void Rec3(EventAA e)
        {
            Debug.Log("低优先级，普通订阅事件， 变量a:" + e.a);

        }



        [SubscriberMain(CallbackPriority = ExecutePriority.HIGH, CallbackExecuteType = ExecuteType.UPDATE)]
        public void recm1(EventBB e)
        {
            Debug.Log("高优先级，在update里调用此条事件，变量：a" + e.a);
        }

        [SubscriberMain(CallbackPriority = ExecutePriority.MIDDLE, CallbackExecuteType = ExecuteType.UPDATE)]
        public void recm2(EventBB e)
        {
            Debug.Log("中优先级，在update里调用此条事件，变量：a" + e.a);
        }

        [SubscriberMain(CallbackPriority = ExecutePriority.LOW, CallbackExecuteType = ExecuteType.UPDATE)]
        public void recm3(EventBB e)
        {
            Debug.Log("低优先级，在update里调用此条事件，变量：a" + e.a);
        }


        [SubscriberMain(CallbackPriority = ExecutePriority.HIGH, CallbackExecuteType = ExecuteType.FIXEDUPDATE)]
        public void recf1(EventBB e)
        {
            Debug.Log("高优先级，在fixedupdate里调用此条事件 变量：a" + e.a);
        }

        [SubscriberMain(CallbackPriority = ExecutePriority.MIDDLE, CallbackExecuteType = ExecuteType.FIXEDUPDATE)]
        public void recf2(EventBB e)
        {
            Debug.Log("中优先级，在fixedupdate里调用此条事件 变量：a" + e.a);
        }


        [SubscriberMain(CallbackPriority = ExecutePriority.LOW, CallbackExecuteType = ExecuteType.FIXEDUPDATE)]
        public void recf3(EventBB e)
        {
            Debug.Log("低优先级，在fixedupdate里调用此条事件 变量：a" + e.a);
        }


        IEnumerator unre()
        {
            yield return new WaitForSeconds(10);
            eventBus.unregister(this, typeof(EventAA));
            yield return new WaitForSeconds(10);
            eventBus.unregisterAll(this, typeof(EventBB));
        }






    }



