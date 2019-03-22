using UnityEngine;
using System.Collections;
using DrawDotGame;

public class GizmosController : MonoBehaviour
{

    private Vector3 screenPoint;
    private Vector3 offset;
    private Rect boundTo;
    bool buttonPressed = false;
    Vector3 init_position;
    public GameManager gmanager;

    public delegate void OnTargetMovedDelegate();
    public static OnTargetMovedDelegate targetMoved;

    private void Start()
    {
        init_position = transform.localPosition;

    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        if (gmanager)
        {
            if (gmanager.isPointInGameArea(curPosition))
            {
                transform.position = curPosition;
                if(targetMoved != null )
                    targetMoved();
                Debug.Log("Moved");

            }
        }
        else
        transform.position = curPosition;
       // transform.localPosition = init_position;

    }


}