using Unity.Mathematics;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float VelocityMagnitude => _carRigidbody2D.velocity.magnitude;

    [Header("Car settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 26.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 30.0f;

    //Local variables
    private float _accelerationInput;
    private float _steeringInput;
    private float _linearDrag;
    private bool _isBreaking;
    private float _rotationAngle;
    private float _velocityVsUp;

    //Components
    Rigidbody2D _carRigidbody2D;


    //Awake is called when the script instance is being loaded.
    private void Awake()
    {
        _carRigidbody2D = GetComponent<Rigidbody2D>();
        _linearDrag = _carRigidbody2D.drag;
    }

    //Frame-rate independent for physics calculations.
    private void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    private void ApplyEngineForce()
    {
        var accelerate = accelerationFactor;
        _carRigidbody2D.drag = _linearDrag;
        _isBreaking = false;

        //Calculate how much "forward" we are going in terms of the direction of our velocity
        _velocityVsUp = Vector2.Dot(transform.up, _carRigidbody2D.velocity);

        //Limit so we cannot go faster than the max speed in the "forward" direction

        if (_accelerationInput > 0)
        {
            //if (velocityVsUp < -1)
            //{
            //    Break();
            //    return;
            //}
            //Limit so we cannot go faster than the 50% of max speed in the "reverse" direction
            if (_velocityVsUp > maxSpeed)
            {
                return;
            }
        }




        //Limit so we cannot go faster in any direction while accelerating
        if (_carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && _accelerationInput > 0)
            return;

        if (_accelerationInput < 0)
        {
            accelerate /= 2f;
            if (_velocityVsUp > 1)
            {
                Break();
                return;
            }
            //Limit so we cannot go faster than the 50% of max speed in the "reverse" direction
            if (_velocityVsUp < -maxSpeed * 0.5f)
            {
                return;
            }
        }

        //Create a force for the engine
        Vector2 engineForceVector = transform.up * (_accelerationInput * accelerate);

        //Apply force and pushes the car forward
        _carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        //Limit the cars ability to turn when moving slowly
        var minSpeedBeforeAllowTurningFactor = _carRigidbody2D.velocity.magnitude / 24;
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        //Update the rotation angle based on input
        if (Vector2.Dot(_carRigidbody2D.velocity, transform.up) < 0)
            _steeringInput *= -1;
        _rotationAngle -= _steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        //Apply steering by rotating the car object
        _carRigidbody2D.MoveRotation(_rotationAngle);
    }

    private void KillOrthogonalVelocity()
    {
        //Get forward and right velocity of the car
        var up = transform.up;
        Vector2 forwardVelocity = up * Vector2.Dot(_carRigidbody2D.velocity, up);
        var right = transform.right;
        Vector2 rightVelocity = right * Vector2.Dot(_carRigidbody2D.velocity, right);


        //Kill the orthogonal velocity (side velocity) based on how much the car should drift. 
        _carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        _steeringInput = inputVector.x;
        _accelerationInput = inputVector.y;
    }

    private void Break()
    {
        _carRigidbody2D.drag *= 2.5f;
        _isBreaking = true;
    }
    public bool IsTierScreeching(out float lateralVelocity, out bool isBreaking)
    {
        isBreaking = _isBreaking;
        float sideVelocity = math.abs(Vector2.Dot(transform.right, _carRigidbody2D.velocity));
        lateralVelocity = sideVelocity;
        if (_isBreaking && _carRigidbody2D.velocity.magnitude >= 15)
            return true;

        return (sideVelocity) > 7f;
    }
}
