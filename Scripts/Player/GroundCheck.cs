using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerController _player;
    private BeastPlayerController _beastPlayer;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
        _beastPlayer = GetComponentInParent<BeastPlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger && other.gameObject.layer != LayerMask.NameToLayer("Barrier"))
        {
            if(_player)_player.OnGround = true;
            if (_beastPlayer) _beastPlayer.OnGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            if(_player)_player.OnGround = false;
            if (_beastPlayer) _beastPlayer.OnGround = false;
        }
    }
}
