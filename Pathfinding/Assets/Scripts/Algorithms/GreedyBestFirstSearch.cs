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
		SimplePriorityQueue<TDTile> frontier = new SimplePriorityQueue<TDTile>();
		frontier.Enqueue(start, 0);

		Dictionary<TDTile, TDTile> cameFrom = new Dictionary<TDTile, TDTile>();
		cameFrom.Add(start, null);

		float priority = 0.0f;
		while (frontier.Count > 0)
		{
			TDTile currentTile = frontier.Dequeue();
			if (currentTile.GetTileType() == (int)TILE_TYPE.ENDPOINT)
				break;

			foreach (TDTile nextTile in currentTile.neighbours)
			{
				if (nextTile == null || nextTile.GetTileType() == (int)TILE_TYPE.WATER || nextTile.GetTileType() == (int)TILE_TYPE.WALL) continue;

				if (!cameFrom.ContainsKey(nextTile))
				{
					priority = Heuristic(end, nextTile);
					frontier.Enqueue(nextTile, priority);
					cameFrom.Add(nextTile, currentTile);
				}
			}

		}

		GeneratePath(end, cameFrom);
	}
}