using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reciver2 : MonoBehaviour {

    public EventBus eventBus;

    void Start()
    {
        eventBus.register(this);
        StartCoroutine(unre());
    }

    [Subscriber]
    public void Rec1(Event e)
    {
        Debug.Log("REC ATTR OK" + e.a);
    }

    public void EBRec1(Event e)
    {
        Debug.Log("REC EB OK" + e.a);
    }


    public void EBMRec1(Event e)
    {
        Debug.Log("REC EBM OK" + e.a);
    }

    [SubscriberMain]
    public void rec2(Event e)
    {
        Debug.Log("REC ATTR OK" + e.a);
    }

    IEnumerator unre()
    {
        yield return new WaitForSeconds(10);
        eventBus.unregister(this, typeof(Event));
        yield return new WaitForSeconds(10);
        eventBus.unregisterMainThread(this,typeof(Event));
    }






}
