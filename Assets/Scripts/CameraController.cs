using JetBrains.Annotations;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [CanBeNull] public GameObject carGameObject;

    public float cameraSpeedFactor = 0.1f;
    private float _cameraSpeedFactor;
    private CarController _carController;


    // Start is called before the first frame update
    private void Start()
    {
        _cameraSpeedFactor = cameraSpeedFactor;
        _carController = carGameObject.GetComponent<CarController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!carGameObject) return;
        var cameraPosition = transform.position;
        var carPosition = carGameObject.transform.position;
        var carXYPositionVector = new Vector3(carPosition.x, carPosition.y, cameraPosition.z);

        var cameraSpeed = _carController.VelocityMagnitude * _cameraSpeedFactor + 0.5f;
        if (cameraSpeed >= _carController.maxSpeed * _cameraSpeedFactor + 0.25f)
            _cameraSpeedFactor = Mathf.Lerp(_cameraSpeedFactor, cameraSpeedFactor * 8f, 0.0005f);
        else
            _cameraSpeedFactor = Mathf.Lerp(_cameraSpeedFactor, cameraSpeedFactor, 0.0005f);
        transform.position = Vector3.Lerp(cameraPosition, carXYPositionVector,
            Time.deltaTime * cameraSpeed);
        // transform.position = Vector3.SmoothDamp(cameraPosition, carXYPositionVector, ref _speed, 0.2f);
    }
}