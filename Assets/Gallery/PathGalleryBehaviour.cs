using Assets.Scripts.UtilityScripts;
using UnityEngine;

[System.Serializable]
public class PathCameraSettings
{
    public float OutZoomDistance;
    [System.NonSerialized]
    public Transform Camera;
    public float Zoom0PanSensitive;
    public float Zoom2PanSensitive;
}
public abstract class CameraBehaviour
{
    protected readonly PathCameraSettings settings;

    protected CameraBehaviour(PathCameraSettings settings)
    {
        this.settings = settings;
    }

    public virtual void Pan(Vector2 obj)
    {
        //EMPTY FUNCTION
    }

    public virtual void Swipe(Transform obj)
    {
        //TODO: Implement here swipe
    }

    public abstract CameraBehaviour ZoomIn(Transform pathElements);

    public abstract CameraBehaviour ZoomOut(Transform pathElements);

    public virtual void Update()
    {
        
    }
}

public class Zoom0Behaviour : CameraBehaviour
{

    private InertialMove<Vector3> panMove;
    const float epsilon = 0.01f * 0.01f;
    public Zoom0Behaviour(PathCameraSettings settings) : base(settings)
    {
        panMove = new InertialMove<Vector3>(suppressMoveFunc: x => x*0.3f, 
                                            sumFunc: (x, y) => x + y, 
                                            isGreaterThenEpsilon: x => x.sqrMagnitude > epsilon, 
                                            zeroValue: Vector3.zero);
    }

    public override void Pan(Vector2 obj)
    {
        panMove.AddMove(new Vector3(obj.x, obj.y, 0));
    }

    public override void Swipe(Transform obj)
    {
        //Empty method
    }

    public override CameraBehaviour ZoomIn(Transform pathElements)
    {
        return new Zoom1Behaviour(settings);
    }

    public override CameraBehaviour ZoomOut(Transform pathElements)
    {
        //EMPTY
        return this;
    }

    public override void Update()
    {
        //TODO Add pan constraints here
        settings.Camera.Translate(panMove.GetMove() * settings.Zoom0PanSensitive);
    }
}


public class Zoom1Behaviour : CameraBehaviour
{
    public Zoom1Behaviour(PathCameraSettings settings)
        : base(settings)
    {
    }

    public override CameraBehaviour ZoomIn(Transform pathElements)
    {
        //TODO
        
        
        bool IsImage = true;
        if (IsImage)
            return new Zoom2Behaviour(settings);
        else
            return new Zoom2ModelBehavior(settings);
    }

    public override CameraBehaviour ZoomOut(Transform pathElements)
    {
        //TODO

        return new Zoom0Behaviour(settings);
    }
}

public class Zoom2Behaviour : CameraBehaviour
{
    private InertialMove<Vector3> panMove;
    const float epsilon = 0.1f * 0.1f;

    public Zoom2Behaviour(PathCameraSettings settings)
        : base(settings)
    {
        panMove = new InertialMove<Vector3>(suppressMoveFunc: x => x * 0.3f,
                                            sumFunc: (x, y) => x + y, 
                                            isGreaterThenEpsilon: x => x.sqrMagnitude > epsilon, 
                                            zeroValue: Vector3.zero);

    }

    public override void Pan(Vector2 obj)
    {
        panMove.AddMove(new Vector3(obj.x, obj.y, 0));
    }

    public override CameraBehaviour ZoomIn(Transform pathElements)
    {
        //EMPTY
        return this;
    }

    public override CameraBehaviour ZoomOut(Transform pathElements)
    {
        //TODO

        return new Zoom1Behaviour(settings);
    }

    public override void Update()
    {
        //TODO Add pan constraints here
        settings.Camera.Translate(panMove.GetMove() * settings.Zoom2PanSensitive);
    }
}

public class Zoom2ModelBehavior : Zoom2Behaviour
{
    public Zoom2ModelBehavior(PathCameraSettings settings)
        : base(settings)
    {
    }

    public override void Pan(Vector2 obj)
    {
        //EMPTY
    }
}