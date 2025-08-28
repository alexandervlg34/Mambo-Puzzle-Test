using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Match3.Views
{
    public class CellContentView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private static readonly int AnimationOffsetHash = Animator.StringToHash("AnimationOffset");

        private void Awake()
        {
            _animator.SetFloat(AnimationOffsetHash, UnityEngine.Random.Range(0f, 0.9f));
        }

        public Task MoveTo(Vector2 targetPosition, float speed, Action callback = null)
        {
            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            StartCoroutine(MoveToRoutine(targetPosition, speed, source, callback));
            return source.Task;
        }

        public Task AnimateShrink(float animationTime)
        {
            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            StartCoroutine(AnimateScaleRoutine(Vector3.zero, animationTime, source));
            return source.Task;
        }

        private IEnumerator MoveToRoutine(Vector2 targetPosition, float speed, TaskCompletionSource<bool> source, Action callback)
        {
            Vector2 initialPosition = transform.position;
            float distance = Vector2.Distance(initialPosition, targetPosition);
            float duration = distance / speed;

            float currentTime = 0f;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;

                float progress = currentTime / duration;

                transform.position = Vector3.Lerp(initialPosition, targetPosition, progress);

                yield return null;
            }

            transform.position = targetPosition;
            callback?.Invoke();
            source.SetResult(true);
        }

        private IEnumerator AnimateScaleRoutine(Vector3 endScale, float animationTime, TaskCompletionSource<bool> source)
        {
            Vector3 initialScale = transform.localScale;

            float currentTime = 0f;

            while (currentTime < animationTime)
            {
                currentTime += Time.deltaTime;

                float progress = currentTime / animationTime;

                transform.localScale = Vector3.Lerp(initialScale, endScale, progress);

                yield return null;
            }

            source.SetResult(true);
        }
    }
}
