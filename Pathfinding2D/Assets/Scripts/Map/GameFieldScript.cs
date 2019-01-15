using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldScript : MonoBehaviour {

	private float initWidth, initHeight;
	public GameObject _map;
	private int mapSizeDivider = 1, border = 10;
	private int maxMaps = 4;
	private int mapCount = 1;
	// Use this for initialization
	void Start () {
		initWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
		initHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
		SpawnNewMap();
	}

	public void SpawnNewMap()
	{
		GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");
		int newMapCount = maps.Length+1;
		if (newMapCount > maxMaps) return;

		if (newMapCount > mapSizeDivider * mapSizeDivider) mapSizeDivider++;

		GameObject map = Instantiate(_map, gameObject.transform);
		GameObject mapToCopy = maps.Length > 0 ? maps[0] : null;
		mapCount++;

		ResizeAndSetPositions();

		HandleNewMap(mapToCopy, map);
	}

	public void RemoveMap()
	{
		int newMapCount = GameObject.FindGameObjectsWithTag("Map").Length - 1;
		if (newMapCount < 1) return;

		if (newMapCount <= (mapSizeDivider-1) * (mapSizeDivider-1)) mapSizeDivider--;

		// remove last map
		GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");
		Destroy(maps[maps.Length - 1]);

		ResizeAndSetPositions();
		mapCount--;
	}

	private void ResizeAndSetPositions()
	{
		// + in x axis, - in y axis
		GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");
		if (maps == null || maps.Length == 0) return;

		int mapIndex = 0;
		// y = rows, x = columns
		for(int y = 0; y < mapSizeDivider; y++)
		{
			for(int x = 0; x < mapSizeDivider; x++)
			{
				GameObject map = maps[mapIndex];
				// size
				RectTransform rt = map.GetComponent<RectTransform>();
				rt.sizeDelta = new Vector2((initWidth - border * (mapSizeDivider-1))/(mapSizeDivider), (initWidth - border * (mapSizeDivider - 1)) / (mapSizeDivider));

				// position
				rt.localPosition = new Vector3(x * (rt.sizeDelta.x + border), -y * (rt.sizeDelta.x + border), 0.0f);

				mapIndex++;
				if (mapIndex >= maps.Length) break;
			}
			if (mapIndex >= maps.Length) break;
		}
	}

	public int GetMapCount()
	{
		return mapCount;
	}

	public void CopyMapAppearance(GameObject mapToCopy, GameObject newMap)
	{
		if (mapToCopy == null || newMap == null) return;

		GameObject[,] tilesToCopy = mapToCopy.GetComponent<MapScript>().GetTiles();
		newMap.GetComponent<MapScript>().CopyMapAppearance(tilesToCopy);
	}

	private void HandleNewMap(GameObject mapToCopy, GameObject newMap)
	{
		newMap.GetComponent<MapScript>().Init();
		GameObject[,] tilesToCopy = mapToCopy == null ? null : mapToCopy.GetComponent<MapScript>().GetTiles();
		newMap.GetComponent<MapScript>().SpawnTiles(tilesToCopy);
		if (mapToCopy == null)
			newMap.GetComponent<MapScript>().SetStartEndPoints();
	}
}
