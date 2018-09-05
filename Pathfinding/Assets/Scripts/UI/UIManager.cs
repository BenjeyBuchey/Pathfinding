using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	private string selectedAlgorithm = string.Empty;
	
	public Dropdown dropdown;
	List<string> algorithms = new List<string>() { "Breadth First Search", "Dijkstra's", "A*" };
	public GameObject tileMap;

	private void Start()
	{
		PopulateList();
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

	private void PopulateList()
	{
		dropdown.AddOptions(algorithms);
	}
}
