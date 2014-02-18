using UnityEngine;
using System.Collections;
using System;
using TouchScript;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// This class listen for touches occurring in given rect.
/// Class use touch script to detect touches.
/// If you are interested in listening kind of event simply register your delegate 
/// </summary>
/// <example>
/// TouchInputListener input = new TouchInputListener(realRect);
/// input.onTouchRealesed += ButtonRealesed;
/// input.onTouchEnter +=  ButtonHover;
/// input.onTouchCancel += ButtonReturnToNormal;
/// input.ChangeTouchRect(newRect);
/// input.Enable();
/// </example>
public class TouchInputListener 
{
#region DebugInterface
    public void DrawDebug()
	{	
		if(isActive){
            String tekst = name ?? "Touch listener";
            int oldDepth = GUI.depth;
            GUI.depth = -100;
            GUI.Button(touchRect, tekst);
            GUI.depth = oldDepth;
        }
    }
#endregion

#region LocalVariables
    TouchPoint point;
    Rect touchRect;
    bool isActive;
#endregion

#region PublicInterface
    public bool TouchSlidingInEnabled = false;
    public TouchInputListener(Rect touchRect, string name) : this(touchRect, true)
    {
        this.name = name;
    }

    public TouchInputListener(Rect? touchRect = null, bool isActive = true)
    {
        this.touchRect = touchRect ?? new Rect();
        this.isActive = isActive;
        if(isActive)
            RegisterToTouchScriptEvents();
    }

    /// <summary>
    /// This method allow you to change obserated rect.
    /// If there is a touch in old rect and is out of new rect there will be generated Cencel event
    /// </summary>
    /// <param name="newRect"> Give new rect in which you will observe</param>
    public void ChangeTouchRect(Rect newRect)
    {
        touchRect = newRect;
        if (isTouchCanceled())
        {
            SendCancelEvent(point.Position);
        }
    }

    public bool IsActive 
    {
        get
        {
            return isActive;
        } 
    }

    public void     Enable()
    {
        if (!isActive)
        {
            RegisterToTouchScriptEvents();
            isActive = true;
        }
    }

    public void Disable()
    {
        if (isActive)
        {
             
            UnRegisterFromTouchScriptEvents();
            isActive = false;
        }
    }

    public void Destroy()
    {
        UnRegisterFromTouchScriptEvents();
    }
    public bool AllowLeavingTouchRect = false;
#endregion

#region  EventUtilities
    public event Action          onTouchEnter;
    public event Action          onTouchRealesed;
    public event Action          onTouchCancel;
    public event Action<Vector2> onTouchEnterWithPos;
    public event Action<Vector2> onTouchRealesedWithPos;
    public event Action<Vector2> onTouchCancelWithPos;
    /// <summary>
    /// Event send one arg which is drag offset
    /// </summary>
    public event Action<Vector2> onDrag;
    /// <summary>
    /// Event send two arguments where first is position where touch ended and second is offset
    /// </summary>
    public event Action<Vector2, Vector2> onDragWithPos;
    private string name;
    private Rect workingRect;

    private void SendCancelEvent( Vector2 position)
    {
        if (IsActive)
        {
            if (onTouchCancel != null)
                onTouchCancel();
            if (onTouchCancelWithPos != null)
                onTouchCancelWithPos(position);
        }
    }

    private void SendRealeseEvent(Vector2 pos)
    {
        if (IsActive)
        {
            if (onTouchRealesed != null)
                onTouchRealesed();
      
            if (onTouchRealesedWithPos != null)
                onTouchRealesedWithPos(pos);
        }
    }
    
    private void SendEnterEvent(Vector2 pos)
    {
        if (IsActive)
        {
            if (onTouchEnter != null)
                onTouchEnter();
            if (onTouchEnterWithPos != null)
                onTouchEnterWithPos(pos);
        }
    }
    private void SendDragEvent(Vector2 pos, Vector2 offset)
    {
        if (IsActive)
        {
            if (onDrag != null)
                onDrag(offset);
            if (onDragWithPos != null)
                onDragWithPos(pos, offset);
        }
    }
#endregion

#region PrivateMethods
    private void RegisterToTouchScriptEvents()
    {
        if(TouchScript.TouchManager.Instance != null)
        { 
            TouchScript.TouchManager.Instance.TouchesBegan += Instance_TouchesBegan;
            TouchScript.TouchManager.Instance.TouchesEnded += Instance_TouchesEnded;
            TouchScript.TouchManager.Instance.TouchesMoved += Instance_TouchesMoved;
            TouchScript.TouchManager.Instance.TouchesCancelled += Instance_TouchesCancelled;
        }
    }

    private void UnRegisterFromTouchScriptEvents()
    {
        if (TouchScript.TouchManager.Instance != null)
        {
            TouchScript.TouchManager.Instance.TouchesBegan -= Instance_TouchesBegan;
            TouchScript.TouchManager.Instance.TouchesEnded -= Instance_TouchesEnded;
            TouchScript.TouchManager.Instance.TouchesMoved -= Instance_TouchesMoved;
            TouchScript.TouchManager.Instance.TouchesCancelled -= Instance_TouchesCancelled;
        }
    }
    void Instance_TouchesCancelled(object sender, TouchScript.Events.TouchEventArgs e)
    {
        if (e.TouchPoints.Contains(point))
            SendCancelEvent(point.Position);
    }

    void Instance_TouchesMoved(object sender, TouchScript.Events.TouchEventArgs e)
    {
        if (point != null)
        {
            if (e.TouchPoints.Contains(point))
            {
                CheckMovedTouch();
            }
        }
        else
        {
            if(TouchSlidingInEnabled)
            {
                point = GetInsideTouchFromList(e.TouchPoints);
                if (point != null)
                    SendEnterEvent(point.Position);
            }
        }
    }

    private void CheckMovedTouch()
    {
        if (isTouchCanceled())
        {
            SendCancelEvent(point.Position);
            point = null;
        }
        else
        {
            Vector2 offset = point.PreviousPosition - point.Position;
            SendDragEvent(point.Position, offset);
        }
    }

    void Instance_TouchesEnded(object sender, TouchScript.Events.TouchEventArgs e)
    {
        if (point != null && e.TouchPoints.Contains(point))
        {
            SendRealeseEvent(point.Position);
            point = null;
        }
    }

    void Instance_TouchesBegan(object sender, TouchScript.Events.TouchEventArgs e)
    {
        if (point == null )
        {
            point = GetInsideTouchFromList(e.TouchPoints);
            if(point != null)
                SendEnterEvent(point.Position);
        }
 
    }

    private bool isTouchCanceled()
    {
        if (point != null)
            return !touchRect.Contains(point.Position.InvertY()) && !AllowLeavingTouchRect;
        else
            return false;
    }

    private TouchPoint GetInsideTouchFromList(List<TouchPoint> points)
    {
        return points.Where(x => touchRect.Contains(x.Position.InvertY())).FirstOrDefault();
    }

#endregion


    public bool HasActiveTouch()
    {
        return point != null;
    }

    public Vector3 ActiveTouchPosition { 
        get 
        {
            if (point != null)
                return new Vector3(point.Position.x, point.Position.y, 0);
            else
                return Vector3.zero;
        } 
    }
}
