using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public List<PuzzlePiece> PieceList;
    public List<PuzzlePiece> HintPieceList;
    public List<PuzzlePiece>[] PieceField;
    public GameObject PuzzlePiecePrefab;
    public GameObject PieceContainer;
    public readonly int PieceSize = 36;
    public readonly Field FieldInfo = new Field(7, 7);

    public GameObject SelectedIcon;
    private Vector2Int matchablePieceAmount = new Vector2Int(3, 5);
    private PuzzlePiece selectedPuzzlePiece;
    private PuzzlePiece swapTargetPuzzlePiece;

    private Queue<PuzzlePiece> repositionedPieceQueue;
    private StateController<PuzzlePieceManager> myStateController;

    public string mystate;

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
        repositionedPieceQueue = new();
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

        if (PieceField != null)
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
                        var nextTarget = PieceField[nextPos.x][nextPos.y];
                        if (nextTarget.MyType == origin.MyType && nextTarget != origin)
                        {
                            testList.Add(nextTarget);
                            nextPos += dir;
                        }
                    }
                    if (IsPlaceAreExist(prevPos) && !IsPlaceEmpty(prevPos))
                    {
                        var prevTarget = PieceField[prevPos.x][prevPos.y];
                        if (prevTarget.MyType == origin.MyType && prevTarget != origin)
                        {
                            testList.Add(prevTarget);
                            prevPos -= dir;
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
        return (new Vector3Int(x, y) - (FieldInfo.Size / 2)) * PieceSize;
    }

    public Vector3Int GetPiecePosition((int, int) targetIndex)
    {
        return GetPiecePosition(targetIndex.Item1, targetIndex.Item2);
    }

    private void RepositionPiece(PuzzlePiece target, (int, int) targetIndex, TweenCallback callback = null, float time = 0.5f)
    {
        var pos = GetPiecePosition(targetIndex);
        target.MyIndex = targetIndex;
        PieceField[targetIndex.Item1][targetIndex.Item2] = target;
        target.DOKill();
        target.transform.DOMove(pos, time).onComplete = callback;
    }
    #endregion

    #region Delay
    private void DelayPlayMethod(Action targetMethod, float delayTime)
    {
        StartCoroutine(DelayingPlayMethod(targetMethod, delayTime));
    }

    private IEnumerator DelayingPlayMethod(Action targetMethod, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        targetMethod.Invoke();
    }

    private void SetHintCountDown(bool stopCoroutine = false)
    {
        if (stopCoroutine && hintCountDownCoroutine != null)
        {
            StopCoroutine(hintCountDownCoroutine);
            return;
        }
        StartCoroutine(HintCountDown());
    }
    private IEnumerator HintCountDown()
    {
        yield return new WaitForSeconds(4.0f);
        if(myStateController.CurrentState.GetType() == typeof(PieceManagerIdleState))
        {
            HintPieceList.ForEach(piece => piece.MyAnimator.SetBool("Hint", true));
        }
    }
    #endregion

}

