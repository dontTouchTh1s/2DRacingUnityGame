using Car;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject carGameObject;

    [SerializeField] private float cameraSpeedFactor = 0.1f;
    private Camera _cameraController;

    private Vector3 _cameraSpeed;
    private CarController _carController;
    private float _currentDistance = 1;
    private float _distanceVelocity;
    private bool _inTurn;
    private float _size = 20;
    private float _sizeVelocity;

    private float _targetDistance = 1;

    // Start is called before the first frame update
    private void Start()
    {
        _carController = carGameObject.GetComponent<CarController>();
        _cameraController = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        _cameraController.orthographicSize =
            Mathf.SmoothDamp(_cameraController.orthographicSize, _size, ref _sizeVelocity, 1f);
        _currentDistance = Mathf.SmoothDamp(_currentDistance, _targetDistance, ref _distanceVelocity, 1f);

        if (!carGameObject) return;
        var cameraPosition = transform.position;
        var carPosition = carGameObject.transform.position;

        var up = _carController.transform.up;
        var carXYPositionVector = new Vector3(carPosition.x + up.x * _currentDistance,
            carPosition.y + up.y * _currentDistance, cameraPosition.z);


        transform.position = Vector3.SmoothDamp(cameraPosition, carXYPositionVector,
            ref _cameraSpeed, 0.1f);
    }

    public void OnTurnEnter()
    {
        _inTurn = !_inTurn;
        if (_inTurn)
        {
            _targetDistance = 18;
            _size = 35f; // 27
        }
        else
        {
            _targetDistance = 2;
            _size = 27f; // 20
        }
    }
}