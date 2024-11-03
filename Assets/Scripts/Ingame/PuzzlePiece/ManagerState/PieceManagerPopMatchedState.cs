using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    private List<PuzzlePiece> sameTypePieceList = new();
    private List<PuzzlePiece> popPieceList = new();
    private float stateChangeDelay;
    public class PieceManagerPopMatchedState : BaseFSM<PuzzlePieceManager>
    {
        private bool isRefill;
        private List<PuzzlePiece[]> matchablePiecesList;
        public override void StateEnter()
        {
            matchablePiecesList = new();
            self.stateChangeDelay = 0.1f;
            isRefill = false;

            if (self.selectedPuzzlePiece != null && self.selectedPuzzlePiece.MyType == PieceType.Rainbow)
            {
                self.StartActiveRainbowBomb(self.selectedPuzzlePiece, self.swapTargetPuzzlePiece.MyType);
            }
            else if (self.swapTargetPuzzlePiece != null && self.swapTargetPuzzlePiece.MyType == PieceType.Rainbow)
            {
                self.StartActiveRainbowBomb(self.swapTargetPuzzlePiece, self.selectedPuzzlePiece.MyType);
            }
            else
            {
                #region Normal Remove
                while (self.repositionedPieceQueue.Count > 0)
                {
                    var target = self.repositionedPieceQueue.Dequeue();
                    var matchables = self.GetMatchablePieces(target);
                    if (matchables.Length > 0)
                    {
                        matchablePiecesList.Add(matchables);
                    }
                }

                matchablePiecesList = matchablePiecesList.OrderBy(arr => arr.Length).ToList();
                PuzzlePiece specialPiece = null;

                foreach (var matchables in matchablePiecesList)
                {
                    specialPiece = null;
                    var matchLength = matchables.Length;
                    #region Create special piece
                    if (matchLength > 3 && matchables.Count(arr => arr.MyIndex == (-1, -1)) == 0)
                    {
                        // set the special piece
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

                        // set special piece type
                        if (matchLength >= 6)
                        {
                            specialPiece.TargetChangeType = PieceType.Rainbow;
                        }
                        else if (matchLength == 5)
                        {
                            if (matchables.Count(piece => piece.MyIndex.Item1 == matchables[0].MyIndex.Item1) == 5 ||
                                matchables.Count(piece => piece.MyIndex.Item2 == matchables[0].MyIndex.Item2) == 5)
                            {
                                specialPiece.TargetChangeType = PieceType.Rainbow;
                                Debug.Log("rainbow");
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
                    }
                    #endregion

                    // remove
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
                                piece.RemoveSelf();
                            }
                            isRefill = true;
                        }
                    }
                }
                #endregion
            }

        }

        public override void StateExit()
        {
            self.swapTargetPuzzlePiece = null;
            self.selectedPuzzlePiece = null;
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
            self.stateChangeDelay -= Time.deltaTime;
            if (self.stateChangeDelay < 0.0f)
            {
                if (isRefill)
                {
                    stateManager.ChangeState<PieceManagerRefillState>();
                    return;
                }
                stateManager.ChangeState<PieceManagerFindMatchableState>();
            }
        }
    }
    /// <summary>
    /// Using for in PieceManagerPopMatchableState only
    /// </summary>
    private void StartActiveBomb(PuzzlePiece target)
    {
        switch (target.MySubType)
        {
            case PieceSubType.Vbomb:
                StartCoroutine(ActiveVerticalBomb(target));
                break;
            case PieceSubType.Hbomb:
                StartCoroutine(ActiveHorizontalBomb(target));
                break;
            case PieceSubType.CrossBomb:
                StartCoroutine(ActiveVerticalBomb(target));
                StartCoroutine(ActiveHorizontalBomb(target));
                break;
        }
        target.RemoveSelf();
        stateChangeDelay = 0.16f;
    }

    private void StartActiveRainbowBomb(PuzzlePiece target, PieceType targetType)
    {
        StartCoroutine(ActiveRainbowBomb(target, targetType));
    }

    /// <summary>
    /// Using for in PieceManagerPopMatchableState only
    /// </summary>
    private IEnumerator ActiveHorizontalBomb(PuzzlePiece bombPiece)
    {
        var bombTuple = bombPiece.MyIndex;
        var bombPieceType = bombPiece.MyType;
        var bombPieceSubType = bombPiece.MySubType;

        var increasedTuple = (bombTuple.Item1 + 1, bombTuple.Item2);
        var decreasedTuple = (bombTuple.Item1 - 1, bombTuple.Item2);
        var repeatTime = bombTuple.Item1 > FieldInfo.Width / 2 ? bombTuple.Item1 : FieldInfo.Width - bombTuple.Item1;
        var delayTime = 0.15f / repeatTime;

        while (repeatTime-- > 0)
        {
            yield return new WaitForSeconds(delayTime);
            // check right
            if (IsPlaceAreExist(increasedTuple) && !IsPlaceEmpty(increasedTuple))
            {
                RemovePieceByBomb(increasedTuple, bombPieceType, bombPieceSubType);
                increasedTuple = (increasedTuple.Item1 + 1, increasedTuple.Item2);
            }

            // check left
            if (IsPlaceAreExist(decreasedTuple) && !IsPlaceEmpty(decreasedTuple))
            {
                RemovePieceByBomb(decreasedTuple, bombPieceType, bombPieceSubType);
                decreasedTuple = (decreasedTuple.Item1 - 1, decreasedTuple.Item2);
            }
        }
    }

    /// <summary>
    /// Using for in PieceManagerPopMatchableState only
    /// </summary>
    private IEnumerator ActiveVerticalBomb(PuzzlePiece bombPiece)
    {
        var bombTuple = bombPiece.MyIndex;
        var bombPieceType = bombPiece.MyType;
        var bombPieceSubType = bombPiece.MySubType;

        var increasedTuple = (bombTuple.Item1, bombTuple.Item2);
        var decreasedTuple = (bombTuple.Item1, bombTuple.Item2 - 1);
        var repeatTime = bombTuple.Item2 > FieldInfo.Height / 2 ? bombTuple.Item2 : FieldInfo.Height - bombTuple.Item2;
        var delayTime = 0.15f / repeatTime;

        while (repeatTime-- > 0)
        {
            yield return new WaitForSeconds(delayTime);
            // check down
            if (IsPlaceAreExist(decreasedTuple) && !IsPlaceEmpty(decreasedTuple))
            {
                RemovePieceByBomb(decreasedTuple, bombPieceType, bombPieceSubType);
                decreasedTuple = (decreasedTuple.Item1, decreasedTuple.Item2 - 1);
            }

            // check up
            if (IsPlaceAreExist(increasedTuple) && !IsPlaceEmpty(increasedTuple))
            {
                RemovePieceByBomb(increasedTuple, bombPieceType, bombPieceSubType);
                increasedTuple = (decreasedTuple.Item1, decreasedTuple.Item2 + 1);
            }

        }
    }

    private void RemovePieceByBomb((int, int) testTuple, PieceType bombPieceType, PieceSubType bombType)
    {
        var target = MyPieceField[testTuple.Item1, testTuple.Item2];
        var targetSubType = target.MySubType;
        if (targetSubType == PieceSubType.Hbomb || targetSubType == PieceSubType.Vbomb)
        {
            if (target.MyIndex != (-1, -1))
            {
                StartActiveBomb(target);
            }
        }
        else if (target.MyType == PieceType.Rainbow)
        {
            if (target.MyIndex != (-1, -1))
            {
                StartActiveRainbowBomb(target, bombPieceType);
            }
        }
        else
        {
            if(bombType == PieceSubType.Hbomb)
            {
                if (target.MyIndex.Item2 == testTuple.Item2)
                {
                    target.RemoveSelf();
                }
            }
            else
            {
                target.RemoveSelf();
            }
        }
    }

    private IEnumerator ActiveRainbowBomb(PuzzlePiece target, PieceType targetType)
    {
        sameTypePieceList.Clear();
        foreach (var piece in PieceList)
        {
            if (piece.MyType == targetType && piece.MyIndex != (-1, -1))
            {
                sameTypePieceList.Add(piece);
            }
        }
        sameTypePieceList = sameTypePieceList.OrderBy(piece => Vector3.Distance(target.transform.position, piece.transform.position)).ToList();
        target.RemoveSelf();
        var delay = 0.5f / sameTypePieceList.Count;
        while (sameTypePieceList.Count > 0)
        {
            if(sameTypePieceList[0] != null)
            {
                sameTypePieceList[0].RemoveSelf();
            }
            sameTypePieceList.RemoveAt(0);
            stateChangeDelay = 0.16f;
            yield return new WaitForSeconds(delay);
        }

        myStateController.ChangeState<PieceManagerRefillState>();
    }


}