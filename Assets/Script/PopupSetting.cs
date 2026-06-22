using UnityEngine;

public class PopupSetting : MonoBehaviour
{
    public GameObject popupSetting;

    private void Start()
    {
        popupSetting.SetActive(false);
    }

    public void OpenPopup()
    {
        popupSetting.SetActive(true);
    }

    public void ClosePopup()
    {
        popupSetting.SetActive(false);
    }
}