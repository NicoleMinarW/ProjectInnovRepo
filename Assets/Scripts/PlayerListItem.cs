using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerListItem : MonoBehaviourPunCallbacks
{
    Player player;
    [SerializeField] TMPro.TextMeshProUGUI text;
    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
