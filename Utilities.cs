using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HyperTools
{
    public static class Utilities
    {
        public static float GetNormalRandom(float mean = 0, float standardDeviation = 1.0f)
        {
            float u1 = Random.Range(0f, 1f); // Uniform(0,1) random number
            float u2 = Random.Range(0f, 1f); // Uniform(0,1) random number

            // Applying the Box-Muller transform
            float z0 = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Cos(2 * Mathf.PI * u2);

            // Return the random number scaled to mean and standard deviation
            return z0 * standardDeviation + mean;
        }

        public static int GetWeightedRoll(float[] weights)
        {
            var sum = 0f;
            
            foreach (var x in weights)
            {
                sum += x;
            }

            var drop = Random.value * sum;
            var checkedArea = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                checkedArea += weights[i];
                if (drop <= checkedArea)
                {
                    return i;
                }
            }
            
            return weights.Length - 1;
        }

        public static void Shuffle<T>(List<T> list, bool useUnityRandom = true)
        {
            var random = useUnityRandom ? null : new System.Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int roll = useUnityRandom ? Random.Range(0, i + 1) : random.Next(0, i + 1);
                (list[i], list[roll]) = (list[roll], list[i]);
            }
        }

        public static bool HasAncestorGameObject(GameObject child, GameObject ancestor)
        {
            Transform currentParent = child.transform;

            while (currentParent != null)
            {
                if (currentParent.gameObject == ancestor)
                {
                    return true;
                }
                currentParent = currentParent.parent; // Move up the hierarchy
            }

            return false;
        }

        public static bool TryGetAncestorComponent<T>(GameObject child, out T component) where T : MonoBehaviour
        {
            Transform currentParent = child.transform;
            component = null;

            while (currentParent != null)
            {
                component = currentParent.gameObject.GetComponent<T>();
                if (component!= null)
                {
                    return true;
                }
                currentParent = currentParent.parent; // Move up the hierarchy
            }

            return false;
        }

        // TODO: import DOTween first, if needed
        // public static async UniTask WaitForTween(Tween tween)
        // {
        //     await WaitForTween(DOTween.Sequence().Append(tween));
        // }
        //
        // public static async UniTask WaitForTween(Sequence sequence)
        // {
        //     var taskCompletionSource = new UniTaskCompletionSource();
        //
        //     sequence.AppendCallback(() =>
        //     {
        //         taskCompletionSource.TrySetResult();
        //     });
        //
        //     sequence.Play();
        //
        //     await taskCompletionSource.Task;
        // }

        public static async UniTask<Sprite> LoadResourceSpriteAsync(string spritePath, Action<Sprite> onComplete = null)
        {
            var resourceRequest = Resources.LoadAsync<Sprite>(spritePath);
            await resourceRequest.ToUniTask();

            var sprite = resourceRequest.asset as Sprite;
            if (sprite == null)
            {
                Debug.LogError($"Failed to load sprite {spritePath}");
            }
            
            onComplete?.Invoke(sprite);

            return sprite;
        }

        public static Vector2 WorldToCanvasLocalPosition(Vector3 worldPosition, Canvas canvas, Camera camera = null)
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            RectTransform canvasRect = (RectTransform)canvas.transform;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, out var localPoint);
            
            return localPoint;
        }
    }
}
