using UnityEngine;

namespace _Scripts.Game.InputControl
{
    public class InputController : MonoBehaviour
    {
        public RaycastHit2D GetTouchHit()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended)
                {
                    Vector2 touchPosition = touch.position;
                    Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                    return hit;
                }
            }

            return new RaycastHit2D();
        }
    }
}