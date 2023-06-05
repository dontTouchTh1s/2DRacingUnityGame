using System;
using Unity.Mathematics;
using UnityEngine;

namespace Car
{
    public class CarController : MonoBehaviour
    {
        [Header("Car settings")] [SerializeField]
        private float driftFactor = 0.95f;

        [SerializeField] private float accelerationFactor = 1.5f;
        [SerializeField] public float enginePower = 10;
        [SerializeField] private float turnFactor = 3.5f;
        [SerializeField] private float breakFactor = 1.4f;
        public float gear = 4;
        public float gearRatio = 0.1f;

        //Local variables
        private float _accelerationInput;

        //Components
        private Rigidbody2D _carRigidbody2D;
        private float _currentEnginePower;

        private int _currentGear = 1;
        private bool _isBreaking;
        private float _maxGearSpeed;
        private float _rotationAngle;
        private float _steeringInput;
        private float _velocityVsUp;
        [NonSerialized] public float LinearDrag;
        [NonSerialized] public WheelSmokeParticleHandler SmokeParticleHandler;
        [NonSerialized] public TrailRenderHandler TrailRenderHandler;

        private float MaxSpeed { get; set; }
        public float VelocityMagnitude => _carRigidbody2D.velocity.magnitude;
        public bool IsReversing => Vector2.Dot(transform.up, _carRigidbody2D.velocity) < 0;
        public float CurrentGear => _currentGear;


        //Awake is called when the script instance is being loaded.
        private void Awake()
        {
            _carRigidbody2D = GetComponent<Rigidbody2D>();
            LinearDrag = _carRigidbody2D.drag;
            _currentEnginePower = enginePower * accelerationFactor;
            MaxSpeed = (gear + gear * gearRatio) * enginePower;
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
            _carRigidbody2D.drag = LinearDrag;
            _isBreaking = false;
            //Calculate how much "forward" we are going in terms of the direction of our velocity
            _velocityVsUp = Vector2.Dot(transform.up, _carRigidbody2D.velocity);

            //Limit so we cannot go faster than the max speed in the "forward" direction
            _maxGearSpeed = (_currentGear + _currentGear * gearRatio) * enginePower;

            if (_accelerationInput > 0)
            {
                if (_velocityVsUp < -1)
                {
                    Break();
                    return;
                }

                if (_velocityVsUp > MaxSpeed) return;

                _currentEnginePower = Mathf.Lerp(_currentEnginePower, _maxGearSpeed, 0.01f * accelerationFactor);
                if (_velocityVsUp > _maxGearSpeed && _currentGear < gear) _currentGear++;
            }

            _maxGearSpeed = (_currentGear - 1 + (_currentGear - 1) * gearRatio) * enginePower - enginePower / 2;
            _maxGearSpeed = Mathf.Max(_maxGearSpeed, enginePower);
            if (_velocityVsUp < _maxGearSpeed)
            {
                if (_currentGear > 1)
                    _currentGear--;
                _currentEnginePower = _maxGearSpeed;
            }

            //Limit so we cannot go faster in any direction while accelerating
            if (_carRigidbody2D.velocity.sqrMagnitude > MaxSpeed * MaxSpeed && _accelerationInput > 0)
                return;

            if (_accelerationInput < 0)
            {
                if (_velocityVsUp > 1)
                {
                    Break();
                    return;
                }

                //Limit so we cannot go faster than the 50% of max speed in the "reverse" direction
                if (_velocityVsUp < -MaxSpeed * 0.5f) return;
                _currentEnginePower = enginePower;
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
}