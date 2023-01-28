using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float upwardsVelocity = 5f;
    [SerializeField] private int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        FlyUp();
        DestroyAfterWhile();
    }

    private void FlyUp()
    {
        rb.velocity = new Vector2(0, upwardsVelocity);
    }

    private void DestroyAfterWhile()
    {
        if (transform.position.y > 15)
        {
            Destroy(gameObject);
        }
    }

    public void HitEnemy()
    {
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }
}
