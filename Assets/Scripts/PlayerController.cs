using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ship Attributes")] 
    [SerializeField] private float fireDelay = 0.1f;
    [SerializeField] private float fireDelay3D = 0.2f;
    [SerializeField] public float health = 4f;
    [SerializeField] private float timeBeforeShieldRecharge = 5f;
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private bool warning2DDisplayed = false;
    [SerializeField] private bool warning3DDisplayed = false;
    [SerializeField] public bool canSwitch = false;

    [Header("Assigned Objects")]
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject playerBullet3D;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject ship3DModel;
    [SerializeField] private GameObject ship3DModelFiringHole;
    [SerializeField] private GameObject qteObject;
    [SerializeField] private GameObject warningSign;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public GameObject switchUI;
    [SerializeField] public GameObject switchUIBackground;
    [SerializeField] private TutorialScript tutorialScript;
    [SerializeField] private GameObject gameEndScreen;

    [Header("Sprites")]
    [SerializeField] private Sprite shieldFull;
    [SerializeField] private Sprite shieldDamaged;
    [SerializeField] private Sprite shieldHeavilyDamaged;

    [Header("Audio")]
    [SerializeField] private AudioClip shooting2DSFX;
    [SerializeField] private AudioClip dying2DSFX;
    [SerializeField] private AudioClip losingShield2DSFX;
    [SerializeField] private AudioClip startQTESFX;
    [SerializeField] private AudioClip enemyWarning2DSFX;
    [SerializeField] private AudioClip shooting3DSFX;
    [SerializeField] private AudioClip gettingHit3DSFX;
    [SerializeField] private AudioClip shieldRegenerating;
    [SerializeField] private float audioVolume = 0.5f;

    private SpriteRenderer shieldRenderer;
    private QTESystem qteSystem;
    private CameraController mainCameraController;
    private bool firstTimeSwitching = true;
    private bool qteRunning = false;
    EnemySpawner enemySpawnerScript;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;
    private float padding = 0.5f;

    Coroutine qteCoroutine;
    Coroutine shieldCoroutine;
    Coroutine firingCoroutine;
    Coroutine firingCoroutine3D;

    // Start is called before the first frame update
    void Start()
    {
        enemySpawnerScript = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        mainCameraController = mainCamera.GetComponent<CameraController>();
        shieldRenderer = shield.GetComponent<SpriteRenderer>();
        qteSystem = qteObject.GetComponent<QTESystem>();
        if (enemySpawnerScript.easyDifficulty)
        {
            timeBeforeShieldRecharge = 3f;
            health = 4f;
        }
        else
        {
            timeBeforeShieldRecharge = 5f;
            health = 3f;
        }
        if (enemySpawnerScript.startingRound > 0)
        {
            canSwitch = true;
            switchUI.SetActive(true);
            switchUIBackground.SetActive(true);
        }
        SetUpMoveBoundaries();   
    }

    // Update is called once per frame
    void Update()
    {
        if (enemySpawnerScript.gameEnded)
        {
            gameEndScreen.SetActive(true);
        }
        if (qteRunning == false)
        {
            if (mainCameraController.isCamera2D)
            {
                if (enemySpawnerScript.enemiesIn3D > 0 && !warning2DDisplayed)
                {
                    warning2DDisplayed = true;
                    AudioSource.PlayClipAtPoint(enemyWarning2DSFX, Camera.main.transform.position);
                    DisplayEnemyWarning(true, false);
                }
                
                if (enemySpawnerScript.enemiesIn3D == 0 && warning2DDisplayed)
                {
                    warning2DDisplayed = false;
                    DisplayEnemyWarning(true, true);
                }
                Move2D();
                Fire2D();
            }
            else
            {
                if (enemySpawnerScript.enemiesIn2D > 0 && !warning3DDisplayed)
                {
                    warning3DDisplayed = true;
                    AudioSource.PlayClipAtPoint(enemyWarning2DSFX, Camera.main.transform.position);
                    DisplayEnemyWarning(false, false);
                }
                
                if (enemySpawnerScript.enemiesIn2D == 0 && warning3DDisplayed)
                {
                    warning3DDisplayed = false;
                    DisplayEnemyWarning(false, true);
                }
                if (!mainCameraController.movingTo3D)
                {
                    Move3D();
                    Fire3D();
                }
            }
        }
        else 
        {
            if (qteSystem.CheckQTE())
            {
                StopCoroutine(qteCoroutine);
                CheckQTE();
            }
        }
                

        if (Input.GetKeyDown(KeyCode.R) && mainCameraController.movingTo2D == false && mainCameraController.movingTo3D == false && qteRunning == false && canSwitch)
        {
            StartCoroutine(SwitchCooldown());
            canSwitch = false;
            qteCoroutine = StartCoroutine(StartQTE());
            AudioSource.PlayClipAtPoint(startQTESFX, Camera.main.transform.position, audioVolume);
            StopCoroutine(firingCoroutine);
            StopCoroutine(firingCoroutine3D);
        }
        
    }

    private void Move2D()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);

        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed;
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void Fire2D()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    private void Move3D()
    {
        var deltaY = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
        var newYRot = new Vector3(0, ship3DModel.transform.eulerAngles.y + deltaY, 0);

        ship3DModel.transform.eulerAngles = newYRot;
    }

    private void Fire3D()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            firingCoroutine3D = StartCoroutine(FireContinuously3D());
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(firingCoroutine3D);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            Instantiate(playerBullet, transform.position + new Vector3(0, 0.25f), Quaternion.identity);
            AudioSource.PlayClipAtPoint(shooting2DSFX, Camera.main.transform.position, audioVolume);
            yield return new WaitForSeconds(fireDelay);
        }
    }

    IEnumerator FireContinuously3D()
    {
        while (true)
        {
            Instantiate(playerBullet3D, ship3DModelFiringHole.transform.position, ship3DModelFiringHole.transform.rotation);
            AudioSource.PlayClipAtPoint(shooting3DSFX, Camera.main.transform.position, audioVolume);
            yield return new WaitForSeconds(fireDelay3D);
        }
    }

    IEnumerator TemporaryInvincibility()
    {
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(255, 255, 255, 0.2f);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(255, 255, 255, 1f);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(255, 255, 255, 0.2f);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(255, 255, 255, 1f);
        gameObject.GetComponent<PolygonCollider2D>().enabled = true;

    }

    private IEnumerator SwitchCooldown()
    {
        switchUI.SetActive(false);
        yield return new WaitForSeconds(5f);
        canSwitch = true;
        switchUI.SetActive(true);
    }

    IEnumerator StartQTE()
    {
        qteRunning = true;
        if (mainCameraController.isCamera2D)
        {
            qteSystem.StartInto2DQTE();
        }
        else
        {
            qteSystem.StartInto3DQTE();
        }

        yield return new WaitForSeconds(5f);
        CheckQTE();
    }

    public void CheckQTE()
    {
        if (qteSystem.CheckQTE())
        {
            if (mainCameraController.isCamera2D)
            {
                StopCoroutine(FireContinuously());
                DisplayEnemyWarning(true, true);
                mainCameraController.CameraInto3D();

            }
            else
            {
                StopCoroutine(FireContinuously3D());
                DisplayEnemyWarning(false, true);
                mainCameraController.CameraInto2D();

            }
            if (firstTimeSwitching && enemySpawnerScript.startingRound == 0)
            {
                firstTimeSwitching = false;
                tutorialScript.DisplayTutorial3();
            }
        }
        qteSystem.ResetQTE();
        qteRunning = false;
    }

    private void DisplayEnemyWarning(bool in2D, bool stopWarning)
    {
        if (!stopWarning)
        {
            warningSign.SetActive(true);
        }
        else
        {
            warningSign.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBullet enemyBullet = collision.gameObject.GetComponent<EnemyBullet>();
        ProcessHit(enemyBullet);

    }

    private void ProcessHit(EnemyBullet enemyBullet)
    {
        if (!enemyBullet)
        {
            return;
        }
        else
        {
            enemyBullet.HitPlayer();
            health -= enemyBullet.GetDamage();
            StartCoroutine(TemporaryInvincibility());
            AudioSource.PlayClipAtPoint(losingShield2DSFX, Camera.main.transform.position, audioVolume);
            if (!enemySpawnerScript.easyDifficulty)
            {
                if (health == 2)
                {
                    shieldCoroutine = StartCoroutine(RegenerateShield());
                    shieldRenderer.sprite = shieldHeavilyDamaged;
                }
                else if (health == 1)
                {
                    shieldRenderer.sprite = null;
                }
            }
            else
            {
                if (health == 3)
                {
                    shieldCoroutine = StartCoroutine(RegenerateShield());
                    shieldRenderer.sprite = shieldDamaged;
                }
                else if (health == 2)
                {
                    shieldRenderer.sprite = shieldHeavilyDamaged;
                }
                else if (health == 1)
                {
                    shieldRenderer.sprite = null;
                }
            }

            if (health <= 0)
            {
                StopCoroutine(shieldCoroutine);
                GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
                Destroy(explosion, 1f);
                AudioSource.PlayClipAtPoint(dying2DSFX, Camera.main.transform.position, audioVolume);
                gameOverUI.SetActive(true);
                enemySpawnerScript.playerDead = true;
                Destroy(gameObject);
            }

        }
    }

    IEnumerator RegenerateShield()
    {
        yield return new WaitForSeconds(timeBeforeShieldRecharge);
        health++;
        AudioSource.PlayClipAtPoint(shieldRegenerating, Camera.main.transform.position, audioVolume);

        if (!enemySpawnerScript.easyDifficulty)
        {
            if (health == 3)
            {
                shieldRenderer.sprite = shieldFull;
            }
            else if (health == 2)
            {
                shieldRenderer.sprite = shieldHeavilyDamaged;
                shieldCoroutine = StartCoroutine(RegenerateShield());
            }
        }
        else
        {
            if (health == 4)
            {
                shieldRenderer.sprite = shieldFull;
            }
            else if (health == 3)
            {
                shieldRenderer.sprite = shieldDamaged;
                shieldCoroutine = StartCoroutine(RegenerateShield());
            }
            else if (health == 2)
            {
                shieldRenderer.sprite = shieldHeavilyDamaged;
                shieldCoroutine = StartCoroutine(RegenerateShield());
            }
        }
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1)).y - padding;
    }
}
