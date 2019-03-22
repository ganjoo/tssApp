using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  DrawDotGame;

public class CollisionDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
      
        //if(collision.collider.gameObject.name == "Collider")
        //{
        //    //Get Parent
        //    GameObject parentLine = collision.collider.gameObject.transform.parent.gameObject;
        //    //parentLine.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        //    //if((parentLine.transform.localScale.x < 0.1f) && (parentLine.transform.localScale.x < 0.1f))
        //    {
        //        //Stop Random Movement
        //        GameObject gobj = GameObject.Find("GameManager");
        //        gobj.GetComponent<GameManager>().randomMovement = false;
        //        Destroy(collision.collider.gameObject);

        //        Transform line_transform = parentLine.transform;
        //        Transform[] childTransform = new Transform[line_transform.childCount];
        //        int i = 0;
        //        Debug.Log(line_transform.childCount + " = Number of Children of This line");
        //        for (; i < line_transform.childCount; ++i)
        //        {
        //            //Debug.Log("Getting Child " + i + parentLine.transform.GetChild(i));
        //            childTransform[i] = line_transform.GetChild(i);
        //        }
        //        if (i == 0)
        //        {
        //            gobj.GetComponent<GameManager>().randomMovement = true;
        //            Destroy(parentLine);    
        //        }

        //        Transform next_prey = GetClosestPrey(childTransform);
                

        //        this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        //        this.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
        //        this.GetComponent<Rigidbody2D>().Sleep();
        //        this.GetComponent<Rigidbody2D>().AddForce(next_prey.position - transform.position);
        //        //transform.position = next_prey.position;

        //    }


        //}
    }
    Transform GetClosestPrey(Transform[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }


}