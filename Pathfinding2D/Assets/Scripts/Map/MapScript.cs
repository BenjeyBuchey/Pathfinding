using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TILE_TYPE { GROUND = 0, WATER = 1, GRASS = 2, WALL = 3, STARTPOINT = 4, ENDPOINT = 5, PATH = 6, PATH_CURRENT = 7, PATH_NEXT = 8 }
public enum TILE_NEIGHBOUR { NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3, NORTHEAST = 4, SOUTHEAST = 5, SOUTHWEST = 6, NORTHWEST = 7 }

public class MapScript : MonoBehaviour {

	public GameObject tile;
	private GameObject start, end, lastTile;
	public int tilesX = 25, tilesY = 25, draggedTileType = -1, _visualizationCounter = 0;
	private GameObject[,] tiles;
	public Sprite[] sprites;
	private bool isRunning = false, isDragged = false, isPressed = false, _isBusy = false;
	private string selectedAlgorithm = string.Empty;
	private List<GameObject> _path = new List<GameObject>();
	private UIManager uiManager;
    private List<AlgorithmStep> _algoSteps = new List<AlgorithmStep>();

	// Raycasting
	GraphicRaycaster raycaster;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isRunning) return;

		UIManager ui = GameObject.Find("UIManager").GetComponent<UIManager>();
		int tileType = ui.GetSelectedButtonTile();

		if (tileType < 0)
			DragDropStartEndTiles();
		else
			DrawTiles(tileType);
	}

	public void SpawnTiles(GameObject[,] tilesToCopy)
	{
		for(int y = 0; y < tilesY; y++)
		{
			for(int x = 0; x < tilesX; x++)
			{
				GameObject go = Instantiate(tile, this.transform);
				tiles[y, x] = go;
				if (tiles[y, x] == null) Debug.Log("NULL: " + tiles[y, x]);
				TileHelper.SetCoordinates(tiles[y, x], x, y);

				// copy tiletype from map1
				if (tilesToCopy != null && tilesToCopy[y, x] != null)
				{
					CopyTile(tilesToCopy[y, x], tiles[y, x]);
				}

				// set neighbours
				tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTH] = (y <= 0) ? null : tiles[y - 1, x];
				tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.WEST] = (x <= 0) ? null : tiles[y, x - 1];
				tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHWEST] = (x <= 0 || y <= 0) ? null : tiles[y - 1, x - 1];
				tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHEAST] = (x >= tilesX - 1 || y <= 0) ? null : tiles[y - 1, x + 1];

				// set north neighbour of this south & east neighbour of this west
				if (tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTH] != null)
					tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTH].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.NORTH] = tiles[y, x];

				if (tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.WEST] != null)
					tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.WEST].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.EAST] = tiles[y, x];

				if (tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHWEST] != null)
					tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHWEST].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.NORTHEAST] = tiles[y, x];

				if (tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHEAST] != null)
					tiles[y, x].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.SOUTHEAST].GetComponent<TileScript>().neighbours[(int)TILE_NEIGHBOUR.NORTHWEST] = tiles[y, x];
			}
		}
	}

	private void CopyTile(GameObject tileToCopy, GameObject tile)
	{
        // TODO: if original map has path tiles then copying leads to ground tiles even if other types are below
		int tileType = tileToCopy.GetComponent<TileScript>().GetTileType();
		// whitelist tiles that shall be copied
		if (tileType != (int)TILE_TYPE.STARTPOINT && tileType != (int)TILE_TYPE.ENDPOINT && tileType != (int)TILE_TYPE.GROUND && tileType != (int)TILE_TYPE.GRASS &&
			tileType != (int)TILE_TYPE.WATER && tileType != (int)TILE_TYPE.WALL)
			tileType = tileToCopy.GetComponent<TileScript>().GetOldTileType();//return;

        tile.GetComponent<TileScript>().SetTileType(tileType);
		tile.GetComponent<Image>().sprite = sprites[tileType];

		if (tileType == (int)TILE_TYPE.STARTPOINT)
			start = tile;
		if (tileType == (int)TILE_TYPE.ENDPOINT)
			end = tile;
	}

	private void SetNumberOfTiles()
	{
		AutoGridLayout agl = gameObject.GetComponent<AutoGridLayout>();
		agl.m_Column = tilesX;
	}

	public void Init()
	{
		SetNumberOfTiles();

		tiles = new GameObject[tilesY, tilesX];
		uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

		//Fetch the Raycaster from the GameObject (the Canvas)
		raycaster = GetComponent<GraphicRaycaster>();
	}

	public void SetStartEndPoints()
	{
		// Start
		int xStart = 0, yStart = 0;
		GetRandomPoint(ref xStart, ref yStart);
		SetTileType(tiles[xStart, yStart], (int)TILE_TYPE.STARTPOINT);
		start = tiles[xStart, yStart];

		// End
		int xEnd = 0, yEnd = 0;
		do
		{
			GetRandomPoint(ref xEnd, ref yEnd);
		} while (xStart == yStart && xEnd == yEnd);

		SetTileType(tiles[xEnd, yEnd], (int)TILE_TYPE.ENDPOINT);
		end = tiles[xEnd, yEnd];
	}

	private void GetRandomPoint(ref int x, ref int y)
	{
		x = UnityEngine.Random.Range(0, tilesX);
		y = UnityEngine.Random.Range(0, tilesY);
	}

	private void SetTileType(GameObject go, int tileType)
	{
		if (tileType < 0) return;

		if(GetTileType(go) != (int)TILE_TYPE.PATH_NEXT && GetTileType(go) != (int)TILE_TYPE.PATH_CURRENT && GetTileType(go) != (int)TILE_TYPE.PATH) // .PATH is new
			go.GetComponent<TileScript>().SetOldTileType(GetTileType(go));

		if (tileType == (int)TILE_TYPE.STARTPOINT)
			start = go;
		else if (tileType == (int)TILE_TYPE.ENDPOINT)
			end = go;

		go.GetComponent<TileScript>().SetTileType(tileType);
		go.GetComponent<Image>().sprite = sprites[tileType];
	}

	private int GetTileType(GameObject go)
	{
		return go.GetComponent<TileScript>().GetTileType();
	}

	void DragDropStartEndTiles()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GameObject draggedTile = GetSelectedTile();
			if (draggedTile == null) return;

			if (GetTileType(draggedTile) != (int)TILE_TYPE.STARTPOINT && GetTileType(draggedTile) != (int)TILE_TYPE.ENDPOINT)
				return;

			isDragged = true;

			draggedTileType = GetTileType(draggedTile);
			lastTile = draggedTile;
			//lastTileCoords = new Vector2(x, z);
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (isDragged)
				RefreshAlgorithm();

			isDragged = false;
		}

		if (isDragged)
		{
			GameObject selectedTile = GetSelectedTile();
			if (selectedTile == null || GetTileType(selectedTile) == (int)TILE_TYPE.WATER || GetTileType(selectedTile) == (int)TILE_TYPE.WALL ||
				GetTileType(selectedTile) == (int)TILE_TYPE.STARTPOINT || GetTileType(selectedTile) == (int)TILE_TYPE.ENDPOINT) return;

			int newTileType = draggedTileType;

			// set old tile
			//Vector2 currentTileCoords = new Vector2(x, z);
			if (selectedTile != lastTile)
			{
				DragDropRefresh(selectedTile, newTileType);
			}
		}
	}

	void DrawTiles(int tileType)
	{
		if (Input.GetMouseButtonDown(0) || isPressed)   // Input.GetMouseButtonDown(0)
		{
			GameObject tile = GetSelectedTile();
			if (tile == null) return;

			isPressed = true;

			if (GetTileType(tile) != (int)TILE_TYPE.STARTPOINT && GetTileType(tile) != (int)TILE_TYPE.ENDPOINT)
				SetTileType(tile, tileType);
		}

		if (Input.GetMouseButtonUp(0))
		{
			isPressed = false;
		}
	}

	void DragDropRefresh(GameObject selectedTile, int newTileType)
	{
		// we clear map but don't draw new visualization yet. we do that after mouse gets released
		ClearMap();

		// set last tile
		SetLastTile(selectedTile);

		// set new tile
		SetTileType(selectedTile, newTileType);
	}

	private void SetLastTile(GameObject currentTile)
	{
		int tileType = lastTile.GetComponent<TileScript>().GetOldTileType();
		if (tileType == (int)TILE_TYPE.STARTPOINT || tileType == (int)TILE_TYPE.ENDPOINT || tileType == -1)
			tileType = (int)TILE_TYPE.GROUND;

		SetTileType(lastTile, tileType);
		lastTile = currentTile;
	}

	private GameObject GetSelectedTile()
	{
		//Set up the new Pointer Event
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		List<RaycastResult> results = new List<RaycastResult>();

		//Raycast using the Graphics Raycaster and mouse click position
		pointerData.position = Input.mousePosition;
		this.raycaster.Raycast(pointerData, results);

		//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
		foreach (RaycastResult result in results)
		{
			if (result.gameObject.tag.Equals("Tile"))
				return result.gameObject;
		}

		return null;
	}

	public float GetCostByTileType(int type)
	{
		switch (type)
		{
			case (int)TILE_TYPE.GROUND:
				return GetCostGround();
			case (int)TILE_TYPE.GRASS:
				return GetCostGrass();
			default:
				return 1.0f;
		}
	}

	public void StartAlgorithm(string algorithm)
	{
		if (isRunning) return;

		ClearAlgorithm();

		selectedAlgorithm = algorithm;

		RefreshAlgorithm();
	}

	private void RefreshAlgorithm()
	{
		switch (selectedAlgorithm)
		{
			case TileHelper.bfs:
				StartAlgorithmBFS();
				break;
			case TileHelper.dijkstra:
				StartAlgorithmDijkstra();
				break;
			case TileHelper.astar:
				StartAlgorithmAStar();
				break;
			case TileHelper.gbfs:
				StartAlgorithmGBFS();
				break;
			default:
				break;
		}
	}

	public void StartAlgorithmBFS()
	{
		BreadthFirstSearch algo = new BreadthFirstSearch();
		algo.StartAlgorithm(start, end, this);
        _algoSteps = algo.GetAlgoSteps();
		_path = algo.GetPath();

        Visualize();
	}

	public void StartAlgorithmDijkstra()
	{
		Dijkstras algo = new Dijkstras();
		algo.StartAlgorithm(start, end, this);
        _algoSteps = algo.GetAlgoSteps();
		_path = algo.GetPath();

		Visualize();
	}

	public void StartAlgorithmAStar()
	{
		AStar algo = new AStar();
		algo.StartAlgorithm(start, end, this);
        _algoSteps = algo.GetAlgoSteps();
		_path = algo.GetPath();

		Visualize();
	}

	public void StartAlgorithmGBFS()
	{
		GreedyBestFirstSearch algo = new GreedyBestFirstSearch();
		algo.StartAlgorithm(start, end, this);
        _algoSteps = algo.GetAlgoSteps();
		_path = algo.GetPath();

		Visualize();
	}

	private void Visualize()
	{
		isRunning = true;

		if (uiManager.visualize.isOn)    // visualizeAlgorithms
			VisualizeAlgorithms();
		else
			DrawPath();
	}

	private void VisualizeAlgorithms()
	{
		StartCoroutine(DoVisualizeAlgorithms());
	}

	public void ResumeVisualization()
	{
		if (_algoSteps == null || _algoSteps.Count == 0) return;
		if (!uiManager.visualize.isOn) return;

		if (!isRunning)
		{
			isRunning = true;
			if (_visualizationCounter < 0)
				_visualizationCounter = 0;
			StartCoroutine(DoVisualizeAlgorithms());
		}
	}

	public void StepForward()
	{
		if (_isBusy || _algoSteps == null || _algoSteps.Count == 0) return;
		if (!uiManager.visualize.isOn) return;

		if (_visualizationCounter+1 < _algoSteps.Count)
			StartCoroutine(DoStepForward());
	}

	public void StepBackwards()
	{
		if (_isBusy || _algoSteps == null || _algoSteps.Count == 0) return;
		if (!uiManager.visualize.isOn) return;

		if (_visualizationCounter < _algoSteps.Count && _visualizationCounter>=0)
			StartCoroutine(DoStepBackwards());
	}

	public void StepEnd()
	{
		if (_isBusy || _algoSteps == null || _algoSteps.Count == 0) return;
		if (!uiManager.visualize.isOn) return;

		if (_visualizationCounter + 1 < _algoSteps.Count)
			StartCoroutine(DoStepEnd());
	}

	public void StepBegin()
	{
		if (_isBusy || _algoSteps == null || _algoSteps.Count == 0) return;
		if (!uiManager.visualize.isOn) return;

		if (_visualizationCounter < _algoSteps.Count && _visualizationCounter >= 0)
			StartCoroutine(DoStepBegin());
	}

	IEnumerator DoVisualizeAlgorithms()
	{
        for(;_visualizationCounter < _algoSteps.Count; _visualizationCounter++)
		//foreach (AlgorithmStep algoStep in algoSteps)
		{
			// MOVE IT DOWN TO END OF FOR LOOP ? 
            //while (IsPaused() || _isBusy) // completely break here? when resume is pressed start this coroutine ??
            //    yield return null;

            AlgorithmStep algoStep = _algoSteps[_visualizationCounter];
            if (algoStep == null) continue;
            // set current tile
            if (algoStep.CurrentTile != start && algoStep.CurrentTile != end)
				SetTileType(algoStep.CurrentTile, (int)TILE_TYPE.PATH_CURRENT);

			foreach (GameObject next in algoStep.NeighbourTiles)
			{
				if (next != start && next != end)
				{
					SetTileType(next, (int)TILE_TYPE.PATH_NEXT);
					TileHelper.SetTileText(next);
				}
			}
			// wait
			yield return new WaitForSeconds(GetVisualizationDelay());

			while (IsPaused() || _isBusy)
				yield return null;
		}
		_visualizationCounter--;

		DrawPath();
        //yield break; // CAN USE THIS TO GO TO END
	}

	IEnumerator DoStepForward()
	{
		_isBusy = true;
		// go to the next step
		_visualizationCounter++;

		AlgorithmStep algoStep = _algoSteps[_visualizationCounter];
		if (algoStep == null) yield break;	// draw path then break!?

		// set current tile
		if (algoStep.CurrentTile != start && algoStep.CurrentTile != end)
			SetTileType(algoStep.CurrentTile, (int)TILE_TYPE.PATH_CURRENT);

		foreach (GameObject next in algoStep.NeighbourTiles)
		{
			if (next != start && next != end)
			{
				SetTileType(next, (int)TILE_TYPE.PATH_NEXT);
				TileHelper.SetTileText(next);
			}
		}
		// wait
		yield return new WaitForSeconds(GetVisualizationDelay());

		// after last item draw path
		if (_visualizationCounter == _algoSteps.Count - 1)
			DrawPath();

		_isBusy = false;
	}

	IEnumerator DoStepEnd()
	{
		_isBusy = true;
		_visualizationCounter++;
		for (; _visualizationCounter < _algoSteps.Count; _visualizationCounter++)
		{
			AlgorithmStep algoStep = _algoSteps[_visualizationCounter];
			if (algoStep == null)
			{
				continue;
			}
			// set current tile
			if (algoStep.CurrentTile != start && algoStep.CurrentTile != end)
				SetTileType(algoStep.CurrentTile, (int)TILE_TYPE.PATH_CURRENT);

			foreach (GameObject next in algoStep.NeighbourTiles)
			{
				if (next != start && next != end)
				{
					SetTileType(next, (int)TILE_TYPE.PATH_NEXT);
					TileHelper.SetTileText(next);
				}
			}
		}
		DrawPath();
		_visualizationCounter = _algoSteps.Count - 1;
		_isBusy = false;
		yield break;
	}

	IEnumerator DoStepBackwards()
    {
		// set current tile to PATH_NEXT, PATH NEXT to old tile type
		// set neighbourtiles to old tile type!? and remove text
		_isBusy = true;
		
		AlgorithmStep algoStep = _algoSteps[_visualizationCounter];
		if (algoStep == null)
		{
			_isBusy = false;
			yield break;
		}

		// if this is the last step we have to remove that path
		if(_visualizationCounter == _algoSteps.Count-1)
			RevertPath();

		// need to set current tile from PATH_CURRENT to PATH_NEXT
		if (algoStep.CurrentTile != start && algoStep.CurrentTile != end)
			SetTileType(algoStep.CurrentTile, (int)TILE_TYPE.PATH_NEXT);

		foreach (GameObject next in algoStep.NeighbourTiles)
		{
			if (next != start && next != end)
			{
				// need to set old tile type and remove tile text
				SetTileType(next, next.GetComponent<TileScript>().GetOldTileType());
				TileHelper.ClearTileText(next);
			}
		}
		// wait
		yield return new WaitForSeconds(GetVisualizationDelay());

		_visualizationCounter--;
		_isBusy = false;
	}

	IEnumerator DoStepBegin()
	{
		_isBusy = true;

		for (; _visualizationCounter >= 0; _visualizationCounter--)
		{
			AlgorithmStep algoStep = _algoSteps[_visualizationCounter];
			if (algoStep == null)
			{
				continue;
			}

			// if this is the last step we have to remove that path
			if (_visualizationCounter == _algoSteps.Count - 1)
				RevertPath();

			// need to set current tile from PATH_CURRENT to PATH_NEXT
			if (algoStep.CurrentTile != start && algoStep.CurrentTile != end)
				SetTileType(algoStep.CurrentTile, (int)TILE_TYPE.PATH_NEXT);

			foreach (GameObject next in algoStep.NeighbourTiles)
			{
				if (next != start && next != end)
				{
					// need to set old tile type and remove tile text
					SetTileType(next, next.GetComponent<TileScript>().GetOldTileType());
					TileHelper.ClearTileText(next);
				}
			}
		}
		_isBusy = false;
		yield break;
	}

	private void DrawPath()
	{
		if (_path == null) return;

		foreach (GameObject tile in _path)
		{
			if (TileHelper.GetTileType(tile) != (int)TILE_TYPE.ENDPOINT)
				SetTileType(tile, (int)TILE_TYPE.PATH);
		}
        AddSummary();

		isRunning = false;
	}

	private void RevertPath()
	{
		// set path to path_current
		if (_path == null) return;

		foreach (GameObject tile in _path)
		{
			if (TileHelper.GetTileType(tile) != (int)TILE_TYPE.ENDPOINT)
				SetTileType(tile, (int)TILE_TYPE.PATH_CURRENT);
		}
	}

    private void AddSummary()
    {
        if (uiManager == null) return;

        uiManager.GetComponent<UIManager>().summary.text += selectedAlgorithm + ": " + _path[_path.Count - 1].GetComponent<TileScript>().Steps + System.Environment.NewLine;
    }

    private void ClearSummary()
    {
        if (uiManager == null) return;

        uiManager.GetComponent<UIManager>().summary.text = String.Empty;
    }

	public void ClearAlgorithm()
	{
		if (isRunning) return;

		selectedAlgorithm = string.Empty;
		ClearMap();
	}

	private void ClearMap()
	{
		// reset all tiles that PATH PATH_NEXT PATH_CURRENT
			for(int y = 0; y < tilesY; y++)
			{
				for(int x = 0; x < tilesX; x++)
				{
					if (TileHelper.GetTileType(tiles[y, x]) == (int)TILE_TYPE.PATH
					|| TileHelper.GetTileType(tiles[y, x]) == (int)TILE_TYPE.PATH_CURRENT || TileHelper.GetTileType(tiles[y, x]) == (int)TILE_TYPE.PATH_NEXT)
					{
						SetTileType(tiles[y, x], TileHelper.GetOldTileType(tiles[y, x]));
						TileHelper.ClearTile(tiles[y, x]);
					}
				}
			}
		_path.Clear();
        _algoSteps.Clear();
        _visualizationCounter = 0;
        ClearSummary();
	}

	private float GetCostGround()
	{
		string val = uiManager.costGround.text;
		float cost = 1.0f;
		float.TryParse(val, out cost);

		return cost;
	}

	private float GetCostGrass()
	{
		string val = uiManager.costGrass.text;
		float cost = 2.0f;
		float.TryParse(val, out cost);

		return cost;
	}

	private float GetVisualizationDelay()
	{
		return uiManager.visualizationDelay.value;
	}

	public GameObject[,] GetTiles()
	{
		return tiles;
	}

	public void CopyMapAppearance(GameObject[,] tilesToCopy)
	{
		if (tilesToCopy == null) return;
		Debug.Log("Trying to copy map1");
		for (int y = 0; y < tilesY; y++)
		{
			for (int x = 0; x < tilesX; x++)
			{
				int tileType = tilesToCopy[y, x].GetComponent<TileScript>().GetTileType();
				int oldTileType = tiles[y, x].GetComponent<TileScript>().GetTileType();
				if (tileType != oldTileType)
					Debug.Log("OLD: " + oldTileType + " - NEW: " + tileType);

				tiles[y, x].GetComponent<TileScript>().SetTileType(tileType);
				tiles[y, x].GetComponent<Image>().sprite = sprites[tileType];
			}
		}
	}

    private bool IsPaused()
    {
        GameObject uiControls = GameObject.Find("UIControls");
        if (uiControls == null) return false;

        return uiControls.GetComponent<UIControls>().IsPaused;
    }
}