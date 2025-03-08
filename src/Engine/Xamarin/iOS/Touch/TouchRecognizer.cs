using AppoMobi.Framework.Forms.Touch;
using AppoMobi.Framework.Forms.UI.Touch;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace AppoMobi.Framework.iOS.Touch
{

    class TouchRecognizer : UIGestureRecognizer
    {

        Element element;        // Forms element for firing events
        UIView view;            // iOS UIView 
        Forms.Touch.TouchEffect touchEffect;
        bool capture;

        static Dictionary<UIView, TouchRecognizer> viewDictionary =
            new Dictionary<UIView, TouchRecognizer>();

        static Dictionary<long, TouchRecognizer> idToTouchDictionary =
            new Dictionary<long, TouchRecognizer>();



        public TouchRecognizer(Element element, UIView view, Forms.Touch.TouchEffect touchEffect)
        {
            //ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) =>
            //{
            //    return true;
            //};
            //ShouldBeRequiredToFailBy = (recognizer, uiGestureRecognizer) => false;

            this.element = element;
            this.view = view;
            this.touchEffect = touchEffect;

            viewDictionary.Add(view, this);
        }

        private bool ShouldRecLocked(UIGestureRecognizer gesturerecognizer, UIGestureRecognizer othergesturerecognizer)
        {
            return true;
        }
        private bool ShouldRecUnlocked(UIGestureRecognizer gesturerecognizer, UIGestureRecognizer othergesturerecognizer)
        {
            return false;
        }

        private bool ShouldFailLocked(UIGestureRecognizer gesturerecognizer, UIGestureRecognizer othergesturerecognizer)
        {
            return false;
        }

        void ShareTouch()
        {
            ShouldBeRequiredToFailBy = ShouldRecUnlocked;
            ShouldRecognizeSimultaneously = ShouldRecLocked;
        }
        void LockTouch()
        {
            ShouldBeRequiredToFailBy = ShouldFailLocked;
            ShouldRecognizeSimultaneously = ShouldRecLocked;

            //if (UIDevice.CurrentDevice.CheckSystemVersion(13, 4))
            //{
            //    ShouldReceiveEvent = ShouldEvent;
            //}

            //  Debug.WriteLine("[TOUCH] LOCKED!");
        }

        private bool ShouldEvent(UIGestureRecognizer gesturerecognizer, UIEvent @event)
        {
            return true;
        }

        void UnlockTouch()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 4))
            {
                ShouldReceiveEvent = ShouldEvent;
            }

            ShouldBeRequiredToFailBy = null;
            ShouldRecognizeSimultaneously = null;

            //  Debug.WriteLine("[TOUCH] UNlocked!");
        }

        public void Detach()
        {
            viewDictionary.Remove(view);
        }

        // touches = touches of interest; evt = all touches of type UITouch
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();
                FireEvent(this, id, TouchActionType.Pressed, touch, true);

                if (!idToTouchDictionary.ContainsKey(id))
                {
                    idToTouchDictionary.Add(id, this);
                }
            }

            // Save the setting of the Capture property
            capture = touchEffect.Capture;

            if (touchEffect.TouchMode == TouchHandlingStyle.Lock)
            {
                LockTouch();
            }
            else
            if (touchEffect.TouchMode == TouchHandlingStyle.Share)
            {
                ShareTouch();
            }
            else
            {
                UnlockTouch();
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Moved, touch, true);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Moved, touch, true);
                    }
                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);


            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                CGPoint cgPoint = touch.LocationInView(this.View);
                Point xfPoint = new Point(cgPoint.X, cgPoint.Y);
                bool isInside = CheckPointIsInsideRecognizer(xfPoint, this);

                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    Debug.WriteLine($"[TOUCH] TouchesEnded!");
                    if (isInside)
                        FireEvent(this, id, TouchActionType.Released, touch, false);
                    else
                        FireEvent(this, id, TouchActionType.Exited, touch, false);

                    UnlockTouch();
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                    {
                        Debug.WriteLine($"[TOUCH] TouchesEnded 2!");
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Released, touch, false);
                    }
                }
                idToTouchDictionary.Remove(id);
            }

        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Cancelled, touch, false);
                }
                else if (idToTouchDictionary[id] != null)
                {
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Cancelled, touch, false);
                }
                idToTouchDictionary.Remove(id);
            }

            UnlockTouch();

        }

        public override bool CancelsTouchesInView
        {
            get
            {
                return false; //todo
            }
        }

        void CheckForBoundaryHop(UITouch touch)
        {
            long id = touch.Handle.ToInt64();

            // TODO: Might require converting to a List for multiple hits
            TouchRecognizer recognizerHit = null;

            foreach (UIView view in viewDictionary.Keys)
            {
                CGPoint location = touch.LocationInView(view);

                if (new CGRect(new CGPoint(), view.Frame.Size).Contains(location))
                {
                    recognizerHit = viewDictionary[view];
                }
            }
            if (recognizerHit != idToTouchDictionary[id])
            {
                if (idToTouchDictionary[id] != null)
                {
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Exited, touch, true);
                }
                if (recognizerHit != null)
                {
                    FireEvent(recognizerHit, id, TouchActionType.Entered, touch, true);
                }
                idToTouchDictionary[id] = recognizerHit;
            }
        }

        bool CheckPointIsInsideRecognizer(Point xfPoint, TouchRecognizer recognizer)
        {
            if (xfPoint.Y < 0 || xfPoint.Y > recognizer.View.Bounds.Height)
            {
                return false;
            }

            if (xfPoint.X < 0 || xfPoint.X > recognizer.View.Bounds.Width)
            {
                return false;
            }

            return true;
        }

        void FireEvent(TouchRecognizer recognizer, long id, TouchActionType actionType, UITouch touch, bool isInContact)
        {
            // Convert touch location to Xamarin.Forms Point value
            CGPoint cgPoint = touch.LocationInView(recognizer.View);
            Point xfPoint = new Point(cgPoint.X, cgPoint.Y);

            if (xfPoint.Y < 0 || xfPoint.Y > recognizer.View.Bounds.Height)
            {
                Debug.WriteLine("[TOUCH] Y OUT!!!");
                Debug.WriteLine($"[TOUCH] {actionType}");
            }

            if (xfPoint.X < 0 || xfPoint.X > recognizer.View.Bounds.Width)
            {
                Debug.WriteLine("[TOUCH] X OUT!!!");
                Debug.WriteLine($"[TOUCH] {actionType}");
            }


            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = recognizer.touchEffect.OnTouchAction;

            // Call that method
            onTouchAction(recognizer.element,
                new TouchActionEventArgs(id, actionType, xfPoint, isInContact, element.BindingContext));
        }
    }
}