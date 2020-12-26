using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class SimpleCarController : Agent
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public Camera cam;
    NavMeshPath path;
    public GameObject playerShadow, destination;
    public LineRenderer line;

    private List<Vector3> point;
    

    private void LateUpdate()
    {
        cam.transform.position = new Vector3(this.transform.position.x, 10f, this.transform.position.z);
    }

    Rigidbody rBody;
    Vector3 startingLoc;
    Quaternion startingRot;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.centerOfMass = Vector3.zero;
        path = new NavMeshPath();
        startingLoc = this.transform.localPosition;
        startingRot = this.transform.rotation;
    }


    float previousDistance = 99999999f;
    float currDistance;
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = startingLoc;
        this.transform.rotation = startingRot;
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        playerShadow.transform.position = this.transform.position;
        NavMesh.CalculatePath(this.transform.position, destination.transform.position, NavMesh.AllAreas, path);
        previousDistance = currDistance = Length();
        //agent.SetDestination(destination.transform.position);
        //Debug.Log("New Cycle"+ Length());
        //while (!agent.pathPending){
        //  Debug.Log("Pathpending");
        //}
    }

    float Length()
    {
        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        while (i < path.corners.Length)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
        }
        return lengthSoFar;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent localPositions 
        if (currDistance <= previousDistance)
        {
            sensor.AddObservation(1);
        }
        else
        {
            sensor.AddObservation(-1);
        }

        //To get local velocity independent of 3 d geometry
        var localVelocity = this.transform.InverseTransformDirection(rBody.velocity);
        sensor.AddObservation(localVelocity);
    }


    public override void OnActionReceived(float[] vectorAction)
    {
        float motor = maxMotorTorque * vectorAction[0];
        float steering = maxSteeringAngle * vectorAction[1];
        NavMesh.CalculatePath(this.transform.position, destination.transform.position, NavMesh.AllAreas, path);
        currDistance = Length();
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }

        var localVelocity = this.transform.InverseTransformDirection(rBody.velocity);
        var forwardSpeed = localVelocity.z;

        if (forwardSpeed > 0 && currDistance < previousDistance)
        {
            
            //Debug.Log("SSSSSSSSSSSSSSSSSSSS    "+ forwardSpeed);
            previousDistance = currDistance;
            AddReward(0.15f);
        }
        else
        {
            AddReward(-0.1f);
        }
        if (currDistance < 2)
        {
            //Debug.Log(distance);
            //agent.isStopped = true;
            SetReward(10);
            //Debug.Log("ending");
            EndEpisode();
        }
        line.positionCount = path.corners.Length;
        line.SetPositions(path.corners);
        playerShadow.transform.position = this.transform.position;

    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided");
        if (collision.collider.tag == "Obstacle")
        {
            //Debug.Log(collision.collider.name);
            //            movement.enabled = false; 
            //Debug.Log("Obstacle");
            SetReward(-5);
            EndEpisode();
        }
    }

    public override void Heuristic(float[] action)
    {
        action[0] = Input.GetAxis("Vertical");
        action[1] = Input.GetAxis("Horizontal");
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}