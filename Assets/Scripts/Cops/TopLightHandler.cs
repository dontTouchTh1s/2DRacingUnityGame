using UnityEngine;

namespace Cops
{
    public class TopLightHandler : MonoBehaviour
    {
        [SerializeField] private float rotation = 10;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(0, 0, transform.rotation.z + rotation * Time.deltaTime * 100);
        }
    }
}