using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngameTouchManager : MonoBehaviour
{
    private static RaycastHit2D[] raycastHits = new RaycastHit2D[5];

    public static GameObject[] GetMousePointObjects(LayerMask targetLayer = default)
    {
        if(targetLayer == default)
        {
            targetLayer = ~0;
        }
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var length = Physics2D.RaycastNonAlloc(mouseWorldPos, Vector2.zero, raycastHits, 0.0f, targetLayer.value);
        if (length > 0)
        {
            var objs = new GameObject[length];
            for (int i = 0; i < length; i++)
            {
                objs[i] = raycastHits[i].collider.gameObject;
            }
            raycastHits = new RaycastHit2D[5];
            return objs;
        }
        return null;
    }
}
