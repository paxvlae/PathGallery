using UnityEngine;
using System.Collections;

public class PlaneTwirl : MonoBehaviour
{

    public float maxHorizontalAngle;
    public float maxVerticalAngle;
    public float horizontalVelocity;
    public float verticalVelocity;

    private float targetHorizontalAngle;
    private float targetVerticalAngle;

    private float actualHorizontalAngle;
    private float actualVerticalAngle;


    private Vector3 horizontalVector;
    private Vector3 verticalVector;


	// Use this for initialization
	void Start ()
	{
	    horizontalVector = transform.up;
	    verticalVector = transform.right;
	    GetNewHorizontalTarget();
	}

    private void GetNewHorizontalTarget()
    {
        targetHorizontalAngle = Random.Range(-maxHorizontalAngle, 0)*Mathf.Sign(targetHorizontalAngle);
        actualHorizontalAngle = 0;
    }

    // Update is called once per frame
	void Update () {
	    if (Mathf.Abs(targetHorizontalAngle - actualHorizontalAngle) > 2)
	    {
	        float angle = horizontalVelocity*Time.deltaTime * Mathf.Sign(targetHorizontalAngle);
	        actualHorizontalAngle += angle;
            transform.Rotate(horizontalVector, angle);
	    }
	    else
	    {
	        GetNewHorizontalTarget();
	    }
	}
}
