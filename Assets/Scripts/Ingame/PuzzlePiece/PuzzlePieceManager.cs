using System;
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
    public List<PuzzlePiece>[] PieceField;
    public readonly int PieceSize = 36;
    public readonly Field FieldInfo = new Field(7, 7);


    private void Awake()
    {
        PieceField = new List<PuzzlePiece>[FieldInfo.Width];

        for (int i = 0; i < PieceField.Length; i++)
        {
            PieceField[i] = new List<PuzzlePiece>();
        }
        foreach (ref var column in PieceField.AsSpan())
        {
           column = new List<PuzzlePiece>();
        }

        PieceContainer = new GameObject("PuzzlePieceContainer");
        PieceContainer.transform.parent = transform;
    } 

    public bool IsPlaceEmpty(int x, int y)
    {
        if(y < PieceField[x].Count)
        {
            if (PieceField[x][y].MyType != PieceType.None)
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
