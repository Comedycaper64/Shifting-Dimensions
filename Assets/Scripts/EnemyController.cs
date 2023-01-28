using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    WaveConfig waveConfig;
    private int waypointIndex = 0;
    float health = 1;
    float shotCounter = 0;
    float minTimeBetweenShots = 0.2f;
    float maxTimeBetweenShots = 1f;
    EnemySpawner enemySpawnerScript;
    private CameraController cameraScript;

    [SerializeField] GameObject enemyBullet;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private GameObject shieldSprite;
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip explodeSFX;
    [SerializeField] private float audioVolume = 0.5f;
    private bool shieldEnabled = false;
    private Vector3 enemyRotation = new Vector3(0, 0, 180);
    //

    // Start is called before the first frame update
    void Start()
    {
        cameraScript = Camera.main.GetComponent<CameraController>();
        if (gameObject.tag == "ShieldedEnemy")
        {
            shieldEnabled = true;
        }
        enemySpawnerScript = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        waypoints = waveConfig.GetWaypoints();
        minTimeBetweenShots = waveConfig.GetMinTimeBetweenShots();
        maxTimeBetweenShots = waveConfig.GetMaxTimeBetweenShots();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (enemySpawnerScript.AreShieldsDown() && gameObject.tag == "ShieldedEnemy")
        {
            shieldSprite.SetActive(false);
            shieldEnabled = false;
        }
        if (waveConfig.GetShootStatus())
        {
            CountDownAndShoot();
        }
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            waypointIndex = 1;
        }
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0)
        {
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
            if (cameraScript.isCamera2D)
            {
                AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, audioVolume);
            }
            Instantiate(enemyBullet, transform.position, Quaternion.Euler(enemyRotation));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBullet playerBullet = collision.gameObject.GetComponent<PlayerBullet>();
        ProcessHit(playerBullet);
        
    }

    private void ProcessHit(PlayerBullet playerBullet)
    {
        if (!playerBullet)
        {
            return;
        }
        else
        {
            playerBullet.HitEnemy();
            if (!shieldEnabled)
            {
                health -= playerBullet.GetDamage();
                if (health <= 0)
                {
                    enemySpawnerScript.enemiesIn2D--;
                    Die();
                }
            }
        }
    }

    private void Die()
    {
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(explodeSFX, Camera.main.transform.position, audioVolume);
        Destroy(explosion, 1f);
        Destroy(gameObject);
    }

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        this.waveConfig = waveConfig;
    }
}
