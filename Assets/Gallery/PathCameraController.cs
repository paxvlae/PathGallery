using System;
using System.Linq;
using UnityEngine;
using System.Collections;

//ZOOM1
//ZOOM2


[System.Serializable]
public class TouchSettings
{
    public float percentageSwipeArea;
    public float zoomInDistance;
    public float zoomOutDistance;
    public float swipeDistance;
    public float swipeAngle;
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
    void Start()
    {
        var gallerySlotsUnorder = GameObject.FindGameObjectsWithTag("PathElement");
        pathElementsList = (from element in gallerySlotsUnorder
            let number = int.Parse(element.name.Split('_')[1])
            orderby number
            select element.transform).ToArray();
        actualElementIndex = 0;
        Interpreter = new UserMovesInterpreter(touchSettings);
        CameraSettings.Camera = transform;
        cameraBehaviour = new Zoom0Behaviour(CameraSettings);
        Interpreter.panGesture += InterpreterOnPanGesture;
        Interpreter.swipe += InterpreterOnSwipe;
        Interpreter.zoomIn += InterpreterOnZoomIn;
        Interpreter.zoomOut += InterpreterOnZoomOut;
    }

    private void InterpreterOnPanGesture(Vector2 vector2)
    {
        cameraBehaviour.Pan(vector2);
    }

    private void InterpreterOnSwipe(int i)
    {
#region DEBUG
        Debug.Log("Swipe " + i);
#endregion
        actualElementIndex += i;
        var oldVal = actualElementIndex;
        actualElementIndex = Mathf.Clamp(actualElementIndex, 0, pathElementsList.Length - 1);
        if (oldVal != actualElementIndex)
            cameraBehaviour.Swipe(pathElementsList[actualElementIndex]);

    }

    private void InterpreterOnZoomOut()
    {
        cameraBehaviour = cameraBehaviour.ZoomOut(pathElementsList[actualElementIndex]);
        #region DEBUG
            Debug.Log("Zoom out. Actual state is" + cameraBehaviour.GetType().Name); 
        #endregion
    }

    private void InterpreterOnZoomIn()
    {
        cameraBehaviour = cameraBehaviour.ZoomIn(pathElementsList[actualElementIndex]);
        #region DEBUG
            Debug.Log("Zoom in. Actual state is" + cameraBehaviour.GetType().Name);
        #endregion
    }

/* Use for debug
    void OnGUI()
    {
        Interpreter.OnGUI();
    }
*/
    void Update()
    {
        //  Interpreter.Update();
    }
    void OnDestroy()
    {
        Interpreter.Destroy();
    }
}
