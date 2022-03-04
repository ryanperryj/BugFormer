public class FixedTimer : AbstractTimer
{
    void FixedUpdate()
    {
        if (Running)
        {
            Elapsed ++;
            if (Elapsed >= Duration)
                Stop();
        }
        else
            JustFinished = false;
    }
}