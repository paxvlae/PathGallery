using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

//ZOOM1
//ZOOM2

class UserMovesInterpreter
{
    public event Action<Vector2> panGesture;
    public event Action zoomIn;
    public event Action zoomOut;
    public event Action<int> swipe;

    public void Update()
    {
        throw new NotImplementedException();
    }
}

//PAXVLAE

public class PathCameraController : MonoBehaviour
{

    private Transform[] pathElementsList;
    private int actualElementIndex;
    public float OutZoomDistance;
    private UserMovesInterpreter Interpreter;
    private CameraBehaviour cameraBehaviour;
    
    // Use this for initialization
	void Start () {
        var gallerySlotsUnorder = GameObject.FindGameObjectsWithTag("PathElement");
	    pathElementsList = gallerySlotsUnorder.OrderBy(x => x.name.Split('_')[1]).Select(x => x.transform).ToArray();
	    actualElementIndex = 0;
	    Interpreter = new UserMovesInterpreter();
        cameraBehaviour = new Zoom0Behaviour(transform, OutZoomDistance);
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
