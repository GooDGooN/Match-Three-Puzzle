using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
                        var selectedIndex = selectedPuzzlePiece.MyIndex;
                        var targetIndex = targetPiece.MyIndex;

                        PieceField[targetIndex.Item1][targetIndex.Item2] = selectedPuzzlePiece;
                        PieceField[selectedIndex.Item1][selectedIndex.Item2] = targetPiece;
                        
                        selectedPuzzlePiece.MyIndex = targetIndex;
                        targetPiece.MyIndex = selectedIndex;

                        var matchableList = GetMatchablePieces(selectedPuzzlePiece).ToList();
                        matchableList.AddRange(GetMatchablePieces(targetPiece));
                        if(matchableList.Count > 0)
                        {
                            foreach( var piece in matchableList )
                            {
                                Destroy(piece.gameObject);
                            }
                        }    
                        
                        selectedPuzzlePiece.Reposition();
                        targetPiece.Reposition();
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

    public bool IsPlaceEmpty((int, int) tuplePos)
    {
        return IsPlaceEmpty(tuplePos.Item1, tuplePos.Item2);
    }

    public bool IsPlaceEmpty(Vector2Int vectorPos)
    {
        return IsPlaceEmpty(vectorPos.x, vectorPos.y);
    }


    public bool IsPlaceAreExist(int x, int y)
    {
        if (x != Mathf.Clamp(x, 0, FieldInfo.Width - 1) || y != Mathf.Clamp(y, 0, FieldInfo.Height - 1))
        {
            return false;
        }
        return true;
    }

    public bool IsPlaceAreExist((int, int) tuplePos)
    {
        return IsPlaceAreExist(tuplePos.Item1, tuplePos.Item2);
    }

    public bool IsPlaceAreExist(Vector2Int vectorPos)
    {
        return IsPlaceAreExist(vectorPos.x, vectorPos.y);
    }

    #endregion

    #region PieceCheck
    public PuzzlePiece[] GetMatchablePieces(PuzzlePiece origin)
    {
        var resultList = new List<PuzzlePiece>();
        var testList = new List<PuzzlePiece>();
        var dirs = new Vector2Int[2]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1)
        };
            
        if (PieceField != null)
        {
            foreach (var dir in dirs)
            {
                var nextPos = new Vector2Int(origin.MyIndex.Item1 + dir.x, origin.MyIndex.Item2 + dir.y);
                var prevPos = new Vector2Int(origin.MyIndex.Item1 - dir.x, origin.MyIndex.Item2 - dir.y);
                var lastLength = -1;
                while (true)
                {
                    if(IsPlaceAreExist(nextPos) && !IsPlaceEmpty(nextPos))
                    {
                        var nextTarget = PieceField[nextPos.x][nextPos.y];
                        if (nextTarget.MyType == origin.MyType)
                        {
                            testList.Add(nextTarget);
                            nextPos += dir;
                        }
                    }
                    if(IsPlaceAreExist(prevPos) && !IsPlaceEmpty(prevPos))
                    {
                        var prevTarget = PieceField[prevPos.x][prevPos.y];
                        if (prevTarget.MyType == origin.MyType)
                        {
                            testList.Add(prevTarget);
                            prevPos -= dir;
                        }
                    }
                    if(lastLength == testList.Count)
                    {
                        break;
                    }
                    lastLength = testList.Count;
                }
                if (testList.Count >= 2)
                {
                    resultList.AddRange(testList);
                }
                testList.Clear();
            }
        }
        if (resultList.Count > 0)
        {
            resultList.Add(origin);
        }
        return resultList.ToArray();
    }
    #endregion


}
