namespace DrawnUi.Draw;

public class PingPongAnimator : RangeAnimator
{
    public PingPongAnimator(SkiaControl player) : base(player)
    {
        Repeat = -1;
    }

    protected override bool FinishedRunning()
    {
        if (Repeat < 0) //forever
        {
            (mMaxValue, mMinValue) = (mMinValue, mMaxValue);
            Distance = mMaxValue - mMinValue;
            
            mValue = mMinValue;
            mLastFrameTime = 0;
            mStartFrameTime = 0;
            return false;
        }
        else if (Repeat > 0)
        {
            Repeat--;

            (mMaxValue, mMinValue) = (mMinValue, mMaxValue);
            Distance = mMaxValue - mMinValue;
            
            mValue = mMinValue;
            mLastFrameTime = 0;
            mStartFrameTime = 0;
            return false;
        }
        return base.FinishedRunning();
    }
}