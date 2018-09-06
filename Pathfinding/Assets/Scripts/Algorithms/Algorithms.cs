using System.Collections;
using System.Collections.Generic;
using UnityEngine; // debugging

public abstract class Algorithms {

	protected string name;
	protected List<TDTile> path;
	protected List<TDTile> algoTiles = new List<TDTile>();

	public abstract void StartAlgorithm(TDTile start, TDTile end, TGMap map);

	public string GetName()
	{
		return name;
	}

	public List<TDTile> GetPath()
	{
		return path;
	}

	public List<TDTile> GetAlgoTiles()
	{
		return algoTiles;
	}

	protected void GeneratePath(TDTile end, Dictionary<TDTile, TDTile> cameFrom)
	{
		TDTile currentTile = end;
		path = new List<TDTile>();

		while (currentTile.GetTileType() != (int)TILE_TYPE.STARTPOINT)
		{
			path.Add(currentTile);
			//cameFrom.TryGetValue(currentTile, out currentTile);
			currentTile = cameFrom[currentTile];
		}

		path.Reverse();
	}

	protected float Heuristic(TDTile a, TDTile b)
	{
		float heuristic = System.Math.Abs(a.GetX() - b.GetX()) + System.Math.Abs(a.GetY() - b.GetY());
		//Debug.Log("HEURISTIC: " + heuristic);
		return heuristic;
	}
}
