using Android.Content;
using Android.Views;
using AppoMobi.Framework.Forms.Touch;
using AppoMobi.Framework.Forms.UI.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Exception = System.Exception;
using Math = System.Math;
using TouchEffect = AppoMobi.Framework.Droid.Touch.TouchEffect;
using View = Android.Views.View;

[assembly: ResolutionGroupName("AMFEffects")]
[assembly: ExportEffect(typeof(TouchEffect), "TouchEffect")]

namespace AppoMobi.Framework.Droid.Touch
{
    public class TouchEffect : PlatformEffect
    {
        Android.Views.View view;
        //Element formsElement;
        public Forms.Touch.TouchEffect FormsEffect;
        bool capture;
        Func<double, double> fromPixels;

        int[] twoIntArray = new int[2];

        static Dictionary<Android.Views.View, TouchEffect> viewDictionary =
            new Dictionary<Android.Views.View, TouchEffect>();

        static Dictionary<int, TouchEffect> idToEffectDictionary = new Dictionary<int, TouchEffect>();
        private TouchListener _touchHandler;

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            Forms.Touch.TouchEffect touchEffect =
                (Forms.Touch.TouchEffect)Element.Effects.
                    FirstOrDefault(e => e is Forms.Touch.TouchEffect);

            if (touchEffect != null && view != null)
            {
                touchEffect.Attach();

                viewDictionary.Add(view, this);

                //if (touchEffect.TouchMode == TouchHandlingStyle.Self)
                //{
                //    var stop = true;
                //}

                FormsEffect = touchEffect;

                // Save fromPixels function
                fromPixels = view.Context.FromPixels;

                // Set event handler on View
                view.SetOnTouchListener(new TouchListener(view.Context));
                //view.Touch += OnTouch;

                FormsEffect.Disposing += OnFormsDisposing;

            }
        }



        private void OnFormsDisposing(object sender, EventArgs e)
        {
            OnDetached();
        }

        protected override void OnDetached()
        {
            if (FormsEffect != null)
            {
                FormsEffect.Disposing -= OnFormsDisposing;

                FormsEffect?.Dispose();
                FormsEffect = null;
            }

            if (viewDictionary.ContainsKey(view))
            {
                viewDictionary.Remove(view);
                //view.Touch -= OnTouch;
                view.SetOnTouchListener(null);
                _touchHandler = null;
            }

        }

        public class TouchListener : GestureDetector.SimpleOnGestureListener, View.IOnTouchListener
        {
            #region SWIPES

            private static int SWIPE_THRESHOLD = 100;
            private static int SWIPE_VELOCITY_THRESHOLD = 100;

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                //return base.OnFling(e1, e2, velocityX, velocityY);


                //System.Diagnostics.Debug.WriteLine($"[TOUCH] Android Flee: {e1.Action} {e2.Action}");

                var result = false;
                try
                {
                    float diffY = e2.GetY() - e1.GetY();
                    float diffX = e2.GetX() - e1.GetX();
                    if (Math.Abs(diffX) > Math.Abs(diffY))
                    {
                        if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                        {
                            if (diffX > 0)
                            {
                                onSwipeRight();
                            }
                            else
                            {
                                onSwipeLeft();
                            }
                            result = true;
                        }
                    }
                    else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (diffY > 0)
                        {
                            onSwipeBottom();
                        }
                        else
                        {
                            onSwipeTop();
                        }
                        result = true;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                return result;
            }

            private TouchActionType _swipe;

            public virtual void onSwipeRight()
            {
                _swipe = TouchActionType.SwipeRight;
            }

            public virtual void onSwipeLeft()
            {
                _swipe = TouchActionType.SwipeLeft;
            }

            public virtual void onSwipeTop()
            {
                _swipe = TouchActionType.SwipeTop;
            }

            public virtual void onSwipeBottom()
            {
                _swipe = TouchActionType.SwipeBottom;
            }

            #endregion


            public TouchListener(Context ctx)
            {
                swipeDetector = new GestureDetector(ctx, this);
            }

            private GestureDetector swipeDetector;
            void LockInput(View sender)
            {
                sender.Parent?.RequestDisallowInterceptTouchEvent(true);
                //    Debug.WriteLine($"[****MODE2*] LOCKED");
            }

            void UnlockInput(View sender)
            {
                sender.Parent?.RequestDisallowInterceptTouchEvent(false);
                //      Debug.WriteLine($"[****MODE2*] UN-LOCKED");
            }

            TouchEffect GetParent(View view)
            {
                return viewDictionary[view];
            }



