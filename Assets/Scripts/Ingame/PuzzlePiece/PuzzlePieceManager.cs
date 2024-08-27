using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
    public PuzzlePiece[,] PieceField;
    public List<PuzzlePiece> PieceList;
    public GameObject PieceContainer;

    private void Awake()
    {
        PieceField = new PuzzlePiece[GameManager.PieceFieldSize.x, GameManager.PieceFieldSize.y];

        PieceContainer = new GameObject("PuzzlePieceContainer");
        PieceContainer.transform.parent = transform;
    } 



    // Update is called once per frame
    private void Update()
    {
        
    }
}
