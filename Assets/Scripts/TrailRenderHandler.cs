using UnityEngine;

public class TrailRenderHandler : MonoBehaviour
{
    private CarController _carController;
    private TrailRenderer _trailRenderer;
    private void Awake()
    {
        _carController = GetComponentInParent<CarController>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.emitting = false;

    }
    // Start is called before the first frame update

    // Update is called once per frame
    private void Update()
    {
        _trailRenderer.emitting = _carController.IsTierScreeching(out _, out _);
    }
}
