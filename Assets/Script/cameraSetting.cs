using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class cameraSetting : NetworkBehaviour
{
    // [ServerRpc]
    // private void AutoTrakingPlayerServerRpc()
    // {
    //     var cam = GetComponent<CinemachineCamera>();
    //     var player = GameObject.FindWithTag("Player");
    //     if (player != null)
    //     {
    //         cam.Target.TrackingTarget = player.transform;
    //         cam.Target.LookAtTarget = player.transform;
    //     }
    // }
}
