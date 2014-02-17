using UnityEngine;
using System.Collections;

public abstract class CameraBehaviour : MonoBehaviour
{
    protected readonly Transform camera;
    protected readonly float outZoomDistance;

    protected CameraBehaviour(Transform camera, float outZoomDistance)
    {
        this.camera = camera;
        this.outZoomDistance = outZoomDistance;
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

}

public class Zoom0Behaviour : CameraBehaviour
{
    public Zoom0Behaviour(Transform camera, float outZoomDistance)
        : base(camera, outZoomDistance)
    {
    }

    public override void Pan(Vector2 obj)
    {
        //TODO
    }

    public override void Swipe(Transform obj)
    {
        //Empty method
    }

    public override CameraBehaviour ZoomIn(Transform pathElements)
    {
        return new Zoom1Behaviour(camera, outZoomDistance);
    }

    public override CameraBehaviour ZoomOut(Transform pathElements)
    {
        //EMPTY
        return this;
    }
}


public class Zoom1Behaviour : CameraBehaviour
{
    public Zoom1Behaviour(Transform camera, float outZoomDistance)
        : base(camera, outZoomDistance)
    {
    }

    public override void Pan(Vector2 obj)
    {
        //EMPTY FUNCTION
    }


    public override CameraBehaviour ZoomIn(Transform pathElements)
    {
        //TODO
        
        
        bool IsImage = true;
        if (IsImage)
            return new Zoom2Behaviour(camera, outZoomDistance);
        else
            return new Zoom2ModelBehavior(camera, outZoomDistance);
    }

    public override CameraBehaviour ZoomOut(Transform pathElements)
    {
        //TODO


        return new Zoom0Behaviour(camera, outZoomDistance);
    }
}

public class Zoom2Behaviour : CameraBehaviour
{
    public Zoom2Behaviour(Transform camera, float outZoomDistance)
        : base(camera, outZoomDistance)
    {
    }

    public override void Pan(Vector2 obj)
    {
        //TODO
    }

    public override CameraBehaviour ZoomIn(Transform pathElements)
    {
        //EMPTY
        return this;
    }

    public override CameraBehaviour ZoomOut(Transform pathElements)
    {
        //TODO

        return new Zoom1Behaviour(camera, outZoomDistance);
    }
}

public class Zoom2ModelBehavior : Zoom2Behaviour
{
    public Zoom2ModelBehavior(Transform camera, float outZoomDistance)
        : base(camera, outZoomDistance)
    {
    }

    public override void Pan(Vector2 obj)
    {
        //EMPTY
    }
}