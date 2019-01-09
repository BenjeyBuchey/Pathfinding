using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldScript : MonoBehaviour {

	private float initWidth, initHeight;
	public GameObject _map;
	private int mapSizeDivider = 1, border = 10;
	// Use this for initialization
	void Start () {
		initWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
		initHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
	}

	public void SpawnNewMap()
	{
		int newMapCount = GameObject.FindGameObjectsWithTag("Map").Length+1;
		Debug.Log("CURRENT #Maps: " + newMapCount);

		if (newMapCount > mapSizeDivider * mapSizeDivider) mapSizeDivider++;

		GameObject map = Instantiate(_map, gameObject.transform);
		ResizeAndSetPositions();
	}

	private void ResizeAndSetPositions()
	{
		// + in x axis, - in y axis
		GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");

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
}
