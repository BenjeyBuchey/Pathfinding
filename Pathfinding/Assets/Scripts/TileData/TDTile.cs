using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TILE_TYPE { GROUND = 0, WATER = 1, GRASS = 2, WALL = 3, STARTPOINT = 4, ENDPOINT = 5, PATH = 6, PATH_CURRENT = 7, PATH_NEXT = 8 }
public enum TILE_NEIGHBOUR { NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3, NORTHEAST = 4, SOUTHEAST = 5, SOUTHWEST = 6, NORTHWEST = 7}

public class TDTile {

	private int type;
	private int oldTile = -1;
	private TDTile cameFrom = null;
	public TDTile[] neighbours = new TDTile[4];
	private int _x, _y;

	public TDTile(int x, int y)
	{
		type = (int)TILE_TYPE.GROUND;
		_x = x;
		_y = y;
	}

	public TDTile(int type)
	{
		this.type = type;
	}

	public int GetTileType()
	{
		return this.type;
	}

	public void SetTileType(int val)
	{
		this.type = val;
	}

	public bool isStartPoint()
	{
		if (type == (int)TILE_TYPE.STARTPOINT)
			return true;

		return false;
	}

	public bool isEndPoint()
	{
		if (type == (int)TILE_TYPE.ENDPOINT)
			return true;

		return false;
	}

	public bool isValidStartOrEndPoint()
	{
		if (type == (int)TILE_TYPE.GROUND)
			return true;

		return false;
	}

	public int GetOldTileType()
	{
		return oldTile;
	}

	public void SetOldTileType(int val)
	{
		if(val != (int)TILE_TYPE.PATH)
			oldTile = val;
	}

	public void PrintNeighbours()
	{
		if(neighbours[(int)TILE_NEIGHBOUR.NORTH] != null)
		{
			Debug.Log("NEIGHBOUR NORTH HAS TYPE " + neighbours[(int)TILE_NEIGHBOUR.NORTH].GetTileType());
		}

		if (neighbours[(int)TILE_NEIGHBOUR.EAST] != null)
		{
			Debug.Log("NEIGHBOUR EAST HAS TYPE " + neighbours[(int)TILE_NEIGHBOUR.EAST].GetTileType());
		}

		if (neighbours[(int)TILE_NEIGHBOUR.SOUTH] != null)
		{
			Debug.Log("NEIGHBOUR SOUTH HAS TYPE " + neighbours[(int)TILE_NEIGHBOUR.SOUTH].GetTileType());
		}

		if (neighbours[(int)TILE_NEIGHBOUR.WEST] != null)
		{
			Debug.Log("NEIGHBOUR WEST HAS TYPE " + neighbours[(int)TILE_NEIGHBOUR.WEST].GetTileType());
		}

	}

	public void SetTileCameFrom(TDTile val)
	{
		cameFrom = val;
	}

	public TDTile GetTileCameFrom()
	{
		return cameFrom;
	}

	public int GetX()
	{
		return _x;
	}

	public int GetY()
	{
		return _y;
	}

	public void printTileType()
	{
		string strTileType = string.Empty;
		switch(this.type)
		{
			case (int)TILE_TYPE.GRASS:
				strTileType = "GRASS";
				break;
			case (int)TILE_TYPE.WALL:
				strTileType = "WALL";
				break;
			case (int)TILE_TYPE.WATER:
				strTileType = "WATER";
				break;
			case (int)TILE_TYPE.GROUND:
				strTileType = "GROUND";
				break;
			default:
				strTileType = "OTHER";
				break;
		}
		Debug.Log("TILE TYPE: " + strTileType);
	}
}
