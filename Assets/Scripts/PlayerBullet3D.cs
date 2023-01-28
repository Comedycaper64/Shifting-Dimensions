using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet3D : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float forwardVelocity = 5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private AudioClip explodeSFX;
    [SerializeField] private GameObject explosion3D;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        FlyForward();
        StartCoroutine(DestroyAfterWhile());
    }

    private void FlyForward()
    {
        var locVel = transform.InverseTransformDirection(rb.velocity);
        locVel.z = forwardVelocity;
        rb.velocity = transform.TransformDirection(locVel);
    }

    private IEnumerator DestroyAfterWhile()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);   
    }

    public void HitEnemy()
    {
        AudioSource.PlayClipAtPoint(explodeSFX, Camera.main.transform.position, 0.1f);
        GameObject explosion = Instantiate(explosion3D, transform.position, transform.rotation);
        Destroy(explosion, 0.5f);
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }
}
