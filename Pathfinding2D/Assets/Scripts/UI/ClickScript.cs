using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickScript : MonoBehaviour, IPointerClickHandler
{
	public Vector3 from = Vector3.one;
	public Vector3 to = Vector3.one * 0.95f;

	// Start is called before the first frame update
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator Scaling()
	{
		GetComponent<RectTransform>().localScale = to;
		yield return new WaitForSeconds(Time.fixedDeltaTime * 3);
		GetComponent<RectTransform>().localScale = from;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		StartCoroutine(Scaling());
	}
}
