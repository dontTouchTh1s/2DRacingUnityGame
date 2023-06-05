using Car;
using Cops.AStar;
using UnityEngine;

namespace Cops
{
    public class CopAi : MonoBehaviour
    {
        private bool _blocked;
        private CarController _carController;
        private Graph _graph;
        private bool _needNewPath;
        private PathFinding _pathFinding;
        private GameObject _player;
        private CarController _playerController;
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
                _targetPosition = playerPosition;
            }


            var inputVector = Vector2.zero;
            var copTransformUp = transform.up;

            // Turning to target direction
            inputVector.x = TurnToTarget();

            if (Vector3.Distance(position, _targetPosition) < _carController.VelocityMagnitude / 4.5)
                // If we are moving really faster than play, and we are near him we break
                if (_playerController.VelocityMagnitude < _carController.VelocityMagnitude / 2f)
                    inputVector.y = -1;

            if (Vector3.Distance(position, _targetPosition) > 12) inputVector.y = 1;

            var blockDistance = Mathf.Clamp(_carController.VelocityMagnitude / 3.5f, 5, 50);
            if (_pathFinding.CheckWayBlock(copTransformUp, blockDistance, out col,
                    Color.yellow))
            {
                if (col.gameObject.CompareTag("CarsCollider") || col.gameObject.CompareTag("PlayerCollider"))
                {
                    inputVector.y = 0;
                    var carController = col.gameObject.GetComponentInParent<CarController>();
                    if (carController.VelocityMagnitude < _carController.VelocityMagnitude / 2f) _blocked = true;
                }
                else
                {
                    _blocked = true;
                }
            }

            if (_blocked)
            {
                inputVector.y = -1;


                if (!_pathFinding.CheckWayBlock(copTransformUp, 7.5f, out _))
                    // Front way is no longer blocked
                    _blocked = false;
            }

            if (_carController.IsReversing) inputVector.x *= -1;

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
        // private IEnumerator CreatePath(Vector3 position, Vector3 playerPosition)
        // {
        //     var paths = _graph.FindPath(position, playerPosition);
        //     for (var i = 0; i < paths.Count; i += 1)
        //         if (!_pathFinding.CheckLineBlock(position, paths[i], out _))
        //         {
        //             Debug.DrawLine(position, paths[i], Color.blue, 2f);
        //             _targetPosition = paths[i];
        //             break;
        //         }
        //
        //     _needNewPath = false;
        //     yield return null;
        // }

        private void NeedNewPath()
        {
            _needNewPath = true;
        }
    }
}