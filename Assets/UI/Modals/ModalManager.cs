using UnityEngine;

namespace Assets.UI.Modals
{
    public class ModalManager : MonoBehaviour
    {
        public virtual void OnCancel()
        {
            Destroy(gameObject);
        }

        public virtual void OnConfirm()
        {
            Destroy(gameObject);
        }
    }
}
