using Assets.Globals;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private int bucketCount = 4;   // 4 x 0.25s = 1s window
    private float[] times;
    private int[] frames;
    private int idx;
    [NonSerialized] public float fps;
    public int targetFPS = 165;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bucketCount = Mathf.Max(1, bucketCount);
        times = new float[bucketCount];
        frames = new int[bucketCount];
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bucketCount = Mathf.Max(1, bucketCount);
        if (times.Length != bucketCount)
        {
            float[] temp_times = times;
            times = new float[bucketCount];
            for (int i = 0; i < Math.Min(bucketCount, temp_times.Length); i++)
            {
                times[i] = temp_times[i];
            }
            idx = 0;
        }
        if (frames.Length != bucketCount)
        {
            int[] temp_frames = frames;
            frames = new int[bucketCount];
            for (int i = 0; i < Math.Min(bucketCount, temp_frames.Length); i++)
            {
                times[i] = temp_frames[i];
            }
            idx = 0;
        }

        Application.targetFrameRate = targetFPS;
        times[idx] += Time.unscaledDeltaTime;
        frames[idx]++;

        if (times[idx] >= 1f/bucketCount)
        {
            float t = 0f; int f = 0;
            for (int i = 0; i < Mathf.Min(times.Length, frames.Length, bucketCount); i++) { t += times[i]; f += frames[i]; }
            fps = f / t;

            idx = (idx + 1) % bucketCount;
            times[idx] = 0f;
            frames[idx] = 0;
        }

        GlobalVars.framesPerSecond = fps;
    }
}
