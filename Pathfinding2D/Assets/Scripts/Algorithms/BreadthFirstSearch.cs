using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; //for debugging

public class BreadthFirstSearch : Algorithms
{
	public BreadthFirstSearch()
	{
		name = "BreadthFirstSearch";
	}

	public override void StartAlgorithm(GameObject start, GameObject end, MapScript map)
	{
		algoSteps.Clear();

		Queue<GameObject> frontier = new Queue<GameObject>();
		frontier.Enqueue(start);

		Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();
		cameFrom.Add(start, null);

		while (frontier.Count > 0)
		{
			GameObject currentTile = frontier.Dequeue();
			if (TileHelper.GetTileType(currentTile) == (int)TILE_TYPE.ENDPOINT)
				break;

			AlgorithmStep algoStep = new AlgorithmStep(currentTile);
			algoSteps.Add(algoStep);

			foreach (GameObject nextTile in currentTile.GetComponent<TileScript>().neighbours)
			{
				if (nextTile == null || TileHelper.GetTileType(nextTile) == (int)TILE_TYPE.WATER || TileHelper.GetTileType(nextTile) == (int)TILE_TYPE.WALL) continue; // || == START TYLE !?

				// diagonal step check: if neighbour is diagonal but diagonal step not allowed --> we skip
				if (IsDiagonalNeighbour(currentTile, nextTile) && !IsDiagonalStepAllowed()) continue;

				if (!cameFrom.ContainsKey(nextTile))
				{
					frontier.Enqueue(nextTile);
					cameFrom.Add(nextTile, currentTile);
					algoStep.NeighbourTiles.Add(nextTile);
					TileHelper.SetSteps(nextTile,TileHelper.GetSteps(currentTile)+ map.GetCostByTileType(TileHelper.GetTileType(nextTile))); // we need to increase by actual cost of this tile; was +1 previously
				}
			}

		}

		GeneratePath(end, cameFrom);
	}
}
