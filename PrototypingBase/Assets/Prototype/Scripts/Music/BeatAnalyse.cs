using System.Collections.Generic;
using UnityEngine;

public class BeatAnalyse : MonoBehaviour
{
    [SerializeField]
    private int windowTrigger;
    [SerializeField]
    private int reactionTime;
    private List<int> beatStarts = new List<int>();

    [SerializeField]
    private float drawWidth;
    [SerializeField]
    private float limit, waitSamples;
    private float timeSample;
    private float[] spectrum;

    [SerializeField]
    private AudioClip wave;
    private AudioSource sourceWave;

    void Start()
    {
        sourceWave = GetComponent<AudioSource>();
        int amount = wave.samples;
        spectrum = new float[amount];
        wave.GetData(spectrum, 0);

        for (int i = 0; i < spectrum.Length; i++)
        {
            spectrum[i] = Mathf.Abs(spectrum[i]);
        }

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            if (spectrum[i] > limit)
            {
                if (spectrum[i] <= spectrum[i - 1] && spectrum[i] >= spectrum[i + 1])
                {
                    beatStarts.Add(i);
                    i += (int)waitSamples;
                }
            }
        }
    }

    public bool IsOnBeat(int preStart)
    {
        timeSample = sourceWave.timeSamples - preStart;
        for (int i = 0; i < beatStarts.Count; i++)
        {
            if (timeSample >= (beatStarts[i] - windowTrigger) &&
                timeSample <= (beatStarts[i] + windowTrigger))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (spectrum == null)
        {
            return;
        }
        Vector3 displacement = Camera.main.ScreenToWorldPoint(new Vector3(100, 100, 5));
        float heightMulti = 1;
        float widthMulti = 0.000005f;
        Gizmos.color = new Color(0.5f, 0, 0.5f, 1);

        for (int i = 0; i < spectrum.Length; i += 100)
        {
            Gizmos.DrawLine(displacement + new Vector3(i * widthMulti, 0, 0),
                            displacement + new Vector3(i * widthMulti, heightMulti * spectrum[i], 0));
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < beatStarts.Count; i++)
        {
            Gizmos.DrawLine(displacement + new Vector3((beatStarts[i] - windowTrigger + reactionTime) * widthMulti, 0, 0),
                            displacement + new Vector3((beatStarts[i] + windowTrigger + reactionTime) * widthMulti, 0, 0));
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(displacement + new Vector3(sourceWave.timeSamples * widthMulti, 0, 0), displacement + new Vector3(sourceWave.timeSamples * widthMulti, heightMulti, 0));

    }
}