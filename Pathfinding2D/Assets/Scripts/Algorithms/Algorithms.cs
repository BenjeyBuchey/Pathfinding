using System.Collections;
using System.Collections.Generic;
using UnityEngine; // debugging

public abstract class Algorithms {

	protected string name;
	protected List<GameObject> path;
	protected List<GameObject> algoTiles = new List<GameObject>();
	protected List<AlgorithmStep> algoSteps = new List<AlgorithmStep>();

	public abstract void StartAlgorithm(GameObject start, GameObject end, MapScript map);

	public string GetName()
	{
		return name;
	}

	public List<GameObject> GetPath()
	{
		return path;
	}

	public List<GameObject> GetAlgoTiles()
	{
		return algoTiles;
	}

	public List<AlgorithmStep> GetAlgoSteps()
	{
		return algoSteps;
	}

	protected void GeneratePath(GameObject end, Dictionary<GameObject, GameObject> cameFrom)
	{
		if (cameFrom == null || cameFrom.Count == 0) return;

		GameObject currentTile = end;
		path = new List<GameObject>();

		while (TileHelper.GetTileType(currentTile) != (int)TILE_TYPE.STARTPOINT)
		{
			path.Add(currentTile);
			if (!cameFrom.TryGetValue(currentTile, out currentTile))
				return;
		}

		path.Reverse();
	}

	protected float Heuristic(GameObject a, GameObject b)
	{
		float heuristic = System.Math.Abs(TileHelper.GetX(a) - TileHelper.GetX(b)) + System.Math.Abs(TileHelper.GetY(a) - TileHelper.GetY(b));
		return heuristic;
	}

	protected float ComputeVectorCrossProduct(GameObject start, GameObject goal, GameObject current)
	{
		UIManager ui = GameObject.Find("UIManager").GetComponent<UIManager>();
		if (ui == null || !ui.vectorCrossProduct.isOn) return 0.0f;

		float dx1 = TileHelper.GetX(current) - TileHelper.GetX(goal);
		float dy1 = TileHelper.GetY(current) - TileHelper.GetY(goal);
		float dx2 = TileHelper.GetX(start) - TileHelper.GetX(goal);
		float dy2 = TileHelper.GetY(start) - TileHelper.GetY(goal);
		return System.Math.Abs(dx1 * dy2 - dx2 * dy1) * 0.001f;
	}

	protected bool IsDiagonalNeighbour(GameObject current, GameObject neighbour)
	{
		if (current == null || neighbour == null) return false;

		if (current.GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.NORTHEAST] == neighbour ||
			current.GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.NORTHWEST] == neighbour ||
			current.GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHEAST] == neighbour ||
			current.GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHWEST] == neighbour)
			return true;

		return false;
	}

	protected bool IsDiagonalStepAllowed()
	{
		//MapScript tgm = GameObject.Find("Map").GetComponent<MapScript>();
		UIManager ui = GameObject.Find("UIManager").GetComponent<UIManager>();
		if (ui == null || !ui.diagonalStep.isOn) return false;

		return true;
	}
}
