using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<RoundConfig> roundList;
    [SerializeField] int startingWave = 0;
    [SerializeField] public int startingRound = 0;
    [SerializeField] public int currentRoundNumber = 0;
    [SerializeField] public int enemiesIn2D = 0;
    [SerializeField] public int enemiesIn3D = 0;
    [SerializeField] AudioSource musicPlayer2D;
    [SerializeField] AudioSource musicPlayer3D;
    [SerializeField] AudioClip alarmBeep;
    [SerializeField] MainMenuController mainMenuController;
    [SerializeField] TutorialScript tutorialScript;
    private CameraController cameraController;
    public bool shieldDown = false;
    public bool easyDifficulty = false;
    public bool playerDead = false;
    public bool gameEnded = false;
    private bool playing3DMusic = false;
    private bool playing2DMusic = true;
    private bool inMainScene = false;
    private bool lastWaveInRound = false;
    private bool penultimateWaveInRound = false;
    private bool breakLoop = false;
    private bool tutorialStarted = false;

    private void Awake()
    {
        SetUpSingleton();
        if (tutorialScript == null)
        {
            Debug.Log("got here");
            tutorialScript = GameObject.FindGameObjectWithTag("Tutorial1").GetComponent<TutorialScript>();
        }
        
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
        mainMenuController = GameObject.FindGameObjectWithTag("MenuController").GetComponent<MainMenuController>();
        musicPlayer2D.volume = 0.3f;
        musicPlayer3D.volume = 0f;
        
        if (mainMenuController.easyDifficulty)
        {
            easyDifficulty = true;
            inMainScene = true;
            StartScene();
        }
        else
        {
            easyDifficulty = false;
            inMainScene = true;
            StartScene();
        }
    }

    private void StartScene()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        enemiesIn3D = 0;
        enemiesIn2D = 0;
        
        if (playing2DMusic)
        {
            musicPlayer2D.volume = 0.3f;
            musicPlayer3D.volume = 0f;
        }
        else
        {
            playing2DMusic = true;
            playing3DMusic = false;
            StartCoroutine(ChangeMusic(true));
        }
        if (inMainScene)
        {
            playerDead = false;
            StartCoroutine(SpawnAllRounds());
        }
    }

    private void Update()
    {
        if (cameraController.movingTo2D && !playing2DMusic)
        {
            playing2DMusic = true;
            playing3DMusic = false;
            StartCoroutine(ChangeMusic(true));

        }
        else if (cameraController.movingTo3D && !playing3DMusic)
        {
            playing2DMusic = false;
            playing3DMusic = true;
            StartCoroutine(ChangeMusic(false));
        }

    }

    private void LateUpdate()
    {
        transform.position = Camera.main.transform.position;
    }

    public void ReloadScene()
    {
        startingRound = currentRoundNumber;
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartScene();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
        mainMenuController.PlayMenuSong();
        Destroy(gameObject);
    }

    private IEnumerator ChangeMusic(bool changeTo2D)
    {
        for (int i = 0; i < 10; i++)
        {
            if (changeTo2D)
            {
                musicPlayer2D.volume = musicPlayer2D.volume + 0.03f;
                musicPlayer3D.volume = musicPlayer3D.volume - 0.03f;
            }
            else
            {
                musicPlayer2D.volume = musicPlayer2D.volume - 0.03f;
                musicPlayer3D.volume = musicPlayer3D.volume + 0.03f;
            }
            yield return new WaitForSeconds(0.1f);

        }
    }

    public void ShieldBugKilled(bool isKilled)
    {
        shieldDown = true;
        StartCoroutine(TimeUntilShieldsReturn());
    }

    public bool AreShieldsDown()
    {
        return shieldDown;
    }

    private IEnumerator TimeUntilShieldsReturn()
    {
        yield return new WaitForSeconds(1f);
        shieldDown = false;
    }

    private IEnumerator SpawnAllRounds()
    {
        for (int roundIndex = startingRound; roundIndex < roundList.Count; roundIndex++)
        {
            var currentRound = roundList[roundIndex];

            yield return StartCoroutine(SpawnAllWaves(currentRound));

            while (!breakLoop)
            {
                if (penultimateWaveInRound && !tutorialStarted)
                {
                    tutorialStarted = true;
                    if (tutorialScript == null)
                    {
                        Debug.Log("Why are you not here");
                        tutorialScript = GameObject.FindGameObjectWithTag("Tutorial1").GetComponent<TutorialScript>();
                    }
                    if (!playerDead)
                    {
                        tutorialScript.StartTutorial();
                    }
                }
                if (lastWaveInRound)
                {
                    if (enemiesIn2D <= 0 && enemiesIn3D <= 0)
                    {
                        if (currentRoundNumber == 5)
                        {
                            gameEnded = true;
                        }
                        enemiesIn2D = 0;
                        enemiesIn3D = 0;
                        lastWaveInRound = false;
                        breakLoop = true;
                    }
                }
                yield return new WaitForSeconds(2f);
            }
            currentRoundNumber++;
            breakLoop = false;
        }
    }

    private IEnumerator SpawnAllWaves(RoundConfig roundConfig)
    {
        List<WaveConfig> waveConfigs = roundConfig.getWaveList();
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            var currentWave = waveConfigs[waveIndex];
            if (waveIndex == waveConfigs.Count - 1)
            {
                lastWaveInRound = true;
            }
            if (waveIndex == waveConfigs.Count - 2)
            {
                penultimateWaveInRound = true;
            }
            yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));

            yield return new WaitForSeconds(waveConfigs[waveIndex].GetTimeUntilNextWave());
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for (int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            var newEnemy = Instantiate(waveConfig.GetEnemyPrefab(), waveConfig.GetWaypoints()[0].transform.position, Quaternion.identity);
            if (!newEnemy.GetComponent<EnemyController3D>())
            {
                enemiesIn2D++;
                newEnemy.GetComponent<EnemyController>().SetWaveConfig(waveConfig);
            }
            else 
            {
                enemiesIn3D++;
                newEnemy.GetComponent<EnemyController3D>().SetWaveConfig(waveConfig);
            }
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }
}
