using UnityEngine;

namespace Car
{
    public class FireController : MonoBehaviour
    {
        public GameObject bullet;

        private Camera _cameraMain;

        private void Awake()
        {
            _cameraMain = Camera.main;
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            // Quaternion direction = 
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var mousePosition = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
                var shootDirection = mousePosition - transform.position;

                var rotationToTarget = Quaternion.LookRotation(Vector3.forward, shootDirection);

                var bulletP = Instantiate(bullet, transform.position + shootDirection.normalized * 4, rotationToTarget);

                bulletP.GetComponent<Rigidbody2D>().AddForce(bulletP.transform.up * 2, ForceMode2D.Impulse);
            }
        }
    }
}