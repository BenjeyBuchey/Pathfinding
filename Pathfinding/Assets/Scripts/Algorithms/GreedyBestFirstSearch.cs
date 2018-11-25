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

	public override void StartAlgorithm(TDTile start, TDTile end, TGMap map)
	{
		algoSteps.Clear();

		SimplePriorityQueue<TDTile> frontier = new SimplePriorityQueue<TDTile>();
		frontier.Enqueue(start, 0);

		Dictionary<TDTile, TDTile> cameFrom = new Dictionary<TDTile, TDTile>();
		cameFrom.Add(start, null);

		float priority = 0.0f;
		while (frontier.Count > 0)
		{
			TDTile currentTile = frontier.Dequeue();
			Debug.Log("Dequeue Tile: " + currentTile.GetHashCode());
			if (currentTile.GetTileType() == (int)TILE_TYPE.ENDPOINT)
				break;

			AlgorithmStep algoStep = new AlgorithmStep(currentTile);
			algoSteps.Add(algoStep);

			foreach (TDTile nextTile in currentTile.neighbours)
			{
				if (nextTile == null || nextTile.GetTileType() == (int)TILE_TYPE.WATER || nextTile.GetTileType() == (int)TILE_TYPE.WALL) continue;

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