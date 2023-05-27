using UnityEngine;

public class UtilityFunctions : MonoBehaviour
{
    private static UtilityFunctions instance;

    public static UtilityFunctions Instance
    {
        get
        {
            // Check if the instance is null
            if (instance == null)
            {
                // Find the existing instance in the scene
                instance = FindObjectOfType<UtilityFunctions>();

                // If no instance exists, create a new one
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UtilityFunctions).Name);
                    instance = singletonObject.AddComponent<UtilityFunctions>();
                }
            }

            return instance;
        }
    }

    public float FindRightScreenBorder()
    {
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        return mainCamera.transform.position.x + cameraWidth / 2f;
    }

    public float FindLeftScreenBorder()
    {
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        return mainCamera.transform.position.x - cameraWidth / 2f;
    }

    public void DestroyPlatform(){
        
    }
}
