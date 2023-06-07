using Bullet;
using UnityEngine;

namespace Car
{
    public class FireController : MonoBehaviour
    {
        public GameObject bullet;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private GameObject parent;
        private Camera _cameraMain;
        private bool _readyToFire = true;

        private void Awake()
        {
            _cameraMain = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            // Quaternion direction = 
            if (Input.GetKeyDown(KeyCode.Mouse0))
                if (_readyToFire)
                {
                    var mousePosition = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
                    var shootDirection = mousePosition - transform.position;

                    var rotationToTarget = Quaternion.LookRotation(Vector3.forward, shootDirection);

                    var bulletP = Instantiate(bullet, transform.position + shootDirection.normalized,
                        rotationToTarget);
                    bulletP.GetComponent<BulletController>().Parent = parent;
                    bulletP.GetComponent<Rigidbody2D>().AddForce(bulletP.transform.up * 0.3f, ForceMode2D.Impulse);
                    _readyToFire = false;
                    Invoke(nameof(MakeReady), fireRate);
                }
        }


        private void MakeReady()
        {
            _readyToFire = true;
        }
    }
}