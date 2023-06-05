using System;
using Car;
using UnityEngine;

public class WheelSmokeParticleHandler : MonoBehaviour
{
    private CarController _carController;
    private ParticleSystem.EmissionModule _emissionModule;
    private float _particleEmissionRate;
    [NonSerialized] public ParticleSystem ParticleSystem;

    private void Awake()
    {
        _carController = GetComponentInParent<CarController>();
        ParticleSystem = GetComponent<ParticleSystem>();
        var emissionModule = ParticleSystem.emission;

        emissionModule.rateOverTime = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        // var emissionModule = ParticleSystem.emission;
        // _particleEmissionRate = Mathf.Lerp(_particleEmissionRate, 0, Time.deltaTime * 5);
        // emissionModule.rateOverTime = _particleEmissionRate;
        //
        // if (!_carController.IsTierScreeching(out var lateralVelocity, out var isBreaking)) return;
        // if (isBreaking)
        //     _particleEmissionRate = 30;
        // else
        //     _particleEmissionRate = math.abs(lateralVelocity * 2);
    }
}