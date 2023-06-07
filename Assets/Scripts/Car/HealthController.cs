using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Car
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private GameObject destroyedObject;
        public UnityEvent Destroyed;
        public float health = 100;
        [SerializeField] private GameObject text;
        private CarController _carController;
        private bool _destroyed;
        private int _destroyType;
        private TextMeshProUGUI _healthText;
        private bool _isPlayer;
        private Rigidbody2D _rigidBody2D;

        private void Awake()
        {
            if (CompareTag("Player"))
            {
                _healthText = text.GetComponent<TextMeshProUGUI>();
                _isPlayer = true;
            }

            _rigidBody2D = GetComponent<Rigidbody2D>();
            _carController = GetComponent<CarController>();
            _destroyType = 0;
        }

        private void LateUpdate()
        {
            if (_isPlayer)
            {
                _healthText.text = health.ToString();
                if (health <= 0)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (health <= 0)
                if (!_destroyed)
                    switch (_destroyType)
                    {
                        case 0:
                        {
                            Destroyed?.Invoke();
                            var obj = Instantiate(destroyedObject, transform.position, transform.rotation);
                            obj.GetComponent<Rigidbody2D>().velocity = _rigidBody2D.velocity;
                            Destroy(gameObject);
                            break;
                        }
                        case 1:
                        {
                            Destroyed?.Invoke();
                            _destroyed = true;
                            break;
                        }
                    }
        }
    }
}