using Car;
using UnityEngine;

namespace Cops
{
    public class CopController : MonoBehaviour
    {
        private CarController _carController;
        private bool _destroyed;
        private float _onDestroyBreak;

        private void Awake()
        {
            _onDestroyBreak = Random.Range(0.8f, 1.6f);
            _carController = GetComponent<CarController>();
        }

        private void Update()
        {
            if (_destroyed)
                _carController.Break(_onDestroyBreak);
        }

        public void onCopDestroy()
        {
            _destroyed = true;
        }
    }
}