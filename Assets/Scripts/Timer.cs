public class Timer : AbstractTimer
{
    void Update()
    {
        if (Running)
        {
            Elapsed += UnityEngine.Time.deltaTime;
            if (Elapsed >= Duration)
                Stop();
        }
        else
            JustFinished = false;
    }
}