using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMap {

	enum AREA_STYLE { WALL = 1, WATER = 2, GRASS = 3 }

	TDTile[,] _tiles;
	private int _width, _height;
	private List<TDArea> _mapAreas;
	private TDTile startPoint = null, endPoint = null;
	private List<TDTile> _path;

	int[,] mapData;

	public TDMap(int width, int height)
	{
		_width = width;
		_height = height;

		_tiles = new TDTile[_height, _width];
		_path = new List<TDTile>();
		InitTiles();
		//BuildArea(); we start with full ground map
		MakePoints();
	}

	private void MakePoints()
	{
		startPoint = MakeRandomPoint();
		endPoint = MakeRandomPoint();

		startPoint.SetTileType((int)TILE_TYPE.STARTPOINT);
		endPoint.SetTileType((int)TILE_TYPE.ENDPOINT);
	}

	private TDTile MakeRandomPoint()
	{
		TDTile tile = null;
		while (tile == null)
		{
			int x = Random.Range(0, _width);
			int y = Random.Range(0, _height);

			if (GetTile(x, y) == null || !GetTile(x, y).isValidStartOrEndPoint()) continue;
			tile = GetTile(x, y);
			break;
		}

		return tile;
	}

	private void InitTiles()
	{
		for (int y = 0; y < _height; y++)
		{
			for (int x = 0; x < _width; x++)
			{
				_tiles[y, x] = new TDTile(x,y);

				// set neighbors
				_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.SOUTH] = (y <= 0) ? null :  _tiles[y - 1, x];
				_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.WEST] = (x <= 0) ? null : _tiles[y, x - 1];
				_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.SOUTHWEST] = (x <= 0 || y <= 0) ? null : _tiles[y - 1, x - 1];
				// TODO OTHER DIAGONAL

				// set north neighbour of this south & east neighbour of this west
				if (_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.SOUTH] != null)
					_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.SOUTH].neighbours[(int)TILE_NEIGHBOUR.NORTH] = _tiles[y, x];

				if (_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.WEST] != null)
					_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.WEST].neighbours[(int)TILE_NEIGHBOUR.EAST] = _tiles[y, x];

				if (_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.SOUTHWEST] != null)
					_tiles[y, x].neighbours[(int)TILE_NEIGHBOUR.SOUTHWEST].neighbours[(int)TILE_NEIGHBOUR.NORTHEAST] = _tiles[y, x];
			}
		}
	}

	public TDTile[,] GetTiles()
	{
		return _tiles;
	}

	public TDTile GetTile(int x, int y)
	{
		if (x < 0 || x >= _width || y < 0 || y >= _height) return null;

		return _tiles[y,x];
	}

	private void BuildArea()
	{
		int numAreas = _width * _height / 50;
		_mapAreas = new List<TDArea>();

		int i = 1,counter = 0;
		while(i <= numAreas)
		{
			int areaSizeX = Random.Range(3, 5);
			int areaSizeY = Random.Range(3, 5);

			TDArea area = new TDArea(Random.Range(0, _width - areaSizeX), Random.Range(0, _height - areaSizeY), areaSizeX, areaSizeY);
			if (!AreaCollides(area))
			{
				_mapAreas.Add(area);
				i++;
				MakeArea(area,Random.Range(1,4));
			}

			counter++;
			if (counter == 500) break; // infinite loop safety
		}
	}

	private bool AreaCollides(TDArea a)
	{
		foreach(TDArea b in _mapAreas)
		{
			if (a.CollidesWith(b)) return true;
		}

		return false;
	}

	private void MakeWallArea(TDArea area)
	{
		for (int x = 0; x < area.width; x++)
		{
			for (int y = 0; y < area.height; y++)
			{
				//if (x == 0 || x == area.width - 1 || y == 0 || y == area.height - 1)
				//	_tiles[area.top+y, area.left+x].SetTileType((int)TILE_TYPE.WALL);
				//else
				//	_tiles[area.top+y, area.left+x].SetTileType((int)TILE_TYPE.GROUND);
				_tiles[area.top + y, area.left + x].SetTileType((int)TILE_TYPE.WALL);
			}
		}
	}

	private void MakeWaterArea(TDArea area)
	{
		for (int x = 0; x < area.width; x++)
		{
			for (int y = 0; y < area.height; y++)
			{
				_tiles[area.top + y, area.left + x].SetTileType((int)TILE_TYPE.WATER);
			}
		}
	}

	private void MakeGrassArea(TDArea area)
	{
		for (int x = 0; x < area.width; x++)
		{
			for (int y = 0; y < area.height; y++)
			{
				_tiles[area.top + y, area.left + x].SetTileType((int)TILE_TYPE.GRASS);
			}
		}
	}

	private void MakeArea(TDArea area, int random)
	{
		switch(random)
		{
			case (int)AREA_STYLE.WALL:
				MakeWallArea(area);
				break;
			case (int)AREA_STYLE.GRASS:
				MakeGrassArea(area);
				break;
			case (int)AREA_STYLE.WATER:
				MakeWaterArea(area);
				break;
		}
	}

	public TDTile GetStartPoint()
	{
		return startPoint;
	}

	public TDTile GetEndPoint()
	{
		return endPoint;
	}

	public void SetStartPoint(TDTile val)
	{
		startPoint = val;
	}

	public void SetEndPoint(TDTile val)
	{
		endPoint = val;
	}

	public void SetPath(List<TDTile> path)
	{
		_path = path;
	}

	public void ClearPath()
	{
		_path.Clear();
	}

	public List<TDTile> GetPath()
	{
		return _path;
	}

	public void SetTiles(TDTile[,] tiles)
	{
		if (tiles == null) return;
		_tiles = (TDTile[,])tiles.Clone();
	}

	public void ResetTiles()
	{
		// we want to clear only tiles with type path, path_past & path_current
		for (int y = 0; y < _height; y++)
		{
			for (int x = 0; x < _width; x++)
			{
				if (_tiles[y, x].GetTileType() != (int)TILE_TYPE.ENDPOINT && (_tiles[y, x].GetTileType() == (int)TILE_TYPE.PATH || _tiles[y, x].GetTileType() == (int)TILE_TYPE.PATH_CURRENT ||
					_tiles[y, x].GetTileType() == (int)TILE_TYPE.PATH_NEXT) && _tiles[y, x].GetOldTileType() >= 0)
					_tiles[y, x].SetTileType(_tiles[y, x].GetOldTileType());
			}
		}
		ClearPath();
	}
}
