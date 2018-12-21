using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldScript : MonoBehaviour {

	private float initWidth, initHeight;
	// Use this for initialization
	void Start () {
		//initWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
		//initHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//float width = this.gameObject.GetComponent<RectTransform>().rect.width;

		//Debug.Log("INIT WIDTH: " + initWidth);
		//Debug.Log("Current WIDTH: " + width);
		
		//int newCols = 0, rows = 0;

		//int currentCols = (int)(initWidth / width);
		//Debug.Log("Current #Columns: " + currentCols);
		//int numMaps = GameObject.FindGameObjectsWithTag("Map").Length;
		///Debug.Log("NUmber of Maps: " + numMaps);

		//if (numMaps > currentCols * currentCols)
		//{
		//	newCols = currentCols + 1;
		//	Debug.Log("New #Columns: " + newCols);

		//	Vector2 newSize = new Vector2(width / newCols, width / newCols);
		//	this.gameObject.GetComponent<GridLayoutGroup>().cellSize = newSize;
		//}
	}
}
