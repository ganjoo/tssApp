using UnityEngine;
using System.Collections;

public class BrowserOpener : MonoBehaviour {

	public string home = "http://www.google.com";
    public string liveFeed = "";
    public string courses = "";
    public string facultyAboutUs = "";
    public string feedback = "";
    public string ecmoFellowship = "";
    public string cccFellowship = "";
    public string echocardiographyFellowship = "";
    public string diabetesFellowship = "";

    public string nmvhEcmoWorkshop = "";
    public string tentativeSchedule = "";

    private void Start()
    {
        OnButtonFacultyClicked();
    }

    // check readme file to find out how to change title, colors etc.
    public void OnButtonHomeClicked() {
		InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
		options.displayURLAsPageTitle = false;
		options.pageTitle = "The Simulation Society App";

		InAppBrowser.OpenURL(home, options);
	}

    public void OnButtonLiveFeedClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(liveFeed, options);
    }

    public void OnButtonCourseslicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(courses, options);
    }

    public void OnButtonFacultyClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(facultyAboutUs, options);
    }

    public void OnButtonFeedbackClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(feedback, options);
    }

    public void OnButtonEchocardiographyClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(echocardiographyFellowship, options);
    }

    public void OnButtonCCCClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(cccFellowship, options);
    }

    public void OnButtonECMOClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(ecmoFellowship, options);
    }

    public void OnButtonDiabetesClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(diabetesFellowship, options);
    }

    public void OnButtonTentativeScheduleClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(tentativeSchedule, options);
    }

    public void OnButtonnmvhEcmoWorkshopClicked()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = "The Simulation Society App";

        InAppBrowser.OpenURL(nmvhEcmoWorkshop, options);
    }
   

    public void OnClearCacheClicked() {
		InAppBrowser.ClearCache();
	}
}
