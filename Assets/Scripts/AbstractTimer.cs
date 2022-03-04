using System;
using UnityEngine;

public abstract class AbstractTimer : MonoBehaviour
{
    public bool Running { get; protected set; }
    public bool JustFinished { get; protected set; }
    public float Elapsed { get; protected set; }
    public float Duration { get; protected set; }
    public float Remaining
    {
        get { return Duration - Elapsed; }
    }

    public void Init(float duration)
    {
        Running = false;
        JustFinished = false;
        Elapsed = 0;
        Duration = duration;
    }
    public void Run()
    {
        Running = true;
        Elapsed = 0;
    }
    public void Stop()
    {
        Running = false;
        JustFinished = true;
    }
}