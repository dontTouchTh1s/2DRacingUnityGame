using JetBrains.Annotations;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [CanBeNull] public GameObject carGameObject;

    public readonly float cameraFirstMaxSpeedFactor = 8f;
    private float _cameraMaxSpeedFactor;
    private CarController _carController;

    // Start is called before the first frame update
    private void Start()
    {
        _cameraMaxSpeedFactor = cameraFirstMaxSpeedFactor;
        _carController = carGameObject.GetComponent<CarController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!carGameObject) return;
        var cameraSpeedFactor = Mathf.Lerp(0.5f, _carController.VelocityMagnitude / _cameraMaxSpeedFactor,
            0.35f);
        Debug.Log(_carController.maxSpeed / cameraFirstMaxSpeedFactor * 0.35f + 0.3f -
                  cameraSpeedFactor);
        if (cameraSpeedFactor >= _carController.maxSpeed / cameraFirstMaxSpeedFactor * 0.35f + 0.3f)
            _cameraMaxSpeedFactor =
                Mathf.Lerp(_cameraMaxSpeedFactor, cameraFirstMaxSpeedFactor - 6, 0.003f);
        else
            _cameraMaxSpeedFactor = cameraFirstMaxSpeedFactor;
        Debug.Log(_cameraMaxSpeedFactor);

        var cameraPosition = transform.position;
        var carPosition = carGameObject.transform.position;
        var carXYPositionVector = new Vector3(carPosition.x, carPosition.y, cameraPosition.z);
        transform.position = Vector3.Lerp(cameraPosition, carXYPositionVector,
            Time.deltaTime * cameraSpeedFactor);
    }
}