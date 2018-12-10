using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmStep {

	public GameObject CurrentTile { get; set; }
	public List<GameObject> NeighbourTiles { get; set; }



	public AlgorithmStep(GameObject _currentTile)
	{
		CurrentTile = _currentTile;
		NeighbourTiles = new List<GameObject>();
	}
}
