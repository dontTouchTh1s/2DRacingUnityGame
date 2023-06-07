using Bullet;
using Car;
using Cops.AStar;
using UnityEngine;

namespace Cops
{
    public class CopAi : MonoBehaviour
    {
        [SerializeField] private GameObject bullet;
        [SerializeField] private float fireRate = 1.5f;

        private bool _blocked;
        private CarController _carController;
        private Graph _graph;
        private bool _needNewPath;
        private PathFinding _pathFinding;
        private GameObject _player;
        private CarController _playerController;
        private bool _readyToFire = true;
        private float _steerDestroyAmount;
        private Vector3 _targetPosition;

        // Start is called before the first frame update
        private void Awake()
        {
            _carController = GetComponent<CarController>();
            _player = GameObject.Find("Car");
            _playerController = _player.GetComponent<CarController>();
            _pathFinding = GetComponentInChildren<PathFinding>();
            _graph = new Graph(500, 1200, 1);
            _needNewPath = true;
        }

        private void FixedUpdate()
        {
            var position = transform.position;
            var playerPosition = _player.transform.position;

            var rayToPlayer = _pathFinding.CheckLineBlock(position, playerPosition, out var col);
            if (rayToPlayer && !col.gameObject.CompareTag("PlayerCollider") &&
                !col.gameObject.CompareTag("CarsCollider"))
            {
                if (_needNewPath)
                    // Way to player is blocked, start path finding
                    CreatePath(position, playerPosition);
            }
            else
            {
                Fire();
                _targetPosition = playerPosition;
            }


            var inputVector = Vector2.zero;
            var copTransformUp = transform.up;

            // Turning to target direction
            inputVector.x = TurnToTarget();

            if (Vector3.Distance(position, _targetPosition) > 12 ||
                _playerController.VelocityMagnitude > _carController.VelocityMagnitude * 1.3)
                inputVector.y = 1;

            if (Vector3.Distance(position, _targetPosition) < _carController.VelocityMagnitude / 3.5f)
                // If we are moving really faster than play, and we are near him we break
                if (_playerController.VelocityMagnitude < _carController.VelocityMagnitude / 1.5f)
                    inputVector.y = -1;

            var blockDistance = Mathf.Clamp(_carController.VelocityMagnitude / 3.5f, 5, 50);
            if (_pathFinding.CheckWayBlock(copTransformUp, blockDistance, out col, Color.yellow))
                if (!col.gameObject.CompareTag("CarsCollider") && !col.gameObject.CompareTag("PlayerCollider"))
                    _blocked = true;
            if (_pathFinding.CheckWayBlock(copTransformUp, 12f, out col, Color.green))
                if (col.gameObject.CompareTag("CarsCollider"))
                    inputVector.y = 0;
            if (_blocked)
            {
                inputVector.y = -1;


                if (!_pathFinding.CheckWayBlock(copTransformUp, 7.5f, out _))
                    // Front way is no longer blocked
                    _blocked = false;
            }

            if (_carController.IsReversing) inputVector.x *= -1;
            inputVector.x += _steerDestroyAmount;
            _carController.SetInputVector(inputVector);
        }


        private float TurnToTarget()
        {
            var copTransform = transform;
            var transformUp = copTransform.up;
            var transformRight = copTransform.right;

            var vectorToTarget = _targetPosition - copTransform.position;
            vectorToTarget.Normalize();
            var angleToTarget = Vector2.SignedAngle(transformUp, vectorToTarget);
            angleToTarget *= -1;
            var steerAmount = angleToTarget / 45f;
            steerAmount = Mathf.Clamp(steerAmount, -1, 1);

            var topLeft = transformRight * -1 + transformUp;
            var topRight = transformRight + transformUp;

            var returnLimitDistance = Mathf.Clamp(_carController.VelocityMagnitude / 7.5f, 2.5f, 15);
            var steerLimitDistance = Mathf.Clamp(_carController.VelocityMagnitude / 6, 4f, 15);
            if (_pathFinding.CheckWayBlock(topRight, returnLimitDistance, out _, Color.magenta))
            {
                steerAmount = -1f;
                return steerAmount;
            }

            if (_pathFinding.CheckWayBlock(topLeft, returnLimitDistance, out _, Color.magenta))
            {
                steerAmount = 1f;
                return steerAmount;
            }

            if (steerAmount > 0)
                // Steering to right, check right for blocking
                if (_pathFinding.CheckWayBlock(topRight, steerLimitDistance, out _))
                {
                    steerAmount = 0;
                    return steerAmount;
                }

            if (steerAmount < 0)
                // Steering to left, check left for blocking
                if (_pathFinding.CheckWayBlock(topLeft, steerLimitDistance, out _))
                {
                    steerAmount = 0;
                    return steerAmount;
                }


            return steerAmount;
        }

        private async void CreatePath(Vector3 position, Vector3 playerPosition)
        {
            _needNewPath = false;
            var paths = await _graph.FindPath(position, playerPosition);
            for (var i = 0; i < paths.Count; i += 1)
                if (!_pathFinding.CheckLineBlock(position, paths[i], out _))
                {
                    Debug.DrawLine(position, paths[i], Color.blue, 1f);
                    _targetPosition = paths[i];
                    break;
                }

            Invoke(nameof(NeedNewPath), 1);
        }

        public void onCopDestroye()
        {
            int[] factor = { 1, -1 };
            _steerDestroyAmount = Random.Range(1.7f, 3f) * factor[Random.Range(0, 2)];
        }

        private void NeedNewPath()
        {
            _needNewPath = true;
        }

        private void Fire()
        {
            if (_readyToFire)
            {
                var playerPosition = _player.transform.position;

                var shootDirection = playerPosition - transform.position;

                var rotationToTarget = Quaternion.LookRotation(Vector3.forward, shootDirection);

                var bulletP = Instantiate(bullet, transform.position + shootDirection.normalized * 1.5f,
                    rotationToTarget);
                bulletP.GetComponent<BulletController>().Parent = gameObject;
                bulletP.GetComponent<Rigidbody2D>().AddForce(bulletP.transform.up * 0.3f, ForceMode2D.Impulse);
                _readyToFire = false;
                Invoke(nameof(MakeReady), fireRate);
            }
        }

        private void MakeReady()
        {
            _readyToFire = true;
        }
    }
}