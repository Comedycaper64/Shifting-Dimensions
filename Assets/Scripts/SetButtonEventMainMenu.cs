using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetButtonEventMainMenu : MonoBehaviour
{
    private Button button;
    private EnemySpawner script;

    private void Awake()
    {
        button = GetComponent<Button>();
        script = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();

        button.onClick.AddListener(OnClickEvent);
    }

    private void OnClickEvent()
    {
        script.GoToMainMenu();
    }
}
