using UnityEngine;

namespace _Scripts.Game.Player
{
    public interface ITongueAttachable
    {
        /// <summary>
        /// Returns the world-space point the tongue should attach to.
        /// hitPoint is the Physics2D raycast hit point.
        /// </summary>
        Vector2 GetAttachPoint(Vector2 hitPoint);
    }
}