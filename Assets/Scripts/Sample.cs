using UnityEngine;
using System.Linq;

public class DemoObject : MonoBehaviour
{
    private Color[] PLAYER_COLOR = new Color[] { Color.white, Color.red, Color.green, Color.blue, Color.yellow };

    public void SetPlayerColor(int i_colorIndex)
    {
        var properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("Color", i_colorIndex);
        PhotonNetwork.player.SetCustomProperties(properties);
    }

    public void OnPhotonPlayerPropertiesChanged(object[] i_playerAndUpdatedProps)
    {
        var player = i_playerAndUpdatedProps[0] as PhotonPlayer;
        var properties = i_playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;


        object colorValue = null;
        if (properties.TryGetValue("Color", out colorValue))
        {
            int colorIndex = (int)colorValue;

            // ゲーム上のPlayer用のオブジェクトの中からPhotonViewのIDが変更したPlayerと同じオブジェクトの色を変更する。
            var playerObjects = GameObject.FindGameObjectsWithTag("Player");
            var playerObject = playerObjects.FirstOrDefault(obj => obj.GetComponent<PhotonView>().ownerId == player.ID);
            playerObject.GetComponent<Renderer>().material.color = PLAYER_COLOR[colorIndex];
            return;
        }
    }
} // class DemoObject