            public bool OnTouch(View sender, MotionEvent motionEvent)
            {
                var _parent = GetParent(sender);

                if (_parent.FormsEffect.TouchMode == TouchHandlingStyle.Disabled)
                    return false;

                // Two object common to all the events
                Android.Views.View senderView = sender as Android.Views.View;

                // Get the pointer index
                int pointerIndex = motionEvent.ActionIndex;

                // Get the id that identifies a finger over the course of its progress
                int id = motionEvent.GetPointerId(pointerIndex);


                senderView.GetLocationOnScreen(_parent.twoIntArray);

                Point screenPointerCoords = new Point(_parent.twoIntArray[0] + motionEvent.GetX(pointerIndex),
                    _parent.twoIntArray[1] + motionEvent.GetY(pointerIndex));

                //System.Diagnostics.Debug.WriteLine($"[TOUCH] Android: {motionEvent}");

                try
                {
                    _swipe = TouchActionType.Cancelled;

                    var swiped = swipeDetector.OnTouchEvent(motionEvent);
                    var canRemove = true && !swiped;

                    switch (motionEvent.ActionMasked)
                    {
                    case MotionEventActions.Down:
                    case MotionEventActions.PointerDown:

                    if (_parent.FormsEffect.TouchMode == TouchHandlingStyle.Lock)
                        LockInput(sender);
                    else
                        UnlockInput(sender);

                    _parent.FireEvent(_parent, id, TouchActionType.Pressed,
                        screenPointerCoords, true);

                    idToEffectDictionary[id] = _parent;

                    _parent.capture = _parent.FormsEffect.Capture;
                    break;

                    case MotionEventActions.Move:

                    if (_parent.FormsEffect.TouchMode == TouchHandlingStyle.Lock)
                        LockInput(sender);
                    else
                        UnlockInput(sender);

                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (_parent.capture)
                        {
                            senderView.GetLocationOnScreen(_parent.twoIntArray);

                            screenPointerCoords = new Point(
                                _parent.twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                _parent.twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            _parent.FireEvent(_parent, id, TouchActionType.Moved, screenPointerCoords,
                                true);
                        }
                        else
                        {
                            _parent.CheckForBoundaryHop(id, screenPointerCoords);

                            if (idToEffectDictionary[id] != null)
                            {
                                _parent.FireEvent(idToEffectDictionary[id], id, TouchActionType.Moved,
                                    screenPointerCoords, true);
                            }
                            else
                            {
                                if (_parent.capture)
                                {
                                    //lost capture
                                    _parent.FireEvent(idToEffectDictionary[id], id,
                                        TouchActionType.Cancelled, screenPointerCoords, true);
                                }
                            }
                        }
                    }

                    break;

                    case MotionEventActions.Up:
                    case MotionEventActions.Pointer1Up:

                    UnlockInput(sender);


                    if (_parent.capture)
                    {
                        _parent.FireEvent(_parent, id, TouchActionType.Released, screenPointerCoords,
                            false);
                    }
                    else
                    {
                        _parent.CheckForBoundaryHop(id, screenPointerCoords);

                        if (idToEffectDictionary[id] != null)
                        {
                            _parent.FireEvent(idToEffectDictionary[id], id, TouchActionType.Released,
                                screenPointerCoords, false);
                        }
                    }

                    if (canRemove)
                        idToEffectDictionary.Remove(id);
                    break;
                    case MotionEventActions.Cancel:

                    //Debug.WriteLine($"[TOUCH] Android native: {motionEvent.ActionMasked} - {_parent.capture}");

                    UnlockInput(sender);

                    if (_parent.capture)
                    {
                        _parent.FireEvent(_parent, id, TouchActionType.Cancelled, screenPointerCoords,
                            false);
                    }
                    else
                    {
                        if (idToEffectDictionary[id] != null)
                        {
                            _parent.FireEvent(idToEffectDictionary[id], id, TouchActionType.Cancelled,
                                screenPointerCoords, false);
                        }
                    }

                    if (canRemove)
                        idToEffectDictionary.Remove(id);
                    break;

                    default:

                    UnlockInput(sender);

                    break;

                    }

                    if (swiped)
                    {
                        _parent.FireEvent(idToEffectDictionary[id], id,
                            _swipe, screenPointerCoords, false);

                        idToEffectDictionary.Remove(id);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"[TOUCH] Android Error: {motionEvent} - {e}");
                }

                return true;
            }
        }

        void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            TouchEffect touchEffectHit = null;

            foreach (Android.Views.View view in viewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(twoIntArray);
                }
                catch // System.ObjectDisposedException: Cannot access a disposed object.
                {
                    continue;
                }
                Rectangle viewRect = new Rectangle(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = viewDictionary[view];
                }
            }

            if (touchEffectHit != idToEffectDictionary[id])
            {
                if (idToEffectDictionary[id] != null)
                {
                    FireEvent(idToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }
                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }
                idToEffectDictionary[id] = touchEffectHit;
            }
        }

        void FireEvent(TouchEffect touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            Debug.WriteLine($"[TOUCH] Android fire: {actionType} - {isInContact}");

            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.FormsEffect.OnTouchAction;

            // Get the location of the pointer within the view
            touchEffect.view.GetLocationOnScreen(twoIntArray);
            double x = pointerLocation.X - twoIntArray[0];
            double y = pointerLocation.Y - twoIntArray[1];
            Point point = new Point(fromPixels(x), fromPixels(y));

            // Call the method
            onTouchAction(touchEffect.Element,
                new TouchActionEventArgs(id, actionType, point, isInContact, Element.BindingContext));
        }



    }
}