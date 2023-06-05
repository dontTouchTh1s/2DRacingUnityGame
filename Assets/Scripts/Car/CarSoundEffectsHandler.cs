using UnityEngine;

namespace Car
{
    [RequireComponent(typeof(AudioSource))]
    public class CarSoundEffectsHandler : MonoBehaviour
    {
        [Header("Audio Sources")] public AudioSource carTierScreechAudioSource;

        [SerializeField] private AudioSource carEngineAudioSource;
        [SerializeField] private AudioSource carHitAudioSource;
        [SerializeField] private AudioSource copSirenAudioSource;

        private CarController _carController;
        private float _gearSpeed;
        private float _velocityMagnitude;

        private void Awake()
        {
            _carController = GetComponent<CarController>();
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateCarEngineSfx();
            UpdateCarTierScreechingSfx();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var relativeVelocity = col.relativeVelocity.magnitude;
            carHitAudioSource.volume = relativeVelocity * 0.07f;
            carHitAudioSource.pitch = Random.Range(0.95f, 1.05f);
            if (!carHitAudioSource.isPlaying)
                carHitAudioSource.Play();
        }


        private void UpdateCarTierScreechingSfx()
        {
            if (_carController.IsTierScreeching(out var lateralVelocity, out var isBreaking))
            {
                if (isBreaking)
                {
                    carTierScreechAudioSource.volume =
                        Mathf.Lerp(carTierScreechAudioSource.volume, 1f, Time.deltaTime * 10);
                    carTierScreechAudioSource.pitch =
                        Mathf.Lerp(carTierScreechAudioSource.pitch, 0.5f, Time.deltaTime * 10);
                    return;
                }

                carTierScreechAudioSource.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                var screechingPitchValue = Mathf.Clamp(Mathf.Abs(lateralVelocity) * 0.05f, 0.1f, 1.5f);
                carTierScreechAudioSource.pitch = screechingPitchValue;
            }

            carTierScreechAudioSource.volume = Mathf.Lerp(carTierScreechAudioSource.volume, 0, Time.deltaTime * 10);
        }

        private void UpdateCarEngineSfx()
        {
            var gearFactor = _carController.CurrentGear * (0.4f + _carController.gearRatio);
            gearFactor = Mathf.Clamp(gearFactor, 0.2f, _carController.gear);
            _velocityMagnitude = _carController.VelocityMagnitude / gearFactor;

            // Set car engine volume based on car magnitude velocity
            var engineVolume = _velocityMagnitude * 0.05f;
            engineVolume = Mathf.Clamp(engineVolume * 0.8f, 0.2f, 1f);
            var enginePitch = _velocityMagnitude * 0.05f;
            enginePitch = Mathf.Clamp(enginePitch, 0.5f, 1.6f);
            carEngineAudioSource.volume = engineVolume;
            carEngineAudioSource.pitch = enginePitch;
        }
    }
}