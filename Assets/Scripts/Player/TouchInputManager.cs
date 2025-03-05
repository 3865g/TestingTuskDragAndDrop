using Items;
using UnityEngine;

namespace Player
{
    public class TouchInputManager : MonoBehaviour
    {
        public CameraScroll cameraScroll;

        private Camera _camera;

        private DragAndDrop _draggedObject; 
        private Vector2 _dragOffset; 
        private int _dragFingerId = -1; 
        private int _scrollFingerId = -1;
        
        
        
        private const string DraggableTag = "Draggable";

        private void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            // Обработка всех касаний
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        HandleTouchBegan(touch);
                        break;

                    case TouchPhase.Moved:
                        HandleTouchMoved(touch);
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        HandleTouchEnded(touch);
                        break;
                }
            }
            
        }

        // Проверяем куда произошло касание, если есть тег Draggable, начинаем перетаскивание, если нет,
        // то скролл экрана, с проверкой что второй палец не выполняет то же действие
        private void HandleTouchBegan(Touch touch)
        {
            Vector2 touchWorldPosition = _camera.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchWorldPosition, Vector2.zero);

            if (hit.collider && hit.collider.CompareTag(DraggableTag))
            {
                if (_dragFingerId == -1) 
                {
                    _draggedObject = hit.collider.gameObject.GetComponent<DragAndDrop>();
                    _draggedObject.TakeObject();
                    _dragFingerId = touch.fingerId;
                    
                }
            }
            else
            {
                if (_scrollFingerId == -1) 
                {
                    _scrollFingerId = touch.fingerId;
                }
            }
        }

        //Выполняем перетаскивание или скролл
        private void HandleTouchMoved(Touch touch)
        {
            if (touch.fingerId == _dragFingerId && _draggedObject)
            {
                _draggedObject.DragObject();
            }
            else if (touch.fingerId == _scrollFingerId)
            {
                cameraScroll.ScrolingCamera(touch.deltaPosition);
            }
        }

        //Когда отпускаем палец, отпускаем объект(если есть) обновляем переменные
        private void HandleTouchEnded(Touch touch)
        {
            if (touch.fingerId == _dragFingerId)
            {
                _draggedObject.DropObject();
                _draggedObject = null;
                _dragFingerId = -1;
            }
            else if (touch.fingerId == _scrollFingerId)
            {
                _scrollFingerId = -1;
            }
        }
        
    }
}