using Unity.Mathematics;
using UnityEngine;

public class WheelSmokeParticleHandler : MonoBehaviour
{
    private float _particleEmissionRate;
    private CarController _carController;
    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _emissionModule;

    private void Awake()
    {
        _carController = GetComponentInParent<CarController>();
        _particleSystem = GetComponent<ParticleSystem>();
        _emissionModule = _particleSystem.emission;

        _emissionModule.rateOverTime = 0;
    }

    // Update is called once per frame
    private void Update()
    {

        _particleEmissionRate = Mathf.Lerp(_particleEmissionRate, 0, Time.deltaTime * 5);
        _emissionModule.rateOverTime = _particleEmissionRate;

        if (!_carController.IsTierScreeching(out float lateralVelocity, out bool isBreaking)) return;
        if (isBreaking)
            _particleEmissionRate = 30;
        else
            _particleEmissionRate = math.abs(lateralVelocity * 2);


    }
}
