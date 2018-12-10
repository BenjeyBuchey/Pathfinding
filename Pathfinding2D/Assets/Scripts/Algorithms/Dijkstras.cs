using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Dijkstras : Algorithms {

	public Dijkstras()
	{
		name = "Dijkstra's Algorithm";
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

				algoTiles.Add(nextTile);

				float newCost = costSoFar[currentTile] + map.GetCostByTileType(TileHelper.GetTileType(nextTile));

				if (!costSoFar.ContainsKey(nextTile) || newCost < costSoFar[nextTile])
				{
					costSoFar[nextTile] = newCost;
					priority = newCost;
					frontier.Enqueue(nextTile, priority);
					cameFrom.Add(nextTile, currentTile);
					algoStep.NeighbourTiles.Add(nextTile);
					TileHelper.SetSteps(nextTile, costSoFar[nextTile]);
				}
			}

		}

		GeneratePath(end, cameFrom);
	}
}
