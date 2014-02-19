using System;
using System.Collections.Generic;
using System.Linq;
using TouchScript;
using TouchScript.Events;
using UnityEngine;

[System.Serializable]
public class TouchSettings
{
    public float percentageSwipeArea;
    public float zoomInDistance;
    public float zoomOutDistance;
    public float swipeDistance;
    public float swipeAngle;
}
class UserMovesInterpreter
{
    private readonly TouchSettings settings;
    public event Action<Vector2> panGesture;
    public event Action zoomIn;
    public event Action zoomOut;
    public event Action<int> swipe;
    class TouchWithStartPosition
    {
        public TouchWithStartPosition(TouchPoint touch, Vector2 startPos)
        {
            this.Touch = touch;
            this.StartPos = startPos;
        }

        public TouchPoint Touch;
        public Vector2 StartPos;
        public Vector2 MoveVector { get { return Touch.Position - StartPos; }}
        public float MoveSqrtMagnitude { get { return MoveVector.sqrMagnitude; } }
    }

    interface ITouchInterpreter
    {
        void TouchBegan(List<TouchPoint> list);
        void TouchEnded(List<TouchPoint> list);
        void TouchMoved(List<TouchPoint> list);
    }
    class SwipeInterpreter : ITouchInterpreter
    {
        private readonly Vector2 idealSwipeVector;
        private readonly float maxAngle;
        private readonly float swipeDistance;
        private readonly Rect rect;
        private List<TouchWithStartPosition> activeTouches = new List<TouchWithStartPosition>();
        public event Action Swipe;


        public SwipeInterpreter(Vector2 idealSwipeVector, float maxAngle, float swipeDistance, Rect rect)
        {
            this.idealSwipeVector = idealSwipeVector;
            this.maxAngle = maxAngle;
            this.swipeDistance = swipeDistance * swipeDistance;
            this.rect = rect;
        }

        public void TouchBegan(List<TouchPoint> list)
        {
            activeTouches.AddRange(
                list.Where(x => rect.Contains(VectorUtility.InvertY((Vector2) x.Position)))
                    .Select(x => new TouchWithStartPosition(x, x.Position)));
        }

        public void TouchEnded(List<TouchPoint> list)
        {
            activeTouches.RemoveAll(x => list.Contains(x.Touch));

        }

        public void TouchMoved(List<TouchPoint> list)
        {
            RemoveMissMoves();
            if (
                activeTouches.Where(x => list.Contains(x.Touch))
                    .Any(x => (x.Touch.Position - x.StartPos).sqrMagnitude > swipeDistance))
            {
                activeTouches.Clear();
                if (Swipe != null)
                    Swipe();
            }
        }

        private void RemoveMissMoves()
        {
            activeTouches.RemoveAll(x =>
            {   
                var moveVect = x.MoveVector;
                if (moveVect.sqrMagnitude > SwipeStartTreshold)
                {
                    var angle = Vector2.Angle(moveVect, idealSwipeVector);
                    //Debug.Log(x.Touch.Id + " ang: " + angle + " MV: " + moveVect + " id: " + idealSwipeVector);
                    return angle > maxAngle;    
                }
                return false;

            });
        }

        public const float SwipeStartTreshold = 4f;

        public void OnGUI()
        {
            GUI.Button(rect, " ");
        }
    }

    class PinchInterpreter : ITouchInterpreter
    {
        public Action ZoomIn;
        public Action ZoomOut;
        private readonly Rect pinchRect;
        private readonly float sqrtZoomInDistance;
        private readonly float sqrtZoomOutDistance;
        private List<TouchWithStartPosition> activeTouches = new List<TouchWithStartPosition>();


        public PinchInterpreter(Rect pinchRect, float zoomInDistance, float zoomOutDistance)
        {
            this.pinchRect = pinchRect;
            this.sqrtZoomInDistance = zoomInDistance * zoomInDistance;
            this.sqrtZoomOutDistance = zoomOutDistance * zoomOutDistance;
        }

        public void TouchMoved(List<TouchPoint> touchPoints)
        {
            var movedTouches = activeTouches;//.Where(x => touchPoints.Contains(x.Touch)).ToArray();
            
            for (int i = 0; i < movedTouches.Count; i++)
            {
                for (int j = i + 1; j < movedTouches.Count; j++)
                {
                    var startVector = movedTouches[i].StartPos - movedTouches[j].StartPos;
                    var movedVector = movedTouches[i].Touch.Position - movedTouches[j].Touch.Position;
                    var startMagnitude = startVector.sqrMagnitude;
                    var movedMagnitude = movedVector.sqrMagnitude;
                    var pinchVectorMagnitude = (movedVector - startVector).sqrMagnitude;
                    if (pinchVectorMagnitude > sqrtZoomInDistance && startMagnitude < movedMagnitude)
                    {
                        FireZoomInEvent();
                        activeTouches.Clear();
                        return;
                    }
                    if(pinchVectorMagnitude > sqrtZoomOutDistance && startMagnitude > movedMagnitude)
                    {
                        FireZoomOutEvent();
                        activeTouches.Clear();
                        return;
                    }
                }
            }
        }

        private void FireZoomOutEvent()
        { 
            if (ZoomOut != null) ZoomOut();
        }

