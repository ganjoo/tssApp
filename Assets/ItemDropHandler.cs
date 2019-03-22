using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour , IDropHandler{



    public void OnDrop(PointerEventData eventData)
    {
        RectTransform gameArea = transform as RectTransform;
        if (!RectTransformUtility.RectangleContainsScreenPoint(gameArea, Input.mousePosition))
        {

        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
