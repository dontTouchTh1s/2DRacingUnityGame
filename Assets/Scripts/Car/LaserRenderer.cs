using UnityEngine;

namespace Car
{
    public class LaserRenderer : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 100;
        private Camera _cameraMain;
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _cameraMain = Camera.main;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            var mousePosition = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
            var carTransform = transform;
            var position = carTransform.position;
            var shootDirection = mousePosition - position;
            var up = carTransform.up;
            var right = carTransform.right;
            var gunPosition = Vector2.zero;
            if (mousePosition.x < position.x && mousePosition.y < position.y)
                gunPosition = up * -1 + right * -1;
            if (mousePosition.x < position.x && mousePosition.y > position.y)
                gunPosition = up * 1 + right * -1;
            if (mousePosition.x > position.x && mousePosition.y < position.y)
                gunPosition = up * -1 + right * 1;
            if (mousePosition.x > position.x && mousePosition.y > position.y)
                gunPosition = up * 1 + right * 1;
            var gunPos3 = new Vector3(position.x + gunPosition.x, position.y + gunPosition.y, position.z);
            var ray = Physics2D.Raycast(gunPos3, shootDirection);
            // Debug.DrawRay(gunPos3, shootDirection * 10, Color.red);
            if (ray.collider != null) DrawLaser(gunPos3, ray.point);
            else DrawLaser(gunPos3, shootDirection * maxDistance);
        }

        private void DrawLaser(Vector2 start, Vector2 end)
        {
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
        }
    }
}