        private void FireZoomInEvent()
        {
            if (ZoomIn != null)
                ZoomIn();
        }

        public void TouchEnded(List<TouchPoint> list)
        {
            activeTouches.RemoveAll(x => list.Contains(x.Touch));
        }

        public void TouchBegan(List<TouchPoint> list)
        {
            activeTouches.AddRange(
                list.Where(x => pinchRect.Contains(x.Position.InvertY()))
                    .Select(x => new TouchWithStartPosition(x, x.Position)));
        }

        public void OnGUI()
        {
            GUI.Button(pinchRect, " ");
        }
    }



    class PanInterpreter : ITouchInterpreter
    {
        private readonly Rect panRect;
        public Action<Vector2> OnPan;
        private List<TouchPoint> activeTouches = new List<TouchPoint>();

        public PanInterpreter(Rect panRect)
        {
            this.panRect = panRect;
        }

        public void TouchBegan(List<TouchPoint> list)
        {
            activeTouches.AddRange(list.Where(x => panRect.Contains(x.Position.InvertY())));
        }

        public void TouchEnded(List<TouchPoint> list)
        {
            activeTouches.RemoveAll(list.Contains);

        }

        public void TouchMoved(List<TouchPoint> list)
        {
            var panTouch = (from touch in activeTouches
                            where list.Contains(touch)
                            let magnitude = (touch.Position - touch.PreviousPosition).sqrMagnitude
                            orderby magnitude descending
                            select touch).FirstOrDefault();
            if (panTouch != null)
            {
                if (OnPan != null) OnPan(panTouch.Position - panTouch.PreviousPosition);
            }

        }
    }


    private SwipeInterpreter lefSwipeInterpreter;
    private SwipeInterpreter rightSwipeInterpreter;
    private PinchInterpreter pinchInterpreter;
    private PanInterpreter panListener;

    public UserMovesInterpreter(TouchSettings settings)
    {
        this.settings = settings;
        TouchManager.Instance.TouchesBegan += TouchesBegan;
        TouchManager.Instance.TouchesCancelled += TouchesEnded;
        TouchManager.Instance.TouchesEnded += TouchesEnded;
        TouchManager.Instance.TouchesMoved += TouchesMoves;

        var borderWidth = Screen.width * settings.percentageSwipeArea / 100;
        var leftBorder = new Rect(0f, 0f, borderWidth, Screen.height);
        lefSwipeInterpreter = new SwipeInterpreter(new Vector2(1.0f, 0), settings.swipeAngle, settings.swipeDistance, leftBorder);
        lefSwipeInterpreter.Swipe += () => { if (swipe != null) swipe(-1); };

        var rightBorder = new Rect(Screen.width - borderWidth, 0f, borderWidth, Screen.height);
        rightSwipeInterpreter = new SwipeInterpreter(new Vector2(-1.0f, 0), settings.swipeAngle, settings.swipeDistance, rightBorder);
        rightSwipeInterpreter.Swipe += () => { if (swipe != null) swipe(1); };

        var pinchRect = new Rect(borderWidth, 0, Screen.width - borderWidth, Screen.height);
        pinchInterpreter = new PinchInterpreter(pinchRect,settings.zoomInDistance, settings.zoomOutDistance);
        pinchInterpreter.ZoomIn += () => { if (zoomIn != null) zoomIn(); };
        pinchInterpreter.ZoomOut += () => { if (zoomOut != null) zoomOut(); };

        panListener = new PanInterpreter(pinchRect);
        panListener.OnPan+= vector2 => { if (panGesture != null) panGesture(vector2);};
    }


    private void TouchesMoves(object sender, TouchEventArgs e)
    {
        lefSwipeInterpreter.TouchMoved(e.TouchPoints);
        rightSwipeInterpreter.TouchMoved(e.TouchPoints);
        pinchInterpreter.TouchMoved(e.TouchPoints);
        panListener.TouchMoved(e.TouchPoints);
    }

    private void TouchesEnded(object sender, TouchEventArgs touchEventArgs)
    {

        lefSwipeInterpreter.TouchEnded(touchEventArgs.TouchPoints);
        rightSwipeInterpreter.TouchEnded(touchEventArgs.TouchPoints);
        pinchInterpreter.TouchEnded(touchEventArgs.TouchPoints);
        panListener.TouchEnded(touchEventArgs.TouchPoints);
    }

    void TouchesBegan(object sender, TouchScript.Events.TouchEventArgs e)
    {
        lefSwipeInterpreter.TouchBegan(e.TouchPoints);
        rightSwipeInterpreter.TouchBegan(e.TouchPoints);
        pinchInterpreter.TouchBegan(e.TouchPoints);
        panListener.TouchBegan(e.TouchPoints);
    }

    public void Destroy()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.TouchesBegan -= TouchesBegan;
            TouchManager.Instance.TouchesCancelled -= TouchesEnded;
            TouchManager.Instance.TouchesEnded -= TouchesEnded;
            TouchManager.Instance.TouchesMoved -= TouchesMoves;
        }
    }
    public void Update()
    {

    }

    public void OnGUI()
    {
        lefSwipeInterpreter.OnGUI();
        rightSwipeInterpreter.OnGUI();
        pinchInterpreter.OnGUI();
    }
}