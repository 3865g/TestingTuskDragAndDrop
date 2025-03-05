using UnityEngine;

namespace Player
{
    public class CameraScroll : MonoBehaviour
    {
        public float scrollSpeed = 10f; 
        public float smoothTime = 0.3f; 
        public GameObject boundingObject;
        
        private Vector3 _velocity = Vector3.zero; 
        private float _targetX; 
        private Camera _camera;
        private Bounds _bounds; 
        private float _cameraHalfWidth; 

        private void Start()
        {
            //Инициализация камеры, позиции камеры, размеров сцены
            _camera = Camera.main;
            _targetX = _camera.transform.position.x;
            InitialSceneBounds();
        }

        public void ScrolingCamera(Vector2 touchDelta)
        {
            if (_camera)
            {
                float moveX = -touchDelta.x / Screen.width * scrollSpeed;
                _targetX += moveX;
                
                _targetX = Mathf.Clamp(_targetX, _bounds.min.x + _cameraHalfWidth, _bounds.max.x - _cameraHalfWidth);
                
                Vector3 targetPosition = new Vector3(_targetX, _camera.transform.position.y, _camera.transform.position.z);
                _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, targetPosition, ref _velocity, smoothTime);
            }
        }

        
        //Получаем границы сцены за которые камера не может выехать
        private void InitialSceneBounds()
        {
            if (!boundingObject)
            {
                Debug.LogError("Bounding object is null");
                return;
            }

            
            Renderer renderer = boundingObject.GetComponent<Renderer>();
            if (renderer)
            {
                _bounds = renderer.bounds;
            }
            
            _cameraHalfWidth = _camera.orthographicSize * _camera.aspect;
        }
    }
}