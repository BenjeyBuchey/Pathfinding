﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	List<string> algorithms = new List<string>() { TileHelper.bfs, TileHelper.dijkstra, TileHelper.gbfs, TileHelper.astar };
	//public Sprite buttonWall, buttonWallSelected, buttonGrass, buttonGrassSelected, buttonWater, buttonWaterSelected, buttonGround, buttonGroundSelected;
	private bool isButtonWallSelected = false, isButtonGrassSelected = false, isButtonGroundSelected = false, isButtonWaterSelected = false;
	private Sprite[] sprites;
	enum sprite_types { wall = 0, wall_selected, grass, grass_selected, ground, ground_selected, water, water_selected }
	public Toggle visualize, vectorCrossProduct, diagonalStep;
	public InputField costGrass, costGround; // visualizationDelay;
	public Slider visualizationDelay;
	public Dropdown dropdownMap1, dropdownMap2, dropdownMap3, dropdownMap4;
	private List<Dropdown> dropdowns;
    public InputField summary;

	private void Start()
	{
		InitTiles();
		PopulateList();
		SetTilesButtons();
	}

    void Update()
    {

    }

    public void UpdateSummary()
    {
        summary.text = string.Empty;
        GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");
        foreach(GameObject map in maps)
        {
            summary.text += map.GetComponent<MapScript>().Summary;
        }
    }

    private void InitTiles()
	{
		sprites = new Sprite[8];
		sprites[(int)sprite_types.wall] = Resources.Load<Sprite>("button_wall");
		sprites[(int)sprite_types.wall_selected] = Resources.Load<Sprite>("button_wall_selected");
		sprites[(int)sprite_types.grass] = Resources.Load<Sprite>("button_grass");
		sprites[(int)sprite_types.grass_selected] = Resources.Load<Sprite>("button_grass_selected");
		sprites[(int)sprite_types.ground] = Resources.Load<Sprite>("button_ground");
		sprites[(int)sprite_types.ground_selected] = Resources.Load<Sprite>("button_ground_selected");
		sprites[(int)sprite_types.water] = Resources.Load<Sprite>("button_water");
		sprites[(int)sprite_types.water_selected] = Resources.Load<Sprite>("button_water_selected");
	}

	private void SetTilesButtons()
	{
		GameObject.Find("ButtonWall").GetComponent<Image>().sprite = sprites[(int)sprite_types.wall];
		GameObject.Find("ButtonGrass").GetComponent<Image>().sprite = sprites[(int)sprite_types.grass];
		GameObject.Find("ButtonGround").GetComponent<Image>().sprite = sprites[(int)sprite_types.ground];
		GameObject.Find("ButtonWater").GetComponent<Image>().sprite = sprites[(int)sprite_types.water];
	}

	public void StartAlgorithm()
	{
		GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");
		//foreach (GameObject map in maps)
		//{
		//	MapScript ms = map.GetComponent<MapScript>();
		//	ms.StartAlgorithm(algorithms[dropdown.value]);
		//}

		for(int i = 0; i < maps.Length; i++)
		{
			MapScript ms = maps[i].GetComponent<MapScript>();
			ms.StartAlgorithm(algorithms[dropdowns[i].value]);
		}
	}

	public void Clear()
	{
		GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");
		foreach (GameObject map in maps)
		{
			MapScript ms = map.GetComponent<MapScript>();
			ms.ClearAlgorithm();
		}
	}

	public void WallButtonClicked()
	{
		string s = "ButtonWall";
		if (!isButtonWallSelected)
		{
			DeselectTileButtons();
			TileButtonClicked(s, (int)sprite_types.wall_selected);
		}
		else
			TileButtonClicked(s, (int)sprite_types.wall);

		isButtonWallSelected = !isButtonWallSelected;
	}

	public void GrassButtonClicked()
	{
		string s = "ButtonGrass";
		if (!isButtonGrassSelected)
		{
			DeselectTileButtons();
			TileButtonClicked(s, (int)sprite_types.grass_selected);
		}
		else
			TileButtonClicked(s, (int)sprite_types.grass);

		isButtonGrassSelected = !isButtonGrassSelected;
	}

	public void GroundButtonClicked()
	{
		string s = "ButtonGround";
		if (!isButtonGroundSelected)
		{
			DeselectTileButtons();
			TileButtonClicked(s, (int)sprite_types.ground_selected);
		}
		else
			TileButtonClicked(s, (int)sprite_types.ground);

		isButtonGroundSelected = !isButtonGroundSelected;
	}

	public void WaterButtonClicked()
	{
		string s = "ButtonWater";
		if (!isButtonWaterSelected)
		{
			DeselectTileButtons();
			TileButtonClicked(s, (int)sprite_types.water_selected);
		}
		else
			TileButtonClicked(s, (int)sprite_types.water);

		isButtonWaterSelected = !isButtonWaterSelected;
	}

	private void TileButtonClicked(string btn, int type)
	{
		GameObject.Find(btn).GetComponent<Image>().sprite = sprites[type];
	}

	private void DeselectTileButtons()
	{
		SetTilesButtons();

		isButtonGrassSelected = false;
		isButtonGroundSelected = false;
		isButtonWallSelected = false;
		isButtonWaterSelected = false;
	}

	private void PopulateList()
	{
		dropdowns = new List<Dropdown>();
		dropdowns.Add(dropdownMap1);
		dropdowns.Add(dropdownMap2);
		dropdowns.Add(dropdownMap3);
		dropdowns.Add(dropdownMap4);

		ShowElement(dropdownMap2.gameObject, false);
		ShowElement(dropdownMap3.gameObject, false);
		ShowElement(dropdownMap4.gameObject, false);

		foreach (Dropdown d in dropdowns)
		{
			d.AddOptions(algorithms);
		}
	}

	private void HideElement(GameObject go)
	{
		go.SetActive(false);
	}

	private void ShowElement(GameObject go, bool visible)
	{
		go.SetActive(visible);
	}

	public int GetSelectedButtonTile()
	{
		if (isButtonWaterSelected)
			return (int)TILE_TYPE.WATER;
		else if (isButtonWallSelected)
			return (int)TILE_TYPE.WALL;
		else if (isButtonGroundSelected)
			return (int)TILE_TYPE.GROUND;
		else if (isButtonGrassSelected)
			return (int)TILE_TYPE.GRASS;
		else
			return -1;
	}

	public void SpawnNewMap()
	{
		GameFieldScript gfs = GameObject.Find("GameField").GetComponent<GameFieldScript>();
		gfs.SpawnNewMap();
		ManageDropdownsAfterChange();
	}

	public void RemoveMap()
	{
		GameFieldScript gfs = GameObject.Find("GameField").GetComponent<GameFieldScript>();
		gfs.RemoveMap();
		ManageDropdownsAfterChange();
	}

	private void ManageDropdownsAfterChange()
	{
		GameFieldScript gfs = GameObject.Find("GameField").GetComponent<GameFieldScript>();
		int mapCount = gfs.GetMapCount();

		for(int i = 0; i < dropdowns.Count; i++)
		{
			bool visible = (i <= (mapCount - 1)) ? true : false;
			ShowElement(dropdowns[i].gameObject, visible);
		}
	}
}