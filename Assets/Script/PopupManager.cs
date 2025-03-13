using UnityEngine;

public class PopupMenuController : MonoBehaviour
{
    public GameObject popupMenu; // Assign your PopupMenu in the Inspector

    public void ToggleMenu()
    {
        popupMenu.SetActive(!popupMenu.activeSelf);
    }
}
