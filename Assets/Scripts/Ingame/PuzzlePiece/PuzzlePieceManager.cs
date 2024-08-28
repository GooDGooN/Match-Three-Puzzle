using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
    public struct Field
    {
        public int Width;
        public int Height;
        public Vector3Int Size;

        public Field(int width, int height)
        {
            Size = new Vector3Int(width, height);
            Width = width;
            Height = height;
        }

        public static Vector3Int operator /(Field a, int b)
        {
            return a.Size / b;
        }
    }

    public List<PuzzlePiece> PieceList;
    public GameObject PieceContainer;
    public PuzzlePiece[,] PieceField;
    public readonly int PieceSize = 36;
    public readonly Field FieldInfo = new Field(7, 7);


    private void Awake()
    {
        PieceField = new PuzzlePiece[FieldInfo.Width, FieldInfo.Height];

        PieceContainer = new GameObject("PuzzlePieceContainer");
        PieceContainer.transform.parent = transform;
    } 



    // Update is called once per frame
    private void Update()
    {
        
    }
}
