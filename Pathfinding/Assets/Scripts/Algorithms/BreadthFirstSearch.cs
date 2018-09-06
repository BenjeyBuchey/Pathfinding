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

	public override void StartAlgorithm(TDTile start, TDTile end, TGMap map = null)
	{
		algoTiles.Clear();

		Queue<TDTile> frontier = new Queue<TDTile>();
		frontier.Enqueue(start);

		Dictionary<TDTile, TDTile> cameFrom = new Dictionary<TDTile, TDTile>();
		cameFrom.Add(start, null);

		while(frontier.Count > 0)
		{
			TDTile currentTile = frontier.Dequeue();
			if (currentTile.GetTileType() == (int)TILE_TYPE.ENDPOINT)
				break;

			foreach (TDTile nextTile in currentTile.neighbours)
			{
				if (nextTile == null || nextTile.GetTileType() == (int)TILE_TYPE.WATER || nextTile.GetTileType() == (int)TILE_TYPE.WALL) continue;

				algoTiles.Add(nextTile);
				if (!cameFrom.ContainsKey(nextTile))
				{
					frontier.Enqueue(nextTile);
					cameFrom.Add(nextTile, currentTile);
				}
			}

		}

		GeneratePath(end, cameFrom);
	}
}
