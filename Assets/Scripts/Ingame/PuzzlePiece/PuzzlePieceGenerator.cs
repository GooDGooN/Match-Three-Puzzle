using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceGenerator : MonoBehaviour
{
    public GameObject PuzzlePiecePrefab;
    public List<PuzzlePiece> puzzlePieces = new();

    private void Awake()
    {
        
    }

    private void Start()
    {
        #region PieceCreate
        {
            var fieldSize = GameManager.PieceFieldSize - new Vector3(1.0f, 1.0f, 0.0f);
            var pieceSize = GameManager.PieceSize + 4;
            for (int ix = 0; ix <= fieldSize.x; ix++)
            {
                for (int iy = 0; iy <= fieldSize.y; iy++)
                {
                    var pos = new Vector3(ix, iy);
                    pos -= fieldSize / 2.0f;
                    pos *= pieceSize;

                    puzzlePieces.Add(Instantiate(PuzzlePiecePrefab, transform).GetComponent<PuzzlePiece>());
                    var target = puzzlePieces[puzzlePieces.Count - 1];
                    target.transform.localPosition = pos;
                    target.MyType = (PieceType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PieceType)).Length);
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
