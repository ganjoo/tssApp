using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyCollisionHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnJellyCollisionEnter(JellySprite.JellyCollision collision)
    {

        UnityJellySprite jellySprite = GetComponent<UnityJellySprite>();
        jellySprite.Scale(0.5f,true);
    }
}
