using AppoMobi.Maui.Gestures;
using DrawnUi;
using DrawnUi;
using DrawnUi;
using DrawnUi.Animate.Animators;
using DrawnUi.Draw;
using SkiaSharp;

namespace DrawnUi.Demo.Views.Controls;

public partial class Lake : SkiaLayout
{

    private SkiaControl _duck;

    private VelocitySkiaAnimator _animationDuckMoveX;
    private VelocitySkiaAnimator _animationDuckMoveY;
    private PendulumAnimator _anumatorJump;

    const int ANIMATE_MINIMUM_DIP = 5;
    private const float ANIMATE_Friction = 0.05f;

    private bool _movingDuck;
    private double _duckMinX;
    private double _duckMinY;
    private double _duckMaxX;
    private double _duckMaxY;
    private double _moveToX;
    private double _moveToY;
    private double _velocityRatoX;
    private double _velocityRatoY;
    private DroppingLetters _welcome;
    private ActionOnTickAnimator _appLoop;

    /// <summary>
    /// Execute on drawing tick
    /// </summary>
    /// <param name="frameTime"></param>
    void ProcessWorld(long frameTime)
    {
        if (_animationDuckMoveX.IsRunning)
        {
            //todo play moving animation
            //Debug.WriteLine("duck moving on X");

        }
    }

    /// <summary>
    /// Setup scene
    /// </summary>
    protected override void OnMeasured()
    {
        base.OnMeasured();

        Setup();
    }

    public override void OnDisposing()
    {
        _anumatorJump?.Stop();
        _anumatorJump?.Dispose();

        _animationDuckMoveX?.Stop();
        _animationDuckMoveX?.Dispose();

        _animationDuckMoveY?.Stop();
        _animationDuckMoveY?.Dispose();

        base.OnDisposing();
    }

    void Setup()
    {
        var container = GetParentElement(this);

        if (_welcome == null)
        {
            _duck = Views.FirstOrDefault(x => x.Tag == "Duck") as SkiaControl;
            _welcome = Views.FirstOrDefault(x => x.Tag == "LabelWelcome") as DroppingLetters;

            if (_duck != null)
            {
                //moving limits
                _duckMinX = 0;
                _duckMinY = 0;
                _duckMaxX = Width - _duck.Width;
                _duckMaxY = Height - _duck.Height;

                //center duck on screen
                MoveDuckX((Width - _duck.Width) / 2.0f);
                MoveDuckY((Height - _duck.Height) / 2.0f);

                _animationDuckMoveX = new(_duck)
                {
                    mMinValue = _duckMinX,
                    mMaxValue = _duckMaxX,
                    Friction = ANIMATE_Friction,
                    InvertOnLimits = true
                };
                _animationDuckMoveX.OnUpdated += MoveDuckX;

                _animationDuckMoveY = new(_duck)
                {
                    mMinValue = _duckMinY,
                    mMaxValue = _duckMaxY,
                    Friction = ANIMATE_Friction,
                    InvertOnLimits = true
                };
                _animationDuckMoveY.OnUpdated += MoveDuckY;

                _anumatorJump = new PendulumAnimator(_duck, (value) =>
                {
                    if (!_movingDuck)
                    {
                        if (_moveToY < 100)
                            _moveToY = 100;
                        _duck.TranslationY = _moveToY - value;
                    }
                })
                {
                    IsOneDirectional = true,
                    Speed = 5.2,
                    mMinValue = 0,
                    mMaxValue = 100,
                    Amplitude = 100,
                };

            }

        }
        else
        {
            if (_duck != null)
            {
                //moving limits
                _duckMinX = 0;
                _duckMinY = 0;
                _duckMaxX = Width - _duck.Width;
                _duckMaxY = Height - _duck.Height;

            }

        }
        //we have velocity coming from the top parent container, so we adapt
        //it to the smaller size of this control
        var adaptVelocity = container.GetVelocityRatioForChild(this);
        _velocityRatoX = adaptVelocity.RatioX;
        _velocityRatoY = adaptVelocity.RatioY;

    }

