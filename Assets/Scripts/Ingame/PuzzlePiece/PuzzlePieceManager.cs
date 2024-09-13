using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public GameObject SelectedIcon;
    private PuzzlePiece selectedPuzzlePiece;

    private void Awake()
    {
        PieceField = new List<PuzzlePiece>[FieldInfo.Width];
        SelectedIcon.SetActive(false);

        for (int i = 0; i < PieceField.Length; i++)
        {
            PieceField[i] = new List<PuzzlePiece>();
            for (int j = 0; j < FieldInfo.Height; j++)
            {
                PieceField[i].Add(null);
            }
        }

        PieceContainer = new GameObject("PuzzlePieceContainer");
        PieceContainer.transform.parent = transform;
    }


    // Update is called once per frame
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var pieceObjs = IngameTouchManager.GetMousePointObjects(1 << 6);
            if(pieceObjs != null)
            {
                var targetPiece = pieceObjs[0].GetComponent<PuzzlePiece>();
                if (selectedPuzzlePiece == null)
                {
                    selectedPuzzlePiece = targetPiece;
                    SelectedIcon.SetActive(true);
                    SelectedIcon.transform.position = selectedPuzzlePiece.transform.position;
                }
                else
                {
                    if (targetPiece.GetNearPieces().Contains(selectedPuzzlePiece))
                    {
                        selectedPuzzlePiece.SwapPiece(targetPiece);
                    }
                    selectedPuzzlePiece = null;
                    SelectedIcon.SetActive(false);
                }
            }
        }
    }
    #region PlaceCheck
    public bool IsPlaceEmpty(int x, int y)
    {
        if (y < PieceField[x].Count)
        {
            if (PieceField[x][y] != null)
            {
                if (PieceField[x][y].MyType != PieceType.None)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsPlaceEmpty((int, int) posTuple)
    {
        return IsPlaceEmpty(posTuple.Item1, posTuple.Item2);
    }

    public bool IsPlaceAreExist((int, int) posTuple)
    {
        return IsPlaceAreExist(posTuple.Item1, posTuple.Item2);
    }

    public bool IsPlaceAreExist(int x, int y)
    {
        if (x != Mathf.Clamp(x, 0, FieldInfo.Width - 1) || y != Mathf.Clamp(y, 0, FieldInfo.Height - 1))
        {
            return false;
        }
        return true;
    }

    #endregion
}
