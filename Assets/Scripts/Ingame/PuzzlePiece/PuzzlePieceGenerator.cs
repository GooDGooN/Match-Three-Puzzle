
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzlePieceGenerator : MonoBehaviour
{
    public GameObject PuzzlePiecePrefab;
    public PuzzlePiece[,] puzzlePieces;
    private Vector2Int matchablePieceAmount = new Vector2Int(3, 5);

    private void Awake() 
    {
        
    }   

    private void Start()
    {
        #region Initialize
        var fieldWidth = GameManager.PieceFieldSize.x;
        var fieldHeight = GameManager.PieceFieldSize.y;
        puzzlePieces = new PuzzlePiece[fieldWidth, fieldHeight];
        #endregion

        #region Minimum Matchable Piece Create
        {
            List<int[,]> matchablePointList = new();
            var count = Random.Range(matchablePieceAmount.x, matchablePieceAmount.y + 1);
            for (int i = 0; i < count; i++)
            {
                var ix = Utility.PickRandom(0, fieldWidth);
                var iy = Utility.PickRandom(0, fieldHeight);
                matchablePointList.Add(new int[ix, iy]);
            }

            while(matchablePointList.Count > 0)
            {
                var randomIndex = Random.Range(0, matchablePointList.Count);

                //check
            }
        }
        #endregion
        #region Piece Create All
        {
            var fieldSize = GameManager.PieceFieldSize - new Vector3(1.0f, 1.0f, 0.0f);
            var pieceSize = GameManager.PieceSize + 4;

            for (int iy = 0; iy <= fieldSize.y; iy++)
            {
                for (int ix = 0; ix <= fieldSize.x; ix++)
                {
                    var excludePiece = new PieceType[2];
                    var pos = new Vector3(ix, iy);
                    pos -= fieldSize / 2.0f;
                    pos *= pieceSize;

                    if (ix >= 2)
                    {
                        if(puzzlePieces[ix - 2, iy].MyType == puzzlePieces[ix - 1, iy].MyType)
                        {
                            excludePiece[0] = puzzlePieces[ix - 1, iy].MyType;
                        }
                    }
                    if (iy >= 2)
                    {
                        if (puzzlePieces[ix, iy - 2].MyType == puzzlePieces[ix, iy - 1].MyType)
                        {
                            excludePiece[1] = puzzlePieces[ix, iy - 1].MyType;
                        }
                    }

                    puzzlePieces[ix, iy] = Instantiate(PuzzlePiecePrefab, transform).GetComponent<PuzzlePiece>();
                    var target = puzzlePieces[ix, iy];
                    target.transform.localPosition = pos;

                    target.MyType = Utility.PickRandom(Utility.GetEnumArray<PieceType>(), excludePiece);
                }
            }
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
