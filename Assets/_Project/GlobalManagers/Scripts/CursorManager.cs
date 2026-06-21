using UnityEngine;

namespace TwinStickShooter.GlobalManagers
{
    public class CursorManager : MonoBehaviour
    {
        #region MonoBehaviour

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        #endregion
    }
}