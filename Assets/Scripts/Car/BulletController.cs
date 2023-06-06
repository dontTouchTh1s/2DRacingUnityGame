using UnityEngine;

namespace Car
{
    public class BulletController : MonoBehaviour
    {
        public int damage = 10;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("colling bnich");
        }
    }
}