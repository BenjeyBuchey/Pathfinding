using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : Algorithms {

	public AStar()
	{
		name = "A*";
	}

	public override void StartAlgorithm(GameObject start, GameObject end, MapScript map)
	{
		algoSteps.Clear();

		SimplePriorityQueue<GameObject> frontier = new SimplePriorityQueue<GameObject>();
		frontier.Enqueue(start, 0);

		Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();
		cameFrom.Add(start, null);

		Dictionary<GameObject, float> costSoFar = new Dictionary<GameObject, float>();
		costSoFar.Add(start, 0);

		float priority = 0.0f;
		while (frontier.Count > 0)
		{
			GameObject currentTile = frontier.Dequeue();
			if (TileHelper.GetTileType(currentTile) == (int)TILE_TYPE.ENDPOINT)
				break;

			AlgorithmStep algoStep = new AlgorithmStep(currentTile);
			algoSteps.Add(algoStep);

			foreach (GameObject nextTile in currentTile.GetComponent<TileScript>().neighbours)
			{
				if (nextTile == null || TileHelper.GetTileType(nextTile) == (int)TILE_TYPE.WATER || TileHelper.GetTileType(nextTile) == (int)TILE_TYPE.WALL) continue;

				// diagonal step check: if neighbour is diagonal but diagonal step not allowed --> we skip
				if (IsDiagonalNeighbour(currentTile, nextTile) && !IsDiagonalStepAllowed()) continue;

				float newCost = costSoFar[currentTile] + map.GetCostByTileType(TileHelper.GetTileType(nextTile));

				if (!costSoFar.ContainsKey(nextTile) || newCost < costSoFar[nextTile])
				{
					costSoFar[nextTile] = newCost;
					priority = newCost + Heuristic(end, nextTile) + ComputeVectorCrossProduct(start, end, nextTile);
					frontier.Enqueue(nextTile, priority);
					if (cameFrom.ContainsKey(nextTile)) // TODO: check if this is correct approach (without this we get multiple entries for keys)
						cameFrom.Remove(nextTile);
					cameFrom.Add(nextTile, currentTile);
					algoStep.NeighbourTiles.Add(nextTile);
				}
			}

		}

		GeneratePath(end, cameFrom);
	}
}