    void MoveDuckX(double value)
    {
        if (value < _duckMinX)
            value = _duckMinX;
        else
        if (value > _duckMaxX)
            value = _duckMaxX;
        _moveToX = value;
        _duck.TranslationX = _moveToX;
    }

    void MoveDuckY(double value)
    {
        if (value < _duckMinY)
            value = _duckMinY;
        else
        if (value > _duckMaxY)
            value = _duckMaxY;
        _moveToY = value;
        _duck.TranslationY = _moveToY;
    }

    public override void OnParentVisibilityChanged(bool newvalue)
    {
        if (!newvalue)
        {
            StopAnimators();
        }

        base.OnParentVisibilityChanged(newvalue);
    }

    private void StopAnimators()
    {
        _anumatorJump?.Stop();
        _animationDuckMoveX?.Stop();
        _animationDuckMoveY?.Stop();
    }

    public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    {
        if (args.Type == TouchActionResult.Touch)
            return null;

        //		Trace.WriteLine($"[IN] {type} {action} dY: {args.Event.Distance.Delta.Y:0.00} dX: {args.Event.Distance.Delta.X:0.00} | vX: {args.Event.Distance.Velocity.X:0.00} vY: {args.Event.Distance.Velocity.Y:0.00}");



        if (_duck != null)
        {

            try
            {

                if (args.Type == TouchActionResult.Down && args.Event.NumberOfTouches < 2)
                {
                    StopAnimators();
                    _movingDuck = true;
                }
                else
                if (args.Type == TouchActionResult.Panning)
                {

                    var velocityX = (float)(args.Event.Distance.Velocity.X / _velocityRatoX);
                    _animationDuckMoveX.SetVelocity(velocityX).SetValue((float)_duck.TranslationX).Start();

                    var velocityY = (float)(args.Event.Distance.Velocity.Y / _velocityRatoY);
                    _animationDuckMoveY.SetVelocity(velocityY).SetValue((float)_duck.TranslationY).Start();

                    //var velocityX = args.Event.Distance.Velocity.X / _velocityRatoX;
                    //_animationDuckMoveX.SetVelocity((float)velocityX).SetValue((float)_duck.TranslationX).Start();

                    //var velocityY = args.Event.Distance.Velocity.Y / _velocityRatoY;
                    //_animationDuckMoveY.SetVelocity((float)velocityY).SetValue((float)_duck.TranslationY).Start();

                    //Console.WriteLine($"[V] {velocityX:0.00} {velocityY:0.00}");
                }
                else
                if (args.Type == TouchActionResult.Up)
                {
                    if (GestureStartedInside(args.Event))
                    {
                        _movingDuck = false;

                        //var velocityX = (float)(args.Event.Distance.Velocity.X / _velocityRatoX);
                        //_animationDuckMoveX.SetVelocity(velocityX).SetValue((float)_duck.TranslationX).Start();

                        //var velocityY = (float)(args.Event.Distance.Velocity.Y / _velocityRatoY);
                        //_animationDuckMoveY.SetVelocity(velocityY).SetValue((float)_duck.TranslationY).Start();


                        if (!_animationDuckMoveX.IsRunning)
                            MoveDuckX(args.Event.Location.X / RenderingScale - _duck.Width / 2);

                        if (!_animationDuckMoveY.IsRunning)
                            MoveDuckY(args.Event.Location.Y / RenderingScale - _duck.Height / 2);

                        _anumatorJump.Start();
                        _welcome.StartAnimation();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        //if (action != TouchActionResult.Touch)
        //{
        //	Debug.WriteLine($"[LAKE] {type} {action} d {args.Event.Distance.Delta.X}x{args.Event.Distance.Delta.Y}  v {args.Event.Distance.Velocity.X}x{args.Event.Distance.Velocity.Y}");
        //}

        return base.OnSkiaGestureEvent(args, apply);
    }
}
