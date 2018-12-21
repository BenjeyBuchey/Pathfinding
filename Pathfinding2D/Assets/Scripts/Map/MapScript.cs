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
	public int tilesX = 25, tilesY = 25, draggedTileType = -1;
	private GameObject[,] tiles;
	public Sprite[] sprites;
	private bool isRunning = false, isDragged = false, isPressed = false;   // , isTileMapRefreshed = false
	//public bool visualizeAlgorithms = true, computeVectorCrossProduct = true, allowDiagonalStep = false;
	//public float costGrass = 2.0f, costGround = 1.0f, visualizationDelay;
	private string selectedAlgorithm = string.Empty;
	private List<GameObject> _path = new List<GameObject>();
	private UIManager uiManager;

	// Raycasting
	GraphicRaycaster raycaster;

	// Use this for initialization
	void Start ()
	{
		SetNumberOfTiles();
		Init();
		SpawnTiles();
		SetStartEndPoints();
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

	void SpawnTiles()
	{
		for(int y = 0; y < tilesY; y++)
		{
			for(int x = 0; x < tilesX; x++)
			{
				GameObject go = Instantiate(tile, this.transform);
				tiles[y, x] = go;
				TileHelper.SetCoordinates(tiles[y, x], x, y);

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

	void SetNumberOfTiles()
	{
		AutoGridLayout agl = gameObject.GetComponent<AutoGridLayout>();
		agl.m_Column = tilesX;
	}

	private void Init()
	{
		tiles = new GameObject[tilesY, tilesX];
		uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

		//Fetch the Raycaster from the GameObject (the Canvas)
		raycaster = GetComponent<GraphicRaycaster>();
	}

	private void SetStartEndPoints()
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
		x = Random.Range(0, tilesX);
		y = Random.Range(0, tilesY);
	}

	private void SetTileType(GameObject go, int tileType)
	{
		if (tileType < 0) return;

		if(GetTileType(go) != (int)TILE_TYPE.PATH_NEXT && GetTileType(go) != (int)TILE_TYPE.PATH_CURRENT)
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

			//if (!isPressed)
			//	ClearMap();

			isPressed = true;

			if (GetTileType(tile) != (int)TILE_TYPE.STARTPOINT && GetTileType(tile) != (int)TILE_TYPE.ENDPOINT)
				SetTileType(tile, tileType);


			//SetTileTextureSelfMapCreation(tile, tileType, x, z);
			//RefreshMap();
		}

		if (Input.GetMouseButtonUp(0))
		{
			//if (isPressed)
			//	RefreshAlgorithm();

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

		//if (visualizeAlgorithms)
		//{
		//	// full visualization
		//	// we clear map but don't draw new visualization yet. we do that after mouse gets released
		//	ClearMap();

		//	// set last tile
		//	SetLastTile(selectedTile);

		//	// set new tile
		//	SetTileType(selectedTile, newTileType);

		//	// set current map
		//	//RefreshMap();
		//}
		//else
		//{
		//	// only path
		//	// clear map, draw new path
		//	ClearMap();

		//	// set last tile
		//	SetLastTile(selectedTile);

		//	// set new tile
		//	SetTileType(selectedTile, newTileType);

		//	// refresh map and draw new path
		//	//RefreshMap();
		//	//RefreshAlgorithm();
		//}
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

		//RefreshMap();

		selectedAlgorithm = algorithm;
		//isRunning = true;
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
		algo.StartAlgorithm(start, end);

		Visualize(algo.GetPath(), algo.GetAlgoSteps());
	}

	public void StartAlgorithmDijkstra()
	{
		Dijkstras algo = new Dijkstras();
		algo.StartAlgorithm(start, end, this);

		Visualize(algo.GetPath(), algo.GetAlgoSteps());
	}

	public void StartAlgorithmAStar()
	{
		AStar algo = new AStar();
		algo.StartAlgorithm(start, end, this);

		Visualize(algo.GetPath(), algo.GetAlgoSteps());
	}

	public void StartAlgorithmGBFS()
	{
		GreedyBestFirstSearch algo = new GreedyBestFirstSearch();
		algo.StartAlgorithm(start, end, this);

		Visualize(algo.GetPath(), algo.GetAlgoSteps());
	}

	private void Visualize(List<GameObject> path, List<AlgorithmStep> algoSteps)
	{
		isRunning = true;

		if (uiManager.visualize.isOn)    // visualizeAlgorithms
			VisualizeAlgorithms(algoSteps, path);
		else
			DrawPath(path);

		//isRunning = false;
	}

	private void VisualizeAlgorithms(List<AlgorithmStep> algoSteps, List<GameObject> path)
	{
		StartCoroutine(DoVisualizeAlgorithms(algoSteps, path));
	}

	IEnumerator DoVisualizeAlgorithms(List<AlgorithmStep> algoSteps, List<GameObject> path)
	{
		foreach (AlgorithmStep algoStep in algoSteps)
		{
			// TODO: PATH_CURRENT_START TILE
			// PATH_NEXT_START TILE


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
		}
		DrawPath(path);
	}

	private void DrawPath(List<GameObject> path)
	{
		if (path == null) return;

		_path = path; // ???

		foreach (GameObject tile in path)
		{
			if (TileHelper.GetTileType(tile) != (int)TILE_TYPE.ENDPOINT)
				SetTileType(tile, (int)TILE_TYPE.PATH);
		}

		isRunning = false;
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

		//if (isTileMapRefreshed)
		//{
			for(int y = 0; y < tilesY; y++)
			{
				for(int x = 0; x < tilesX; x++)
				{

				if (TileHelper.GetTileType(tiles[y, x]) == (int)TILE_TYPE.PATH
				|| TileHelper.GetTileType(tiles[y, x]) == (int)TILE_TYPE.PATH_CURRENT || TileHelper.GetTileType(tiles[y, x]) == (int)TILE_TYPE.PATH_NEXT)
				{
					SetTileType(tiles[y, x], TileHelper.GetOldTileType(tiles[y, x]));
					TileHelper.ClearTileText(tiles[y, x]);
				}
				}
			}
		//}
		_path.Clear();
		//isTileMapRefreshed = false;
	}

	//private void RefreshMap()
	//{
		//isTileMapRefreshed = true;
		//ClearMap();
		//oldTileMap = (TDTile[,])map.GetTiles().Clone();
		//Graphics.CopyTexture(texture, oldTexture);
	//}

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
		//string val = uiManager.visualizationDelay.value
		//float delay = 0.0f;
		//float.TryParse(val, out delay);

		return uiManager.visualizationDelay.value;
	}
}