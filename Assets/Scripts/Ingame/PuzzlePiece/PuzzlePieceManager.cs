using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
    public PuzzlePiece[,] PieceField;
    public List<PuzzlePiece> PieceList;
    public GameObject PieceContainer;

    public readonly Vector3Int PieceFieldSize = new Vector3Int(7, 7);
    public readonly int PieceSize = 36;

    private void Awake()
    {
        PieceField = new PuzzlePiece[PieceFieldSize.x, PieceFieldSize.y];

        PieceContainer = new GameObject("PuzzlePieceContainer");
        PieceContainer.transform.parent = transform;
    } 



    // Update is called once per frame
    private void Update()
    {
        
    }
}
