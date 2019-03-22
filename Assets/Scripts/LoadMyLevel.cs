using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMyLevel : MonoBehaviour {

    public void LoadLevel(string name)
    {

        Debug.Log("Level load requested for " + name);
        SceneManager.LoadScene(name);

    }
}
