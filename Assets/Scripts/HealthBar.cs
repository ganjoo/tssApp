using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

	Vector3 localScale;
    public float healthAmount = 1;

	// Use this for initialization
	void Start () {
		localScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		localScale.x = healthAmount*2;
		transform.localScale = localScale;

	}
}
