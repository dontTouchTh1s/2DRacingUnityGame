using Surfaces;
using UnityEngine;

namespace Car
{
    public class CarTriggerHandler : MonoBehaviour
    {
        private CarController _carController;
        private int _currentPriority;
        private WheelSmokeParticleHandler _particleSystem;
        private TrailRenderHandler _trailRendererHandler;

        private void Awake()
        {
            _carController = GetComponent<CarController>();
            _trailRendererHandler = GetComponentInChildren<TrailRenderHandler>();
            _particleSystem = GetComponentInChildren<WheelSmokeParticleHandler>();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Surface")) return;
            _currentPriority = 0;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Surface")) return;

            var materialHandler = other.GetComponentInParent<SurfaceMaterialHandler>();

            if (materialHandler.priority >= _currentPriority)
            {
                _currentPriority = materialHandler.priority;
                _carController.LinearDrag = materialHandler.drag;
                //_trailRendererHandler.TrailRenderer.colorGradient = materialHandler.trailRenderer;
                // _particleSystem.ParticleSystem = materialHandler.particleSystem;
            }
        }
    }
}