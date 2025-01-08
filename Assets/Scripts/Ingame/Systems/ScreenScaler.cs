
using UnityEngine;
using UnityEngine.UIElements;

public class SreenScaler : MonoBehaviour
{
    private (float, float) ratio = (0.0f, 0.0f);
    private readonly (float, float) targetScreenSize = (480, 800);
    private void LateUpdate()
    {
        ratio.Item1 = Screen.width / targetScreenSize.Item1;
        ratio.Item2 = Screen.height / targetScreenSize.Item2;
        var scale = ratio.Item1 > ratio.Item2 ? ratio.Item2 : ratio.Item1;
        
        var screenWidth = targetScreenSize.Item1 * scale;
        var screenHeight = targetScreenSize.Item2 * scale;

        var rectx = targetScreenSize.Item1 % screenWidth;
        var recty = targetScreenSize.Item2 % screenHeight;

        if(rectx != 0)
        {
            rectx = (Screen.width - screenWidth) / 2;
        }
        if(recty != 0)
        {
            recty = (Screen.height - screenHeight) / 2;
        }

        Camera.main.pixelRect = new Rect(rectx, recty, screenWidth, screenHeight);
    }
}

