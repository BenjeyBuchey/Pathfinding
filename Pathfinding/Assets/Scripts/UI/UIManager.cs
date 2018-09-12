using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	public Dropdown dropdown;
	List<string> algorithms = new List<string>() { "Breadth First Search", "Dijkstra's", "Greedy Best First Search", "A*" };
	public GameObject tileMap;
	public Sprite buttonWall, buttonWallSelected, buttonGrass, buttonGrassSelected, buttonWater, buttonWaterSelected, buttonGround, buttonGroundSelected;
	private bool isButtonWallSelected = false, isButtonGrassSelected = false, isButtonGroundSelected = false, isButtonWaterSelected = false;

	private void Start()
	{
		PopulateList();
		SetTilesButtons();
	}

	private void SetTilesButtons()
	{
		Image img = GameObject.Find("ButtonWall").GetComponent<Image>();
		img.sprite = buttonWall;

		img = GameObject.Find("ButtonGrass").GetComponent<Image>();
		img.sprite = buttonGrass;

		img = GameObject.Find("ButtonGround").GetComponent<Image>();
		img.sprite = buttonGround;

		img = GameObject.Find("ButtonWater").GetComponent<Image>();
		img.sprite = buttonWater;
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
			TileButtonClicked(s, buttonWallSelected);
		}
		else
			TileButtonClicked(s, buttonWall);

		isButtonWallSelected = !isButtonWallSelected;
	}

	public void GrassButtonClicked()
	{
		string s = "ButtonGrass";
		if (!isButtonGrassSelected)
		{
			DeselectTileButtons();
			TileButtonClicked(s, buttonGrassSelected);
		}
		else
			TileButtonClicked(s, buttonGrass);

		isButtonGrassSelected = !isButtonGrassSelected;
	}

	public void GroundButtonClicked()
	{
		string s = "ButtonGround";
		if (!isButtonGroundSelected)
		{
			DeselectTileButtons();
			TileButtonClicked(s, buttonGroundSelected);
		}
		else
			TileButtonClicked(s, buttonGround);

		isButtonGroundSelected = !isButtonGroundSelected;
	}

	public void WaterButtonClicked()
	{
		string s = "ButtonWater";
		if (!isButtonWaterSelected)
		{
			DeselectTileButtons();
			TileButtonClicked(s, buttonWaterSelected);
		}
		else
			TileButtonClicked(s, buttonWater);

		isButtonWaterSelected = !isButtonWaterSelected;
	}

	private void TileButtonClicked(string btn, Sprite s)
	{
		Image img = GameObject.Find(btn).GetComponent<Image>();
		img.sprite = s;
	}

	private void DeselectTileButtons()
	{
		Image img = GameObject.Find("ButtonWall").GetComponent<Image>();
		img.sprite = buttonWall;

		img = GameObject.Find("ButtonGrass").GetComponent<Image>();
		img.sprite = buttonGrass;

		img = GameObject.Find("ButtonWater").GetComponent<Image>();
		img.sprite = buttonWater;

		img = GameObject.Find("ButtonGround").GetComponent<Image>();
		img.sprite = buttonGround;

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
