using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Blue,
    Green,
    Orange,
    Pink,
    Red,
    Sky,
    Yellow,
}
public class PuzzlePiece : MonoBehaviour
{

    public Sprite[] PieceSprites;
    public PieceType MyType;
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
