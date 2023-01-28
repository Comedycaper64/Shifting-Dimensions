using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] GameObject tutorialUI1;
    [SerializeField] GameObject tutorialUI2;
    [SerializeField] GameObject tutorialUI3;
    [SerializeField] GameObject switchUI;
    [SerializeField] GameObject switchBackground;
    [SerializeField] private PlayerController player;

    public void StartTutorial()
    {
        if (tutorialUI1 == null)
        {
            Debug.Log("wjat the fuck");
        }
        tutorialUI1.SetActive(true);
    }

    public void AdvanceTutorial1()
    { 
        player.canSwitch = true;
        switchUI.SetActive(true);
        switchBackground.SetActive(true);
        tutorialUI1.SetActive(false);
        tutorialUI2.SetActive(true);
    }

    public void AdvanceTutorial2()
    {
        tutorialUI2.SetActive(false);
    }

    public void DisplayTutorial3()
    {
        tutorialUI3.SetActive(true);
    }

    public void AdvanceTutorial3()
    {
        tutorialUI3.SetActive(false);
    }
}
