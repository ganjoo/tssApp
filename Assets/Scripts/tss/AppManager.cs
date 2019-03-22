using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{

    public GameObject homeSlider;
    public GameObject courseSlider;
    public ScrollRect scrollRectContainer;

    // Start is called before the first frame update
    void Start()
    {
        openHome();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openFaculty()
    {
        GameObject[] slides = GameObject.FindGameObjectsWithTag("slides");
        foreach(GameObject slide in slides)
        {
            if (slide.name != "FacultySlide")
                slide.SetActive(false);
        }
    }



    public void openHome()
    {
        homeSlider.SetActive(true);
        courseSlider.SetActive(false);
        scrollRectContainer.content = homeSlider.transform.GetComponent<RectTransform>();
    }

    public void openCourses()
    {

        homeSlider.SetActive(false);
        courseSlider.SetActive(true);
        scrollRectContainer.content = courseSlider.transform.GetComponent<RectTransform>();

    }

    void openJournals()
    {

    }
}
