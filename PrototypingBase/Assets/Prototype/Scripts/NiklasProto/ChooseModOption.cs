using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseModOption : MonoBehaviour
{

    public void Choose()
    {
        WaveManager.instance.waveSpawnPaused = false;
        WaveManager.instance.modSelected = true;
        UIManager.instance.ToggleAugmentSelection(false);
        
    }
}
