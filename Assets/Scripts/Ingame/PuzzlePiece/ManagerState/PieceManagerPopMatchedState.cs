using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    private List<PuzzlePiece> sameTypePieceList = new();
    private float popStateChangeDelay;
    private const float popDelayValue = 0.3f;
    public class PieceManagerPopMatchedState : BaseFSM<PuzzlePieceManager>
    {
        private bool isRefill;
        private List<PuzzlePiece[]> matchablePiecesList;
        public override void StateEnter()
        {
            matchablePiecesList = new();
            self.popStateChangeDelay = popDelayValue;
            isRefill = false;
            

            if (self.selectedPuzzlePiece != null && self.selectedPuzzlePiece.MySubType == PieceSubType.Rainbow)
            {
                self.StartActiveRainbowBomb(self.selectedPuzzlePiece, self.swapTargetPuzzlePiece.MyType);
                isRefill = true;
            }
            else if (self.swapTargetPuzzlePiece != null && self.swapTargetPuzzlePiece.MySubType == PieceSubType.Rainbow)
            {
                self.StartActiveRainbowBomb(self.swapTargetPuzzlePiece, self.selectedPuzzlePiece.MyType);
                isRefill = true;
            }
            else
            {
                while (self.repositionedPieceQueue.Count > 0)
                {
                    var target = self.repositionedPieceQueue.Dequeue();
                    var matchables = self.GetMatchablePieces(target);
                    if (matchables.Length > 0)
                    {
                        matchablePiecesList.Add(matchables);
                    }
                }

                matchablePiecesList = matchablePiecesList.OrderByDescending(arr => arr.Length).ToList();
                PuzzlePiece specialPiece = null;

                foreach (var matchables in matchablePiecesList)
                {
                    specialPiece = null;
                    var matchLength = matchables.Length;
                    var emptyCount = matchables.Count(arr => arr.MyIndex == (-1, -1));
                    #region Create special piece
                    if (matchLength > 3 && emptyCount == 0)
                    {
                        if(matchables.Count(piece => piece.MySubType != PieceSubType.None) == 0)
                        {
                            #region set the special piece
                            if (matchables.Contains(self.selectedPuzzlePiece))
                            {
                                specialPiece = self.selectedPuzzlePiece;
                            }
                            else if (matchables.Contains(self.swapTargetPuzzlePiece))
                            {
                                specialPiece = self.swapTargetPuzzlePiece;
                            }
                            else
                            {
                                specialPiece = matchables[Random.Range(0, matchables.Length)];
                            }
                            #endregion

                            #region set special piece type
                            if (matchLength >= 6)
                            {
                                specialPiece.TargetChangeSubType = PieceSubType.Rainbow;
                            }
                            else if (matchLength == 5)
                            {
                                if (matchables.Count(piece => piece.MyIndex.Item1 == matchables[0].MyIndex.Item1) == 5 ||
                                    matchables.Count(piece => piece.MyIndex.Item2 == matchables[0].MyIndex.Item2) == 5)
                                {
                                    specialPiece.TargetChangeSubType = PieceSubType.Rainbow;
                                }
                                else
                                {
                                    specialPiece.TargetChangeSubType = PieceSubType.CrossBomb;
                                    specialPiece.TargetChangeType = specialPiece.MyType;
                                }
                            }
                            else if (matchLength == 4)
                            {
                                if (matchables[0].MyIndex.Item1 == matchables[1].MyIndex.Item1)
                                {
                                    specialPiece.TargetChangeSubType = PieceSubType.Hbomb;
                                }
                                else
                                {
                                    specialPiece.TargetChangeSubType = PieceSubType.Vbomb;
                                }
                                specialPiece.TargetChangeType = specialPiece.MyType;
                            }
                            #endregion
                        }

                    }

                    // remove
                    if(emptyCount == 0)
                    {
                        specialPiece?.MyAnimator.SetTrigger("ChangeType");
                        foreach (var piece in matchables)
                        {
                            if (piece != specialPiece && piece.MyIndex != (-1, -1))
                            {
                                if (piece.MySubType != PieceSubType.None)
                                {
                                    self.StartActiveBomb(piece);
                                }
                                else
                                {
                                    self.PopPiece(piece);
                                }
                                isRefill = true;
                            }
                        }
                        GameManager.Instance.AddScore(matchLength);
                    }
                }
                #endregion
            }

        }

        public override void StateExit()
        {
            GameManager.Instance.Combo++;
            self.swapTargetPuzzlePiece = null;
            self.selectedPuzzlePiece = null;
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
            self.popStateChangeDelay -= Time.deltaTime;
            if (self.popStateChangeDelay < 0.0f)
            {
                if (isRefill)
                {
                    self.MyPieceField.RepositionNulls();
                    stateManager.ChangeState<PieceManagerRefillState>();
                    return;
                }
                stateManager.ChangeState<PieceManagerFindMatchableState>();
            }
        }
    }

    private void PopPiece(PuzzlePiece piece)
    {
        piece.transform.DOKill();
        BombPieceList.Remove(piece);
        MyPieceField[piece.MyIndex.Item1, piece.MyIndex.Item2] = null;
        piece.MyIndex = (-1, -1);
        piece.MyAnimator.SetTrigger("Pop");
        piece.PlayPopSound(GameManager.Instance.Combo);
    }

    /// <summary>
    /// Using for in PieceManagerPopMatchableState only
    /// </summary>
    private void StartActiveBomb(PuzzlePiece bombPiece, int scoreMultiply = 2)
    {
        StartCoroutine(ActiveLineBomb(bombPiece, scoreMultiply));
        PopPiece(bombPiece);
        popStateChangeDelay = popDelayValue;
    }

    private void StartActiveRainbowBomb(PuzzlePiece bombPiece, PieceType targetType, int scoreMultiply = 3)
    {
        StartCoroutine(ActiveRainbowBomb(bombPiece, targetType, scoreMultiply));
    }

    private IEnumerator ActiveLineBomb(PuzzlePiece bombPiece, int scoreMultiply)
    {
        var bombTuple = bombPiece.MyIndex;
        var bombPieceType = bombPiece.MyType;
        var bombPieceSubType = bombPiece.MySubType;
        var addTuple = (0, 0);
        var repeatTime = 0;
        var amount = 0;

        switch(bombPieceSubType)
        {
            case PieceSubType.Hbomb:
                addTuple.Item1 = 1;
                repeatTime = Mathf.Max(bombTuple.Item1, FieldInfo.Width - bombTuple.Item1);
                break;
            case PieceSubType.Vbomb:
                addTuple.Item2 = 1;
                repeatTime = Mathf.Max(bombTuple.Item1, FieldInfo.Height - bombTuple.Item1);
                break;
            case PieceSubType.CrossBomb:
                addTuple.Item2 = 1;
                var heightTime = Mathf.Max(bombTuple.Item1, FieldInfo.Height - bombTuple.Item1);
                var widthTime = Mathf.Max(bombTuple.Item1, FieldInfo.Width - bombTuple.Item1);
                if (widthTime < heightTime)
                {
                    repeatTime = heightTime;
                    break;
                }
                repeatTime = widthTime;

                break;
        }

        var increasedTuple = (bombTuple.Item1 + addTuple.Item1, bombTuple.Item2 + addTuple.Item2);
        var decreasedTuple = (bombTuple.Item1 - addTuple.Item1, bombTuple.Item2 - addTuple.Item2);
        var delayTime = popDelayValue / repeatTime;

        Debug.Log($"{increasedTuple} {decreasedTuple}");

        while (repeatTime-- > 0)
        {
            yield return new WaitForSeconds(delayTime);
            popStateChangeDelay = popDelayValue;
            StartCoroutine(TryRemovePiece(increasedTuple, bombPieceType));
            StartCoroutine(TryRemovePiece(decreasedTuple, bombPieceType));

            if(bombPieceSubType == PieceSubType.CrossBomb)
            {
                var testIncreasedPiece = (bombTuple.Item1 + addTuple.Item2, bombTuple.Item2);
                var testDecreasedPiece = (bombTuple.Item1 - addTuple.Item2, bombTuple.Item2);
                StartCoroutine(TryRemovePiece(testIncreasedPiece, bombPieceType));
                StartCoroutine(TryRemovePiece(testDecreasedPiece, bombPieceType));
            }

            addTuple.Item1 += addTuple.Item1 != 0 ? 1 : 0;
            addTuple.Item2 += addTuple.Item2 != 0 ? 1 : 0;

            increasedTuple = (bombTuple.Item1 + addTuple.Item1, bombTuple.Item2 + addTuple.Item2);
            decreasedTuple = (bombTuple.Item1 - addTuple.Item1, bombTuple.Item2 - addTuple.Item2);

            IEnumerator TryRemovePiece((int, int) testTuple, PieceType bombPieceType)
            {
                if (IsPlaceAreExist(testTuple) && !IsPlaceEmpty(testTuple))
                {
                    var targetPiece = MyPieceField[testTuple.Item1, testTuple.Item2];
                    var targetSubType = targetPiece.MySubType;

                    while(targetPiece.MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("ChangeType"))
                    {
                        yield return null;
                    }

                    if (targetPiece.MySubType == PieceSubType.Rainbow)
                    {
                        StartActiveRainbowBomb(targetPiece, bombPieceType, 4);
                    }
                    else if (targetSubType != PieceSubType.None)
                    {
                        StartActiveBomb(targetPiece, 3);
                    }
                    else
                    {
                        PopPiece(targetPiece);
                    }
                    amount++;
                }
            }
        }

        GameManager.Instance.AddScore(amount, scoreMultiply);
    }
    private IEnumerator ActiveRainbowBomb(PuzzlePiece bombPiece, PieceType targetType, int scoreMultiply)
    {
        sameTypePieceList.Clear();
        foreach (var piece in PieceList)
        {
            if (piece.MyType == targetType && piece.MyIndex != (-1, -1) && piece != bombPiece)
            {
                sameTypePieceList.Add(piece);
            }
        }
        sameTypePieceList = sameTypePieceList.OrderBy(piece => Vector3.Distance(bombPiece.transform.position, piece.transform.position)).ToList();
        PopPiece(bombPiece);
        var amount = sameTypePieceList.Count;
        var delay = 0.5f / sameTypePieceList.Count;
        while (sameTypePieceList.Count > 0)
        {
            if (sameTypePieceList[0] != null && sameTypePieceList[0].MyIndex != (-1, -1))
            {
                PopPiece(sameTypePieceList[0]);
            }
            sameTypePieceList.RemoveAt(0);
            popStateChangeDelay = popDelayValue;
            yield return new WaitForSeconds(delay);
        }

        GameManager.Instance.AddScore(amount, scoreMultiply);
    }


}