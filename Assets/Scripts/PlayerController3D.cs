using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController3D : MonoBehaviour
{
    //[SerializeField] PlayerController playerController2D;
    private float health = 3f;
    [SerializeField] private AudioClip dying3DSFX;
    [SerializeField] private Sprite mediumHealth;
    [SerializeField] private Sprite lowHealth;
    [SerializeField] private Sprite emptyHealth;
    [SerializeField] private GameObject explosion3D;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject healthUI;

    private void Start()
    {
       
    }

    private void OnTriggerEnter(Collider collision)
    {
        EnemyController3D enemy = collision.gameObject.GetComponent<EnemyController3D>();
        ProcessHit(enemy);

    }

    private void ProcessHit(EnemyController3D enemy)
    {
        if (!enemy)
        {
            return;
        }
        else
        {
            enemy.HitPlayer();
            health -= enemy.GetDamage();
            if (health == 2)
            {
                healthUI.GetComponent<Image>().sprite = mediumHealth;
            }
            else if (health == 1)
            {
                healthUI.GetComponent<Image>().sprite = lowHealth;
            }
            if (health <= 0)
            {
                Camera.main.transform.parent = null;
                AudioSource.PlayClipAtPoint(dying3DSFX, Camera.main.transform.position);
                healthUI.GetComponent<Image>().sprite = emptyHealth;
                GameObject explosion = Instantiate(explosion3D, transform.position, transform.rotation);
                Destroy(explosion, 1f);
                gameOverUI.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}
