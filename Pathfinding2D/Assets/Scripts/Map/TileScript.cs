using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {

	private int type = (int)TILE_TYPE.GROUND;
	private int oldTile = -1;
	private GameObject cameFrom = null;
	public GameObject[] neighbours = new GameObject[8];
	private int _x, _y;
	private float _steps = 0.0f;

	public int X
	{
		get
		{
			return _x;
		}

		set
		{
			_x = value;
		}
	}

	public int Y
	{
		get
		{
			return _y;
		}

		set
		{
			_y = value;
		}
	}

	public float Steps
	{
		get
		{
			return _steps;
		}

		set
		{
			_steps = value;
		}
	}

	public void SetTileType(int val)
	{
		type = val;
	}

	public int GetTileType()
	{
		return type;
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

	public void SetTileCameFrom(GameObject val)
	{
		cameFrom = val;
	}

	public GameObject GetTileCameFrom()
	{
		return cameFrom;
	}

	public int GetOldTileType()
	{
		return oldTile;
	}

	public void SetOldTileType(int val)
	{
		oldTile = val;
	}
}
