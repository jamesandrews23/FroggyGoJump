using UnityEngine;

namespace _Scripts.Game
{
    public class UtilityFunctions : MonoBehaviour
    {
        private static UtilityFunctions _instance;

        public static UtilityFunctions Instance
        {
            get
            {
                // Check if the instance is null
                if (_instance != null) return _instance;
                // Find the existing instance in the scene
                _instance = FindObjectOfType<UtilityFunctions>();

                // If no instance exists, create a new one
                if (_instance != null) return _instance;
                var singletonObject = new GameObject(nameof(UtilityFunctions));
                _instance = singletonObject.AddComponent<UtilityFunctions>();

                return _instance;
            }
        }

        public static float FindRightScreenBorder()
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                var cameraHeight = 2f * mainCamera.orthographicSize;
                var cameraWidth = cameraHeight * mainCamera.aspect;
                return mainCamera.transform.position.x + cameraWidth / 2f;
            }
            else
            {
                return 0f;
            }
        }

        public static float FindLeftScreenBorder()
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                var cameraHeight = 2f * mainCamera.orthographicSize;
                var cameraWidth = cameraHeight * mainCamera.aspect;
                return mainCamera.transform.position.x - cameraWidth / 2f;
            }
            else
            {
                return 0f;
            }
        }
    }
}
