using UnityEngine;


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

}

public class Zoom0Behaviour : CameraBehaviour
{

    public Zoom0Behaviour(PathCameraSettings settings) : base(settings)
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
        return new Zoom1Behaviour(settings);
    }

    public override CameraBehaviour ZoomOut(Transform pathElements)
    {
        //EMPTY
        return this;
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
    public Zoom2Behaviour(PathCameraSettings settings)
        : base(settings)
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

        return new Zoom1Behaviour(settings);
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