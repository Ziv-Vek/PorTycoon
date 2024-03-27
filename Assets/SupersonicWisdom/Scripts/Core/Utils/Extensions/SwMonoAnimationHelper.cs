using System;
using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public static class SwMonoAnimationHelper
    {
        #region --- Constants ---

        private const float DEFAULT_DURATION = 2f;
        private const float SHRINK_TARGET_SCALE = 0f;
        private const float ENLARGE_TARGET_SCALE = 1f;

        #endregion


        #region --- Members ---

        private static readonly AnimationCurve DefaultAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        #endregion


        #region --- Public Methods ---

        public static void ShrinkObject(this MonoBehaviour monoBehaviour, float duration = DEFAULT_DURATION, float targetScale = SHRINK_TARGET_SCALE, AnimationCurve curve = null, Action callback = null)
        {
            monoBehaviour.StartCoroutine(ChangeObjectScaleCoroutine(monoBehaviour.transform, duration, targetScale, curve, callback));
        }
        
        public static void EnlargeObject(this MonoBehaviour monoBehaviour, float duration = DEFAULT_DURATION, float targetScale = ENLARGE_TARGET_SCALE, AnimationCurve curve = null, Action callback = null)
        {
            monoBehaviour.StartCoroutine(ChangeObjectScaleCoroutine(monoBehaviour.transform, duration, targetScale, curve, callback));
        }

        public static IEnumerator Animate(RectTransform rectTransform, float speed, Vector3 startPosition, Vector3 toPosition, float animationTime, Action onDone = null)
        {
            var animationPassedTime = 0f;

            while (Vector3.Distance(rectTransform.localPosition, toPosition) > 2f)
            {
                animationPassedTime += Time.deltaTime * speed;
                rectTransform.localPosition = Vector3.Lerp(startPosition, toPosition, animationPassedTime / animationTime);

                yield return null;
            }

            rectTransform.localPosition = toPosition;
            onDone?.Invoke();
        }
        
        #endregion


        #region --- Private Methods ---

        private static IEnumerator ChangeObjectScaleCoroutine(Transform transform, float duration, float targetScale, AnimationCurve curve, Action callback)
        {
            curve ??= DefaultAnimationCurve;
            var initialScale = transform.localScale;
            var target = new Vector3(targetScale, targetScale, targetScale);

            var time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                var percentage = time / duration;
                var scale = Vector3.Lerp(initialScale, target, curve.Evaluate(percentage));
                transform.localScale = scale;
                yield return null;
            }
            transform.localScale = target;
            callback?.Invoke();
        }

        #endregion
    }
}