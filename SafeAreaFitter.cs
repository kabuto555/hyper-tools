using UnityEngine;

namespace HyperTools
{
    /*
     * Resize it's associated RectTransform, so any children can use anchoring to fit within a safe area without
     * things like hardware notches or severely rounded corners
     */
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private bool updateInEditMode;
        private RectTransform RectTransform => transform as RectTransform;
        private Rect _lastSafeAreaRect;

        private void Awake()
        {
            FitToSafeArea();
        }

        private void Update()
        {
            if (_lastSafeAreaRect != Screen.safeArea)
            {
                FitToSafeArea();
            }
        }

        private void FitToSafeArea()
        {
            if (Application.isEditor && !updateInEditMode && !Application.isPlaying)
            {
                return;
            }
            
            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            RectTransform.anchorMin = anchorMin;
            RectTransform.anchorMax = anchorMax;
        }
    }
}
