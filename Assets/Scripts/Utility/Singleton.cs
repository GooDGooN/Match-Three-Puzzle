using Unity.VisualScripting;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get => instance;
    }
    protected virtual void Awake()
    {
        var objs = FindObjectsByType(typeof(T), FindObjectsSortMode.None);
        if (objs.Length > 0)
        {
            foreach (var obj in objs)
            {
                if(obj != this)
                {
                    Destroy(obj);
                }
            }
        }
        instance = this as T;
    }
}
