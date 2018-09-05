using System.Collections;
using System.Collections.Generic;

public abstract class Algorithms {

	protected string name;
	protected List<TDTile> path;

	public abstract void StartAlgorithm(TDTile start, TDTile end, TGMap map);

	public string GetName()
	{
		return name;
	}

	public List<TDTile> GetPath()
	{
		return path;
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
}
