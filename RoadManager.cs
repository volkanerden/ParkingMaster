using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public List<RoadPiece> roadPieces;

    private void Start()
    {
        for (int i = 0; i < roadPieces.Count; i++)
        {
            roadPieces[i].roadID = i + 1;
            roadPieces[i].nextRoadID = i + 2;
        }
    }
}