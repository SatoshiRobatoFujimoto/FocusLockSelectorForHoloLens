using UnityEngine.EventSystems;

namespace FocusLockable
{
    public interface IFocusLockable : IEventSystemHandler
    {
        void OnFocusLocked();
        void OnFocusReleased();
    }
}