using Photon.Realtime;
using Photon.Pun;
//using UnityEditorInternal;
using UnityEngine;

public class RoomListItems : MonoBehaviourPunCallbacks
{
    [SerializeField] TMPro.TextMeshProUGUI text;
    public RoomInfo info;
    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
