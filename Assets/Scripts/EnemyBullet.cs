using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float downwardsVelocity = 5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            FlyDown();
            DestroyAfterWhile();
        }
    }

    private void FlyDown()
    {
        rb.velocity = new Vector2(0, -downwardsVelocity);
    }

    private void DestroyAfterWhile()
    {
        if (transform.position.y < -15)
        {
            Destroy(gameObject);
        }
    }

    public void HitPlayer()
    {
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }
}
