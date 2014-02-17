using System;
using System.Collections.Generic;
using System.Linq;
using TouchScript;
using TouchScript.Events;
using UnityEngine;
using System.Collections;

//ZOOM1
//ZOOM2


[System.Serializable]
public class TouchSettings
{
    public float sensitiv;
    public float percentageSwipeArea;
}




class UserMovesInterpreter
{
    private readonly TouchSettings settings;
    public event Action<Vector2> panGesture;
    public event Action zoomIn;
    public event Action zoomOut;
    public event Action<int> swipe;


    public UserMovesInterpreter(TouchSettings settings)
    {
        this.settings = settings;
        TouchManager.Instance.TouchesBegan += TouchesBegan;
        TouchManager.Instance.TouchesCancelled += TouchesEnded;
        TouchManager.Instance.TouchesEnded += TouchesEnded;
        TouchManager.Instance.TouchesMoved += TouchesMoves;

    }

    private void TouchesMoves(object sender, TouchEventArgs e)
    {
        
    }

    private void TouchesEnded(object sender, TouchEventArgs touchEventArgs)
    {
        
    }

    void TouchesBegan(object sender, TouchScript.Events.TouchEventArgs e)
    {
        
    }

    public void Update()
    {

    }
}


public class PathCameraController : MonoBehaviour
{
    public TouchSettings touchSettings;
    public PathCameraSettings CameraSettings;

    private Transform[] pathElementsList;
    private int actualElementIndex;
    private UserMovesInterpreter Interpreter;
    private CameraBehaviour cameraBehaviour;
    
    // Use this for initialization
	void Start () {
        var gallerySlotsUnorder = GameObject.FindGameObjectsWithTag("PathElement");
	    pathElementsList = gallerySlotsUnorder.OrderBy(x => int.Parse(x.name.Split('_')[1])).Select(x => x.transform).ToArray();
	    actualElementIndex = 0;
	    Interpreter = new UserMovesInterpreter(touchSettings);
	    CameraSettings.Camera = transform;
        cameraBehaviour = new Zoom0Behaviour(CameraSettings);
        Interpreter.panGesture += cameraBehaviour.Pan;
	    Interpreter.swipe += InterpreterOnSwipe;
	    Interpreter.zoomIn += InterpreterOnZoomIn;
	    Interpreter.zoomOut += InterpreterOnZoomOut;
	}

    private void InterpreterOnSwipe(int i)
    {
        actualElementIndex += i;
        var oldVal = actualElementIndex;
        actualElementIndex = Mathf.Clamp(actualElementIndex, 0, pathElementsList.Length - 1);
        if(oldVal != actualElementIndex)
            cameraBehaviour.Swipe(pathElementsList[actualElementIndex]);

    }

    private void InterpreterOnZoomOut()
    {
        cameraBehaviour = cameraBehaviour.ZoomOut(pathElementsList[actualElementIndex]);
        
    }

    private void InterpreterOnZoomIn()
    {
        cameraBehaviour = cameraBehaviour.ZoomIn(pathElementsList[actualElementIndex]);
    }
    void Update()
    {
        Interpreter.Update();
    }
}
