using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmStep {

	public TDTile CurrentTile { get; set; }
	public List<TDTile> NeighbourTiles { get; set; }

	public AlgorithmStep(TDTile _currentTile)
	{
		CurrentTile = _currentTile;
		NeighbourTiles = new List<TDTile>();
	}
}
