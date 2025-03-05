using UnityEngine;

namespace Items
{
    public class DragAndDrop : MonoBehaviour
    {
        public DropToBox dropToBox;
        public Rigidbody2D rigidBody;
         
        private bool _isDragging;
        private bool _isInsideTrigger;
        private bool _dropToBox;
        private Transform _boxTransform;
        private Camera _camera;
        
        private Vector2 _screenBounds; 
        private float _objectWidth; 
        private float _objectHeight; 
        
        private const string FloorTag = "Floor";
        private const string BoxHoleTag = "BoxHole";

        void Start()
        {
            _camera = Camera.main;
            
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                _objectWidth = spriteRenderer.bounds.size.x / 2;
                _objectHeight = spriteRenderer.bounds.size.y / 2;
            }
        }

        
        private void OnTriggerEnter2D(Collider2D other)
        {
            DetectTriggerTag(other);
        }

        
        private void OnTriggerExit2D(Collider2D other)
        {
            DetectTriggerExitTag(other);
        }
        
        /* логика поднятия объекта, проверяем рейкастом попали ли мы мышкой по объекту, если да,
        переключем bool isDragging, отключаем симуляцию и переходим к DragObject */
        public void TakeObject()
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider && hit.collider.gameObject == gameObject)
            {
                _isDragging = true;
                if (rigidBody)
                {
                    rigidBody.simulated = true; 
                    rigidBody.isKinematic = true; 
                }
            }
        }
        
        /* логика перемещения объекта за мышкой,
         конвертируем позицию мыши в мировые координаты и устанавливаем их объекту, ограничивая границами экрана */
        
        public void DragObject()
        {
            
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector4 screenBounds = CalculateScreenBounds();
            
            float clampedX = Mathf.Clamp(mousePosition.x, screenBounds.x + _objectWidth, screenBounds.y - _objectWidth);
            float clampedY = Mathf.Clamp(mousePosition.y, screenBounds.z + _objectHeight, screenBounds.w - _objectHeight);

            transform.position = new Vector2(clampedX, clampedY);
        }

        
        /* логика что делать с объектом когда отпускам мышку,
         либо он ложится, либо улетает в коробку, либо включается симуляция и он падает */
        
        public void DropObject()
        {
            _isDragging = false;

            if (rigidBody)
            {
                if (_isInsideTrigger)
                {
                    rigidBody.isKinematic = true;
                    rigidBody.linearVelocity = Vector2.zero;
                }
                else if(_dropToBox)
                {
                    dropToBox.MoveToBox(_boxTransform);
                }
                else
                {
                    rigidBody.isKinematic = false;
                    rigidBody.simulated = true;
                }
            }
        }

        /* ниже проверяем теги при входе и выходе из триггеров,
         и при необходимости отключаем симуляцию и переключаем bool isInsideTrigger  */
        
        private void DetectTriggerTag(Collider2D other)
        {
            
            if (other.CompareTag(FloorTag) && other.isTrigger)
            {
                _isInsideTrigger = true;
                
                rigidBody.linearVelocity = Vector2.zero; 
                rigidBody.isKinematic = true; 
            }
            else if (other.CompareTag(BoxHoleTag) && other.isTrigger)
            {
                
                _boxTransform = other.transform;
                _dropToBox = true;
                rigidBody.linearVelocity = Vector2.zero;
                rigidBody.isKinematic = true; 
            }
        }

        private void DetectTriggerExitTag(Collider2D other)
        {
            if (other.CompareTag(FloorTag) && other.isTrigger)
            {
                _isInsideTrigger = false;
            }
            else if (other.CompareTag(BoxHoleTag) && other.isTrigger)
            {
                _dropToBox = false;
            }
        }
        
        //Расчет границ экрана что бы нельзя было уносить предметы за экран

        private Vector4 CalculateScreenBounds()
        {
            if (!_camera)
            {
                return Vector4.zero;
            }

            // Получаем текущие границы экрана в мировых координатах
            Vector3 bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            Vector3 topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
            
            return new Vector4(bottomLeft.x, topRight.x, bottomLeft.y, topRight.y);
        }
        
    }
}