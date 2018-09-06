using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TGMap : MonoBehaviour {

	// grid settings
	public float tileSize = 1.0f;
	public Vector3 gridOffset;
	public int gridSizeX = 10, gridSizeZ = 10;
	
	public Texture2D terrainTiles;
	public int tileResolution = 16;
	private Color[][] tilePixels;
	private TDMap map;
	private Texture2D texture;
	private bool isDragged = false;
	private Vector2 lastTileCoords;
	private int draggedTileType = -1;
	private string selectedAlgorithm = string.Empty;
	private const string bfs = "Breadth First Search", dijkstra = "Dijkstra's", astar = "A*", gbfs = "Greedy Best First Search";
	public float costGrass = 2.0f, costGround = 1.0f;

	// Use this for initialization
	void Start () {
		BuildMesh();
		BuildTilePixels();
		BuildTexture();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			//Debug.Log("MOUSEBUTTON DOWN FIRED!");
			int x = 0, z = 0;
			TDTile draggedTile = GetSelectedTile(ref x, ref z);
			if (draggedTile == null) return;

			if (draggedTile.GetTileType() == (int)TILE_TYPE.STARTPOINT || draggedTile.GetTileType() == (int)TILE_TYPE.ENDPOINT)
				isDragged = true;

			//ClearPath();

			draggedTileType = draggedTile.GetTileType();
			lastTileCoords = new Vector2(x, z);
		}

		if (Input.GetMouseButtonUp(0))
		{
			//Debug.Log("MOUSEBUTTON UP FIRED!");
			if (isDragged)
				RefreshAlgorithm();

			isDragged = false;
		}

		if(isDragged)
		{
			int x = 0, z = 0;
			TDTile selectedTile = GetSelectedTile(ref x, ref z);
			if (selectedTile == null || selectedTile.GetTileType() == (int)TILE_TYPE.WATER || selectedTile.GetTileType() == (int)TILE_TYPE.WALL || 
				selectedTile.GetTileType() == (int)TILE_TYPE.STARTPOINT || selectedTile.GetTileType() == (int)TILE_TYPE.ENDPOINT) return;

			int newTileType = draggedTileType;

			// set old tile
			Vector2 currentTileCoords = new Vector2(x, z);
			if (currentTileCoords != lastTileCoords)
			{
				ClearPath();

				SetLastTile(currentTileCoords);

				// set new tile
				SetTileTexture(selectedTile, newTileType, x, z);

				RefreshAlgorithm();
			}
		}
	}

	void BuildTilePixels()
	{
		int numTilesPerRow = terrainTiles.width / tileResolution;
		int numRows = terrainTiles.height / tileResolution;

		tilePixels = new Color[numTilesPerRow * numRows][];
		for (int y = 0; y < numRows; y++)
		{
			for (int x = 0; x < numTilesPerRow; x++)
			{
				tilePixels[y * numTilesPerRow + x] = terrainTiles.GetPixels(x*tileResolution,y*tileResolution,tileResolution,tileResolution);
			}
		}
	}

	void BuildTexture()
	{
		map = new TDMap(gridSizeX, gridSizeZ);

		int texWidth = gridSizeX * tileResolution;
		int texHeight = gridSizeZ * tileResolution;
		texture = new Texture2D(texWidth, texHeight);

		for(int y=0;y<gridSizeZ;y++)
		{
			for(int x=0;x<gridSizeX;x++)
			{
				TDTile tile = map.GetTile(x, y);
				if (tile == null) continue;

				texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, tilePixels[tile.GetTileType()]);
			}
		}

		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();

		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.sharedMaterials[0].mainTexture = texture;

		Debug.Log("Done Texture!");
	}
	
	void BuildMesh()
	{
		int numTiles = gridSizeX * gridSizeZ;
		int numTris = numTiles * 2;

		int vgridSizeX = gridSizeX + 1;
		int vgridSizeZ = gridSizeZ + 1;
		int numVerts = vgridSizeX * vgridSizeZ;

		// Generate the mesh data
		Vector3[] vertices = new Vector3[numVerts];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];

		int[] triangles = new int[numTris * 3];

		int x, z;
		for (z = 0; z < vgridSizeZ; z++)
		{
			for (x = 0; x < vgridSizeX; x++)
			{
				vertices[z * vgridSizeX + x] = new Vector3(x * tileSize, 0, -z * tileSize);
				normals[z * vgridSizeX + x] = Vector3.up;
				uv[z * vgridSizeX + x] = new Vector2((float)x / gridSizeX, 1f - (float)z / gridSizeZ);
			}
		}
		Debug.Log("Done Verts!");

		for (z = 0; z < gridSizeZ; z++)
		{
			for (x = 0; x < gridSizeX; x++)
			{
				int squareIndex = z * gridSizeX + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = z * vgridSizeX + x + 0;
				triangles[triOffset + 2] = z * vgridSizeX + x + vgridSizeX + 0;
				triangles[triOffset + 1] = z * vgridSizeX + x + vgridSizeX + 1;

				triangles[triOffset + 3] = z * vgridSizeX + x + 0;
				triangles[triOffset + 5] = z * vgridSizeX + x + vgridSizeX + 1;
				triangles[triOffset + 4] = z * vgridSizeX + x + 1;
			}
		}

		Debug.Log("Done Triangles!");

		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;

		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();

		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
		Debug.Log("Done Mesh!");
	}

	private TDTile GetSelectedTile(ref int x, ref int z)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		if (GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity))
		{
			x = Mathf.FloorToInt(hitInfo.point.x / tileSize);
			z = Mathf.FloorToInt(hitInfo.point.z / tileSize);
			z += gridSizeZ;

			TDTile selectedTile = map.GetTile(x, z);
			return selectedTile;
		}
		return null;
	}

	private void SetLastTile(Vector2 currentTileCoords)
	{
		int x = (int)lastTileCoords.x;
		int z = (int)lastTileCoords.y;
		TDTile lastTileObject = map.GetTile(x, z);
		if (lastTileObject == null) return;

		// set last tile to old type
		// if last tile is start or endpoint --> assign ground
		int newTileType = lastTileObject.GetOldTileType();
		if (newTileType == (int)TILE_TYPE.STARTPOINT || newTileType == (int)TILE_TYPE.ENDPOINT || newTileType == -1)
			newTileType = (int)TILE_TYPE.GROUND;

		// set last tile to current tile
		lastTileCoords = currentTileCoords;

		SetTileTexture(lastTileObject, newTileType, x, z);
	}

	private void SetTileTexture(TDTile tile, int tileType, int x, int z)
	{
		tile.SetOldTileType(tile.GetTileType());
		tile.SetTileType(tileType);

		if (tileType == (int)TILE_TYPE.STARTPOINT)
			map.SetStartPoint(tile);
		else if (tileType == (int)TILE_TYPE.ENDPOINT)
			map.SetEndPoint(tile);

		texture.SetPixels(x * tileResolution, z * tileResolution, tileResolution, tileResolution, tilePixels[tileType]);
		texture.Apply();
	}

	public void StartAlgorithm(string algorithm)
	{
		selectedAlgorithm = algorithm;
		ClearPath();
		RefreshAlgorithm();
	}

	private void RefreshAlgorithm()
	{
		switch (selectedAlgorithm)
		{
			case bfs:
				StartAlgorithmBFS();
				break;
			case dijkstra:
				StartAlgorithmDijkstra();
				break;
			case astar:
				StartAlgorithmAStar();
				break;
			case gbfs:
				StartAlgorithmGBFS();
				break;
			default:
				break;
		}
	}

	public void StartAlgorithmBFS()
	{
		BreadthFirstSearch algo = new BreadthFirstSearch();
		algo.StartAlgorithm(map.GetStartPoint(), map.GetEndPoint());

		DrawPath(algo.GetPath());
	}

	public void StartAlgorithmDijkstra()
	{
		Dijkstras algo = new Dijkstras();
		algo.StartAlgorithm(map.GetStartPoint(), map.GetEndPoint(), this);

		DrawPath(algo.GetPath());
	}

	public void StartAlgorithmAStar()
	{
		AStar algo = new AStar();
		algo.StartAlgorithm(map.GetStartPoint(), map.GetEndPoint(), this);

		DrawPath(algo.GetPath());
	}

	public void StartAlgorithmGBFS()
	{
		GreedyBestFirstSearch algo = new GreedyBestFirstSearch();
		algo.StartAlgorithm(map.GetStartPoint(), map.GetEndPoint(), this);

		DrawPath(algo.GetPath());
	}

	private void DrawPath(List<TDTile> path)
	{
		if (path == null) return;

		map.SetPath(path);

		foreach (TDTile tile in path)
		{
			if (tile.GetTileType() != (int)TILE_TYPE.ENDPOINT)
				SetTileTexture(tile, (int)TILE_TYPE.PATH, tile.GetX(), tile.GetY());
		}
	}

	private void ClearPath()
	{
		foreach (TDTile tile in map.GetPath())
		{
			if (tile.GetTileType() != (int)TILE_TYPE.ENDPOINT)
				SetTileTexture(tile, tile.GetOldTileType(), tile.GetX(), tile.GetY());
		}
	}

	public void ClearAlgorithm()
	{
		selectedAlgorithm = string.Empty;
		ClearPath();
	}

	public float GetCostByTileType(int type)
	{
		switch(type)
		{
			case (int)TILE_TYPE.GROUND:
				return costGround;
			case (int)TILE_TYPE.GRASS:
				return costGrass;
			default:
				return 1.0f;
		}
	}
}
