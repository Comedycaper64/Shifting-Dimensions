using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private GameObject player3D;
    // Start is called before the first frame update
    void Start()
    {
        player3D = GameObject.FindGameObjectWithTag("Player3D");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player3D.transform);
    }
}
