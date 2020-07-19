using UnityEngine;

namespace Assets.UI.Modals
{
    /// <summary>
    /// Behavior placed on the object under which all modals should be instantiated
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ModalContainer : MonoBehaviour
    {
        public static ModalContainer instance;

        public void Awake()
        {
            if(instance != null)
            {
                Debug.LogError("multiple modal container instances registered");
            }
            instance = this;
        }
    }
}
