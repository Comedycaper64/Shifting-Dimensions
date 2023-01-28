using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetButtonEventStartGame : MonoBehaviour
{
    private Button button;
    private MainMenuController script;

    private void Awake()
    {
        button = GetComponent<Button>();
        script = GameObject.FindGameObjectWithTag("MenuController").GetComponent<MainMenuController>();

        button.onClick.AddListener(OnClickEvent);
    }

    private void OnClickEvent()
    {
        script.AdvanceToDifficultySelection();
    }
}
