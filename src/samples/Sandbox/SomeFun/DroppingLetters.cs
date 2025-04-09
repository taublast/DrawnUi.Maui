namespace DrawnUi.Demo.Views.Controls;

public class DroppingLetters : SkiaLabel
{
    public DroppingLetters()
    {
    }

    protected override SKRect GetCacheRecordingArea(SKRect drawingRect)
    {
        return Superview.Destination; //record whole area because we are animating letters outside of the DrawingRect
    }

    public override void OnDisposing()
    {
        StopAnimators();

        base.OnDisposing();
    }

    public override void OnParentVisibilityChanged(bool newvalue)
    {
        if (!newvalue)
        {
            StopAnimators();
        }

        base.OnParentVisibilityChanged(newvalue);
    }

    protected override void OnPropertyChanged(string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(Text))
        {
            StopAnimators();
        }
    }

    protected override void DrawCharacter(SKCanvas canvas, int lineIndex, int letterIndex, ReadOnlySpan<char> characters, float x, float y,
        SKPaint paint, SKPaint paintStroke, SKPaint paintDropShadow, SKRect destination, float scale)
    {
        // 1 line enabled only!
        if (lineIndex == 0 && _letterOffsetsY != null)
        {
            var animated = _letterOffsetsY[letterIndex] != double.NegativeInfinity;
            if (animated)
            {

                var offsetY = (float)(_letterOffsetsY[letterIndex] * scale);

                //move gradient
                var dest = new SKRect(destination.Left, destination.Top - offsetY, destination.Right,
                    destination.Bottom - offsetY);
                SetupGradient(paint, FillGradient, dest);

                if (paintStroke != null)
                {
                    SetupGradient(paintStroke, StrokeGradient, dest);
                }

                base.DrawCharacter(canvas, lineIndex, letterIndex, characters, x, y - offsetY, paint, paintStroke, paintDropShadow, destination, scale);
            }
        }
    }

    private bool lockStart;
    public void StartAnimation()
    {
        if (lockStart)
        {
            return;
        }

        lockStart = true;

        SetupAnimators();

        if (_letterOffsetsY != null)
        {
            var index = 0;

            void StartDelayed(int index, ISkiaAnimator[] animators)
            {
                if (index < animators.Length)
                {
                    var animator = animators[index];

                    Task.Run(async () =>
                    {
                        await Task.Delay(50);
                        animator.Start();
                        StartDelayed(index + 1, animators);
                    }).ConfigureAwait(false);

                }

            }

            StartDelayed(0, _animators.ToArray());
        }

        lockStart = false;
    }
    public void SetupAnimators()
    {
        StopAnimators();
        _letterOffsetsY = new double[this.Text.Length];
        Array.Fill(_letterOffsetsY, double.NegativeInfinity);
        if (!string.IsNullOrEmpty(this.Text))
        {

            var max = GetParentElement(this).Height;

            for (int i = 0; i < Text.Length; i++)
            {
                var index = i;
                var animator = new PendulumAnimator(this, (value) =>
                {
                    try
                    {
                        _letterOffsetsY[index] = value;// * MeasuredSize.Scale;
                        Update();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                })
                {
                    IsOneDirectional = true,
                    Speed = 3.0,
                    mMinValue = 0,
                    mMaxValue = max,
                    Amplitude = max,
                    Gravity = 9.8,
                    AirResistance = 1.0
                };
                _animators.Add(animator);
                //put offscreen
                _letterOffsetsY[index] = -max;
            }
        }
    }
    public void StopAnimators()
    {
        foreach (var animator in _animators.ToList())
        {
            animator.Stop();
        }
        _animators.Clear();
    }

    private double[] _letterOffsetsY;

    private List<ISkiaAnimator> _animators = new();

}
