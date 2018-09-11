using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	public Dropdown dropdown;
	List<string> algorithms = new List<string>() { "Breadth First Search", "Dijkstra's", "Greedy Best First Search", "A*" };
	public GameObject tileMap;
	public Sprite buttonWall, buttonWallSelected;
	private bool isButtonWallSelected = false;

	private void Start()
	{
		PopulateList();
		SetTilesButtons();
	}

	private void SetTilesButtons()
	{
		Image imgWall = GameObject.Find("ButtonWall").GetComponent<Image>();
		imgWall.sprite = buttonWall;
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
		Image imgWall = GameObject.Find("ButtonWall").GetComponent<Image>();

		if (!isButtonWallSelected)
		{
			isButtonWallSelected = true;
			imgWall.sprite = buttonWallSelected;
		}
		else
		{
			isButtonWallSelected = false;
			imgWall.sprite = buttonWall;
		}
	}

	private void PopulateList()
	{
		dropdown.AddOptions(algorithms);
	}
}
