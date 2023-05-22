using Unity.Mathematics;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car settings")] public float driftFactor = 0.95f;
    public float accelerationFactor = 1.5f;
    public float enginePower = 10;
    public float turnFactor = 3.5f;
    public float maxSpeed = 45.0f;
    public float breakFactor = 1.4f;
    public float gear = 4;

    public float gearRatio = 0.1f;

    //Local variables
    private float _accelerationInput;

    //Components
    private Rigidbody2D _carRigidbody2D;
    private float _currentEnginePower;

    private int _currentGear = 1;
    private bool _isBreaking;
    private float _linearDrag;
    private float _maxGearSpeed;
    private float _rotationAngle;
    private float _steeringInput;
    private float _velocityVsUp;
    public float VelocityMagnitude => _carRigidbody2D.velocity.magnitude;
    public float CurrentGear => _currentGear;


    //Awake is called when the script instance is being loaded.
    private void Awake()
    {
        _carRigidbody2D = GetComponent<Rigidbody2D>();
        _linearDrag = _carRigidbody2D.drag;
        _currentEnginePower = enginePower * accelerationFactor;
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
        _carRigidbody2D.drag = _linearDrag;
        _isBreaking = false;
        //Calculate how much "forward" we are going in terms of the direction of our velocity
        _velocityVsUp = Vector2.Dot(transform.up, _carRigidbody2D.velocity);

        //Limit so we cannot go faster than the max speed in the "forward" direction

        if (_accelerationInput > 0)
        {
            if (_velocityVsUp < -1)
            {
                Break();
                return;
            }

            if (_velocityVsUp > maxSpeed) return;
        }


        //Limit so we cannot go faster in any direction while accelerating
        if (_carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && _accelerationInput > 0)
            return;

        if (_accelerationInput < 0)
        {
            if (_velocityVsUp > 1)
            {
                Break();
                return;
            }

            //Limit so we cannot go faster than the 50% of max speed in the "reverse" direction
            if (_velocityVsUp < -maxSpeed * 0.5f) return;
            _currentEnginePower = enginePower;
        }

        _maxGearSpeed = (_currentGear + _currentGear * gearRatio) * enginePower;

        _currentEnginePower = Mathf.Lerp(_currentEnginePower, _maxGearSpeed, 0.01f * accelerationFactor);
        Debug.Log("enginePower: " + _currentEnginePower);
        Debug.Log("maxSpeed: " + _maxGearSpeed);
        Debug.Log("currentGear: " + _currentGear);


        if (_velocityVsUp > _maxGearSpeed && _currentGear < gear) _currentGear++;
        _maxGearSpeed = (_currentGear - 1 + (_currentGear - 1) * gearRatio) * enginePower - enginePower / 2;
        if (_velocityVsUp < _maxGearSpeed && _currentGear > 1)
        {
            _currentGear--;
            _currentEnginePower = _maxGearSpeed;
        }

        //Create a force for the engine
        Vector2 engineForceVector = transform.up * (_accelerationInput * _currentEnginePower);

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
        _carRigidbody2D.drag += breakFactor;
        _isBreaking = true;
    }

    public bool IsTierScreeching(out float lateralVelocity, out bool isBreaking)
    {
        isBreaking = _isBreaking;
        var sideVelocity = math.abs(Vector2.Dot(transform.right, _carRigidbody2D.velocity));
        lateralVelocity = sideVelocity;
        if (_isBreaking && _carRigidbody2D.velocity.magnitude >= 15)
            return true;

        return sideVelocity > 7f;
    }
}