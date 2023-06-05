using System;
using Car;
using UnityEngine;

public class TrailRenderHandler : MonoBehaviour
{
    private CarController _carController;
    [NonSerialized] public TrailRenderer TrailRenderer;

    private void Awake()
    {
        _carController = GetComponentInParent<CarController>();
        TrailRenderer = GetComponent<TrailRenderer>();
        TrailRenderer.emitting = false;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    private void Update()
    {
        // TrailRenderer.emitting = _carController.IsTierScreeching(out _, out _);
    }
}