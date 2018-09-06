using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : Algorithms {

	public AStar()
	{
		name = "A*";
	}

	public override void StartAlgorithm(TDTile start, TDTile end, TGMap map)
	{
		//SimplePriorityQueue<TDTile> frontier = new SimplePriorityQueue<TDTile>();
		//frontier.Enqueue(start, 0);

		//Dictionary<TDTile, TDTile> cameFrom = new Dictionary<TDTile, TDTile>();
		//cameFrom.Add(start, null);

		//Dictionary<TDTile, float> costSoFar = new Dictionary<TDTile, float>();
		//costSoFar.Add(start, 0);

		//float priority = 0.0f;
		//while (frontier.Count > 0)
		//{
		//	TDTile currentTile = frontier.Dequeue();
		//	if (currentTile.GetTileType() == (int)TILE_TYPE.ENDPOINT)
		//		break;

		//	foreach (TDTile nextTile in currentTile.neighbours)
		//	{
		//		if (nextTile == null || nextTile.GetTileType() == (int)TILE_TYPE.WATER || nextTile.GetTileType() == (int)TILE_TYPE.WALL) continue;

		//		float newCost = costSoFar[currentTile] + map.GetCostByTileType(nextTile.GetTileType());

		//		if (!costSoFar.ContainsKey(nextTile) || newCost < costSoFar[nextTile])
		//		{
		//			costSoFar[nextTile] = newCost;
		//			priority = newCost;
		//			frontier.Enqueue(nextTile, priority);
		//			cameFrom.Add(nextTile, currentTile);
		//		}
		//	}

		//}

		//GeneratePath(end, cameFrom);
	}
}
