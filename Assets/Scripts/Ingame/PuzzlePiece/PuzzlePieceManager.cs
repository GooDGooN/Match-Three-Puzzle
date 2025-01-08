using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager : MonoBehaviour
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

    public class PieceField
    {
        public PuzzlePiece[][] Pieces
        {
            get;
            private set;
        }

        public PieceField(int width, int height)
        {
            Pieces = new PuzzlePiece[width][];
            for(int i = 0; i < Pieces.Length; i++)
            {
                Pieces[i] = new PuzzlePiece[height];
            }
        }
       
        public PuzzlePiece this[int x, int y]
        {
            get => Pieces[x][y];
            set => Pieces[x][y] = value;
        }

        public PuzzlePiece[] this[int x]
        {
            get => Pieces[x];
        }

        public void RepositionNulls()
        {
            for(int x = 0; x < Pieces.Length; x++)
            {
                var ty = -1;
                for (int y = 0; y < Pieces[x].Length; y++)
                {
                    if(Pieces[x][y] == null && ty == -1)
                    {
                        ty = y;
                    }
                    else if (Pieces[x][y] != null && ty != -1)
                    {
                        Pieces[x][ty] = Pieces[x][y];
                        Pieces[x][y] = null;
                        y = ty;
                        ty = -1;
                    }
                }
            }
        }

        public int GetNullYPos(int x)
        {
            for(int y = 0; y < Pieces[x].Length; y++)
            {
                if (Pieces[x][y] == null)
                {
                    return y;
                }
            }
            return -1;
        }
    }

    public enum PieceRepositionType
    {
        Generate,
        Swap,
        Refill,
    }

    public List<PuzzlePiece> PieceList;
    public List<PuzzlePiece> HintPieceList;
    public List<PuzzlePiece> BombPieceList;
    public PieceField MyPieceField;

    public GameObject PuzzlePiecePrefab;
    public GameObject PieceContainer;
    public readonly int PieceSize = 40;
    public readonly Field FieldInfo = new Field(8, 8);

    public GameObject SelectedIcon;
    private Vector2Int matchablePieceAmount = new Vector2Int(3, 5);
    private PuzzlePiece selectedPuzzlePiece;
    private PuzzlePiece swapTargetPuzzlePiece;

    private Queue<PuzzlePiece> repositionedPieceQueue;
    private StateController<PuzzlePieceManager> myStateController;

    public string mystate;

    private void Awake()
    {
        MyPieceField = new PieceField(FieldInfo.Width, FieldInfo.Height);
        SelectedIcon.SetActive(false);

        PieceContainer = new GameObject("PuzzlePieceContainer");
        PieceContainer.transform.parent = transform;
        PieceContainer.transform.localPosition = Vector3.zero;
        repositionedPieceQueue = new();
        BombPieceList = new();
        HintPieceList = new();
        myStateController = new StateController<PuzzlePieceManager>(this);
    }
    private void Start()
    {
        myStateController.ChangeState<PieceManagerGeneratePieceState>();
    }

    private void FixedUpdate()
    {
        myStateController.CurrentState.StateFixedUpdate();
    }
    private void Update()
    {
        mystate = myStateController.CurrentState.ToString();
        myStateController.CurrentState.StateUpdate();
    }
    #region Field Check
    public bool IsPlaceEmpty(int x, int y)
    {
        if (MyPieceField[x][y] != null)
        {
            if (MyPieceField[x][y].MyType != PieceType.None || MyPieceField[x][y].MyType != PieceType.None || MyPieceField[x][y].MyType != PieceType.Block)
            {
                return false;
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

    #region Get Piece
    public PuzzlePiece[] GetMatchablePieces(PuzzlePiece origin)
    {
        var resultList = new List<PuzzlePiece>();
        var testList = new List<PuzzlePiece>();
        var dirs = new Vector2Int[2]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1)
        };

        if (MyPieceField != null && origin.MySubType != PieceSubType.Rainbow)
        {
            foreach (var dir in dirs)
            {
                var nextPos = new Vector2Int(origin.MyIndex.Item1 + dir.x, origin.MyIndex.Item2 + dir.y);
                var prevPos = new Vector2Int(origin.MyIndex.Item1 - dir.x, origin.MyIndex.Item2 - dir.y);
                var lastLength = -1;
                while (true)
                {
                    if (IsPlaceAreExist(nextPos) && !IsPlaceEmpty(nextPos))
                    {
                        var nextTarget = MyPieceField[nextPos.x][nextPos.y];
                        if(nextTarget.MySubType != PieceSubType.Rainbow)
                        {
                            if (nextTarget.MyType == origin.MyType && nextTarget != origin)
                            {
                                testList.Add(nextTarget);
                                nextPos += dir;
                            }
                        }
                    }
                    if (IsPlaceAreExist(prevPos) && !IsPlaceEmpty(prevPos))
                    {
                        var prevTarget = MyPieceField[prevPos.x][prevPos.y];
                        if (prevTarget.MySubType != PieceSubType.Rainbow) 
                        {
                            if (prevTarget.MyType == origin.MyType && prevTarget != origin)
                            {
                                testList.Add(prevTarget);
                                prevPos -= dir;
                            }
                        }
                    }
                    if (lastLength == testList.Count)
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

    public PuzzlePiece GetUseablePiece()
    {
        foreach (var piece in PieceList)
        {
            if (piece.MyIndex == (-1, -1))
            {
                return piece;
            }
        }
        return null;
    }
    #endregion

    #region Position of Piece
    public Vector3Int GetPiecePosition(int x, int y)
    {
        var delta = FieldInfo.Size.x % 2 == 0 ? 20 : 0;
        var result = (new Vector3Int(x, y) - (FieldInfo.Size / 2)) * PieceSize;
        result.x += delta;
        return result;
    }

    public Vector3Int GetPiecePosition((int, int) targetIndex)
    {
        return GetPiecePosition(targetIndex.Item1, targetIndex.Item2);
    }

    private void RepositionPiece(PuzzlePiece target, (int, int) targetIndex, PieceRepositionType repositionType, TweenCallback callback = null)
    {
        var pos = GetPiecePosition(targetIndex);
        target.MyIndex = targetIndex;
        MyPieceField[targetIndex.Item1][targetIndex.Item2] = target;
        target.DOKill();

        switch(repositionType)
        {
            case PieceRepositionType.Generate:
                target.transform.DOLocalMove(pos, 0.5f).SetEase(Ease.InOutSine).onComplete = callback;
                break;
            case PieceRepositionType.Swap:
                target.transform.DOLocalMove(pos, 0.25f).SetEase(Ease.InOutSine).onComplete = callback;
                break;
            case PieceRepositionType.Refill:
                target.transform.DOLocalMove(pos, 0.5f).SetEase(Ease.InCubic).onComplete = callback;
                break;
        }
    }
    #endregion


}

