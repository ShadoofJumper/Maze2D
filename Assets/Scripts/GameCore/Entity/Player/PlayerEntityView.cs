using System.Collections;
using UnityEngine;

namespace GameCore.Entity.Player
{
    public class PlayerEntityView : MonoBehaviour
    {
        [SerializeField] private float _playerMoveDuration;

        private IEnumerator _moveCoroutine;
        private bool _isMoving = false;
        
        public bool IsMoving => _isMoving;
        
        public void MoveTo(Vector2 destination)
        {
            if (IsMoving)
                return;
            if(_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
            _moveCoroutine = MoveWithDuration(destination, _playerMoveDuration);
            StartCoroutine(_moveCoroutine);
        }
        
        private IEnumerator MoveWithDuration(Vector2 destination, float duration)
        {
            _isMoving = true;
            float timeElapsed = 0;

            while ((Vector2)transform.position != destination)
            {
                timeElapsed += Time.deltaTime;
                Vector3 newPosition = Vector2.Lerp(transform.position, destination, timeElapsed / duration);
                newPosition.z = transform.position.z; // preserve Z
                transform.position = newPosition;
                yield return null;
            }
            _isMoving = false;
        }

        private void OnDestroy()
        {
            if(_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        }
    }
}