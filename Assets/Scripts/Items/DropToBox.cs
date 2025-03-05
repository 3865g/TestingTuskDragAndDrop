using UnityEngine;
using System.Collections;

namespace Items
{
    public class DropToBox : MonoBehaviour
    {
        
        public SpriteRenderer spriteRenderer;
        
        public float moveDuration = 1f; // Длительность анимации перемещения
        public float shrinkScale = 0.1f; // Масштаб объекта при уменьшении
        public float respawnDelay = 1f;
        

        private Vector3 _originalScale;
        private Vector3 _spawnLocation;
        private Transform _boxTransform; 

        void Start()
        {
            // Устанавливаем изначальный размер, и позицию
            _originalScale = transform.localScale; 
            _spawnLocation =  gameObject.transform.position;
        }

        //Функция которая запускается из скрипта DragAndDrop, если предмет отпускается над коробкой
        public void MoveToBox(Transform boxTransform)
        {
            _boxTransform = boxTransform;
            StartCoroutine(MoveIntoBox());
        }
       
        // Анимация перемещения в коробку
        private IEnumerator MoveIntoBox()
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = _boxTransform.position;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = new Vector3(shrinkScale, shrinkScale, shrinkScale);

            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / moveDuration);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            
            transform.position = endPosition;
            transform.localScale = endScale;

            

           spriteRenderer.enabled = false;
            
            //После того как предмет улетел в коробку, он спавнится на стартовой позиции
            StartCoroutine(RespawnObject());
        }
        
        
        // логика возвращения предмета на стартовую позицию
        private IEnumerator RespawnObject()
        {
            
            yield return new WaitForSeconds(respawnDelay);

           
            spriteRenderer.enabled = true;
            gameObject.transform.position = _spawnLocation;

            
            Vector3 startScale = new Vector3(shrinkScale, shrinkScale, shrinkScale);
            Vector3 endScale = _originalScale;

            float elapsedTime = 0f;
            
            while (elapsedTime < moveDuration)
            {
                gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            gameObject.transform.localScale = endScale;
        }
    }
}