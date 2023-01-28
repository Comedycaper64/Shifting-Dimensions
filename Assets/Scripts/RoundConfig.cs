using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Round Config")]
public class RoundConfig : ScriptableObject
{
    [SerializeField] List<WaveConfig> waveList;

    public List<WaveConfig> getWaveList()
    {
        return waveList;
    }
}
