using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialForce : MonoBehaviour {

    public float radius = 5.0F;
    public float power = 10.0F;
    public GameObject leftButton;
    public GameObject rightButton;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ApplyRightForce()
    {

        Vector3 explosionPos = rightButton.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log("Applying Force");
                rb.AddExplosionForce(power, explosionPos, radius, 4.0F);
            }

        }
    }
    public void ApplyLeftForce()
    {
        
        Vector3 explosionPos = leftButton.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log("Applying Force");
                rb.AddExplosionForce(power, explosionPos, radius, 4.0F);
            }
               
        }
    }
}
