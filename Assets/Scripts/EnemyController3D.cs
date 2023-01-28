using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController3D : MonoBehaviour
{
    WaveConfig waveConfig;
    [SerializeField] private List<Transform> waypoints;
    private int waypointIndex = 0;
    [SerializeField] float health = 1f;
    [SerializeField] float damageDealt = 1f;
    private GameObject player3D;
    private EnemySpawner enemySpawnerScript;
    [SerializeField] private AudioClip explodeSFX;
    [SerializeField] private GameObject explosion3D;

    // Start is called before the first frame update
    void Start()
    {
        player3D = GameObject.FindGameObjectWithTag("Player3D");
        enemySpawnerScript = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            Vector3 direction = targetPosition - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, movementThisFrame);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementThisFrame);
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

    private void OnTriggerEnter(Collider collision)
    {
        PlayerBullet3D playerBullet = collision.gameObject.GetComponent<PlayerBullet3D>();
        ProcessHit(playerBullet);

    }

    private void ProcessHit(PlayerBullet3D playerBullet)
    {
        if (!playerBullet)
        {
            return;
        }
        else
        {
            playerBullet.HitEnemy();
            health -= playerBullet.GetDamage();
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        enemySpawnerScript.enemiesIn3D--;
        AudioSource.PlayClipAtPoint(explodeSFX, Camera.main.transform.position, 0.3f);
        GameObject explosion = Instantiate(explosion3D, transform.position, transform.rotation);
        Destroy(explosion, 1f);
        if (gameObject.tag == "ShieldBug")
        {
            enemySpawnerScript.ShieldBugKilled(true);
        }
        Destroy(gameObject);
    }

    public void HitPlayer()
    {
        Die();
    }

    public float GetDamage()
    {
        return damageDealt;
    }

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        this.waveConfig = waveConfig;
    }
}
