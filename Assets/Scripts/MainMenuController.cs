using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public bool easyDifficulty = false;
    public bool hardDifficulty = false;
    [SerializeField] private GameObject startButtonUI;
    [SerializeField] private GameObject difficultyUI;

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayMenuSong()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void AdvanceToDifficultySelection()
    {
        startButtonUI.SetActive(false);
        difficultyUI.SetActive(true);
    }

    public void EasyDifficulty()
    {
        easyDifficulty = true;
        gameObject.GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene("Main Scene");
    }

    public void HardDifficulty()
    {
        hardDifficulty = true;
        gameObject.GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene("Main Scene");
    }
}
