﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	public Dropdown dropdown;
	List<string> algorithms = new List<string>() { "Breadth First Search", "Dijkstra's", "Greedy Best First Search", "A*" };
	public GameObject tileMap;
	//public Sprite buttonWall, buttonWallSelected, buttonGrass, buttonGrassSelected, buttonWater, buttonWaterSelected, buttonGround, buttonGroundSelected;
	private bool isButtonWallSelected = false, isButtonGrassSelected = false, isButtonGroundSelected = false, isButtonWaterSelected = false;
	private Sprite[] sprites;
	enum sprite_types { wall = 0, wall_selected, grass, grass_selected, ground, ground_selected, water, water_selected }

	private void Start()
	{
		InitTiles();
		PopulateList();
		SetTilesButtons();
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


		//Image img = GameObject.Find("ButtonWall").GetComponent<Image>();
		//img.sprite = buttonWall;

		//img = GameObject.Find("ButtonGrass").GetComponent<Image>();
		//img.sprite = buttonGrass;

		//img = GameObject.Find("ButtonGround").GetComponent<Image>();
		//img.sprite = buttonGround;

		//img = GameObject.Find("ButtonWater").GetComponent<Image>();
		//img.sprite = buttonWater;
	}

	public void StartAlgorithm()
	{
		TGMap tgmap = tileMap.GetComponent<TGMap>();
		tgmap.StartAlgorithm(algorithms[dropdown.value]);
	}

	public void Clear()
	{
		TGMap tgmap = tileMap.GetComponent<TGMap>();
		tgmap.ClearAlgorithm();
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

	//private void TileButtonClicked(string btn, Sprite s)
	//{
	//	Image img = GameObject.Find(btn).GetComponent<Image>();
	//	img.sprite = s;
	//}

	private void DeselectTileButtons()
	{
		//Image img = GameObject.Find("ButtonWall").GetComponent<Image>();
		//img.sprite = buttonWall;

		//img = GameObject.Find("ButtonGrass").GetComponent<Image>();
		//img.sprite = buttonGrass;

		//img = GameObject.Find("ButtonWater").GetComponent<Image>();
		//img.sprite = buttonWater;

		//img = GameObject.Find("ButtonGround").GetComponent<Image>();
		//img.sprite = buttonGround;

		SetTilesButtons();

		isButtonGrassSelected = false;
		isButtonGroundSelected = false;
		isButtonWallSelected = false;
		isButtonWaterSelected = false;
	}

	private void PopulateList()
	{
		dropdown.AddOptions(algorithms);
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
}
