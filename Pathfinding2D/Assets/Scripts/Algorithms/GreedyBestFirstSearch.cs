using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyBestFirstSearch : Algorithms {

	public GreedyBestFirstSearch()
	{
		name = "Greedy Best First Search";
	}

	public override void StartAlgorithm(GameObject start, GameObject end, MapScript map)
	{
		algoSteps.Clear();

		SimplePriorityQueue<GameObject> frontier = new SimplePriorityQueue<GameObject>();
		frontier.Enqueue(start, 0);

		Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();
		cameFrom.Add(start, null);

		float priority = 0.0f;
		while (frontier.Count > 0)
		{
			GameObject currentTile = frontier.Dequeue();
			Debug.Log("Dequeue Tile: " + currentTile.GetHashCode());
			if (TileHelper.GetTileType(currentTile) == (int)TILE_TYPE.ENDPOINT)
				break;

			AlgorithmStep algoStep = new AlgorithmStep(currentTile);
			algoSteps.Add(algoStep);

			foreach (GameObject nextTile in currentTile.GetComponent<TileScript>().neighbours)
			{
				if (nextTile == null || TileHelper.GetTileType(nextTile) == (int)TILE_TYPE.WATER || TileHelper.GetTileType(nextTile) == (int)TILE_TYPE.WALL) continue;

				// diagonal step check: if neighbour is diagonal but diagonal step not allowed --> we skip
				if (IsDiagonalNeighbour(currentTile, nextTile) && !IsDiagonalStepAllowed()) continue;

				algoTiles.Add(nextTile);

				if (!cameFrom.ContainsKey(nextTile))
				{
					priority = Heuristic(end, nextTile) + ComputeVectorCrossProduct(start, end, nextTile); // TODO: add priority field to TDTile? to debug and or display priority in game
					frontier.Enqueue(nextTile, priority);
					//Debug.Log("Enqueue Tile: " + nextTile.GetHashCode() + " - priority: " + priority);
					cameFrom.Add(nextTile, currentTile);
					algoStep.NeighbourTiles.Add(nextTile); // WRONG! WE NEED TO LOOK AT THE TILE WITH LOWEST HEURISTIC FIRST
				}
			}

		}

		GeneratePath(end, cameFrom);
	}
}