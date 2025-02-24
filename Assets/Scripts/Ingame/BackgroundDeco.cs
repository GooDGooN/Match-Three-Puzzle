using UnityEngine;
using UnityEngine.UI;

public class BackgroundDeco : MonoBehaviour
{
    public Texture2D NormalMap;
    public Texture2D Mask;
    private Image myImage { get => GetComponent<Image>(); }

    private void Start()
    {
        myImage.material = new Material(myImage.material);
        myImage.material.SetTexture("_MainTex", myImage.sprite.texture);
        myImage.material.SetTexture("_NormalMap", NormalMap);
        myImage.material.SetTexture("_MaskTex", Mask);
        myImage.material.SetVector("_Color3", myImage.color);
    }
}
