using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzlePieceGenerator : MonoBehaviour
{
    public GameObject PuzzlePiecePrefab;
    public PuzzlePiece[,] puzzlePieces;

    private void Awake() 
    {
        
    }   

    private void Start()
    {
        #region PieceCreate
        {
            puzzlePieces = new PuzzlePiece[GameManager.PieceFieldSize.x, GameManager.PieceFieldSize.y];
            var fieldSize = GameManager.PieceFieldSize - new Vector3(1.0f, 1.0f, 0.0f);
            var pieceSize = GameManager.PieceSize + 4;
            var typeArray = Enum. GetValues(typeof(PieceType));

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
