using System.Collections;
using System.Collections.Generic;
using UnityEngine; // debugging

public abstract class Algorithms {

	protected string name;
	protected List<TDTile> path;
	protected List<TDTile> algoTiles = new List<TDTile>();
	protected List<AlgorithmStep> algoSteps = new List<AlgorithmStep>();

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

	public List<AlgorithmStep> GetAlgoSteps()
	{
		return algoSteps;
	}

	protected void GeneratePath(TDTile end, Dictionary<TDTile, TDTile> cameFrom)
	{
		if (cameFrom == null || cameFrom.Count == 0) return;

		TDTile currentTile = end;
		path = new List<TDTile>();

		while (currentTile.GetTileType() != (int)TILE_TYPE.STARTPOINT)
		{
			path.Add(currentTile);
			if (!cameFrom.TryGetValue(currentTile, out currentTile))
				return;
		}

		path.Reverse();
	}

	protected float Heuristic(TDTile a, TDTile b)
	{
		float heuristic = System.Math.Abs(a.GetX() - b.GetX()) + System.Math.Abs(a.GetY() - b.GetY());
		return heuristic;
	}

	protected float ComputeVectorCrossProduct(TDTile start, TDTile goal, TDTile current)
	{
		TGMap tgm = GameObject.Find("TileMap").GetComponent<TGMap>();
		if (tgm == null || !tgm.computeVectorCrossProduct) return 0.0f;

		float dx1 = current.GetX() - goal.GetX();
		float dy1 = current.GetY() - goal.GetY();
		float dx2 = start.GetX() - goal.GetX();
		float dy2 = start.GetY() - goal.GetY();
		return System.Math.Abs(dx1 * dy2 - dx2 * dy1)*0.001f;
	}
}
