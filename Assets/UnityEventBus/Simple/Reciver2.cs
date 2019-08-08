using cn.blockstudio.unityeventbus;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace cn.blockstudio.unityeventbus
{
    public class Reciver2 : MonoBehaviour
    {

        public EventBus eventBus;

        void Start()
        {
            eventBus = EventBus.GetInstance();
            eventBus.register(this);
            //StartCoroutine(unre());
        }

        [Subscriber]
        public void Rec1(Event e)
        {
            Debug.Log("REC ATTR OK" + e.a + "  " + Thread.CurrentThread.GetHashCode());

        }



        [SubscriberMain]
        public void rec2(Event e)
        {
            Debug.Log("REC ATTR OK" + e.a + "  " + Thread.CurrentThread.GetHashCode());

        }

        IEnumerator unre()
        {
            yield return new WaitForSeconds(10);
            eventBus.unregister(this, typeof(Event));
            yield return new WaitForSeconds(10);
            eventBus.unregisterAll(this, typeof(Event));
        }






    }

}

