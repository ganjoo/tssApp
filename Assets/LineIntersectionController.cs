using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawDotGame;
public class LineIntersectionController : MonoBehaviour {


    public List<GameObject> listLines;
    public GameObject pinkBall;
    public GameObject blueBall;
    // Use this for initialization
    void Start () {
        listLines = GameObject.Find("GameManager").GetComponent<GameManager>().listLine;
        pinkBall = GameObject.Find("GameManager").GetComponent<GameManager>().pinkBall;
        blueBall = GameObject.Find("GameManager").GetComponent<GameManager>().blueBall;
    }
	
	// Update is called once per frame
	void Update () {
        listLines = GameObject.Find("GameManager").GetComponent<GameManager>().listLine;
        //1. Check if intersected with any balls 2. Make the object move along the path 3. Make noise on hitting each point on line 4. Count and kill when age comes
        for(int i = 0; i < listLines.Count; ++i)
        {
            GameObject currentLine = listLines[i];
        }

    }
}
