using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundTracker : MonoBehaviour
{
    EnemySpawner enemySpawnerScript;
    Text roundText;

    // Start is called before the first frame update
    void Start()
    {
        enemySpawnerScript = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        roundText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        roundText.text = "Round: " + enemySpawnerScript.currentRoundNumber + " / 5";
    }
}
