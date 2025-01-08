using System;
using UnityEngine;

public class TimeOverChildren : MonoBehaviour
{
    public void PopDone()
    {
        transform.parent.GetComponent<TimeOver>().PopDone();
    }
}
