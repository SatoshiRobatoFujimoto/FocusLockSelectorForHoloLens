using FocusLockable;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class TestListener : MonoBehaviour, IInputClickHandler, IFocusLockable
{
    [SerializeField]
    private TextMesh _tMesh;

    public void OnFocusLocked()
    {
        Debug.Log("Focus Locked");
        _tMesh.text = gameObject.name + ": Focus locked";
    }

    public void OnFocusReleased()
    {
        Debug.Log("Focus Released");
        _tMesh.text = gameObject.name + ": Focus released";
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Air Tap");
        _tMesh.text = gameObject.name + ": Tapped";
    }
}
