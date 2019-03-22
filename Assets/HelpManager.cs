using DrawDotGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpManager : MonoBehaviour
{


    public GameObject[] hints;
    public GameObject arrow;

    IEnumerator timer(float time)
    {
        yield return new WaitForSeconds(time);
        if (current_hint == 2)
            displayNextHint();
    }
    public void OnTargetMoved()
    {
        StartCoroutine(timer(5f));
    }

    int current_hint = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.LevelLoaded >= 2)
        {
            SceneManager.LoadScene("index");
        }
        else
        {
            hints[current_hint].SetActive(true);
            arrow.SetActive(false);
        }

    }


    // Subscribing Delegate
    void OnEnable()
    {
        GizmosController.targetMoved += OnTargetMoved;
        BallController.firstLevelDone += onLevelOneCompleted;
    }


    void onLevelOneCompleted()
    {
        displayNextHint();
    }
    // Unsubscribing Delegate
    void OnDisable()
    {
        GizmosController.targetMoved -= OnTargetMoved;
        BallController.firstLevelDone -= onLevelOneCompleted;

    }
    public void displayNextHint()
    {
        if (current_hint == ((hints.Length) - 1))
        {
            GameManager.LevelLoaded = 2;
            SceneManager.LoadScene("LevelScene");
        }
        hints[current_hint].SetActive(false);

        current_hint++;
        if(current_hint == 5)
        {
            GameObject.Find("GameManager").GetComponent<DrawDotGame.GameManager>().LoadLevelOne();
            arrow.SetActive(true);
        }
        hints[current_hint].SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if(current_hint == 5)
        {
            //Draw arrow over the ant
            GameObject[] ants = GameObject.FindGameObjectsWithTag("Ball");
            if(ants.Length > 0)
                arrow.transform.position = new Vector3( (ants[0].transform.position.x + 0.05f), (ants[0].transform.position.y + 0.5f));
        }
    }
}
