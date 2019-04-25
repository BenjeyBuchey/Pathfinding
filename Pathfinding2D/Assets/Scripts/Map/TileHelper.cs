using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TileHelper
{
	public const string bfs = "Breadth First Search", dijkstra = "Dijkstra's", astar = "A*", gbfs = "Greedy Best First Search";

	public static int GetTileType(GameObject go)
	{
		return go.GetComponent<TileScript>().GetTileType();
	}

	public static void SetCoordinates(GameObject go, int x, int y)
	{
		go.GetComponent<TileScript>().X = x;
		go.GetComponent<TileScript>().Y = y;
	}

	public static int GetX(GameObject go)
	{
		return go.GetComponent<TileScript>().X;
	}

	public static int GetY(GameObject go)
	{
		return go.GetComponent<TileScript>().Y;
	}

	public static int GetOldTileType(GameObject go)
	{
		return go.GetComponent<TileScript>().GetOldTileType();
	}

	public static void SetTileText(GameObject go)
	{
		go.GetComponentInChildren<Text>().text = go.GetComponent<TileScript>().Steps.ToString();
	}

	public static void IncreaseSteps(GameObject go)
	{
		go.GetComponent<TileScript>().Steps++;
	}

	public static void SetSteps(GameObject go, float val)
	{
		go.GetComponent<TileScript>().Steps = val;
	}

	public static float GetSteps(GameObject go)
	{
		return go.GetComponent<TileScript>().Steps;
	}

	public static void ClearTile(GameObject go)
	{
		go.GetComponent<TileScript>().Steps = 0;
		go.GetComponentInChildren<Text>().text = string.Empty;
	}

	public static void ClearTileText(GameObject go)
	{
		go.GetComponentInChildren<Text>().text = string.Empty;
	}
}