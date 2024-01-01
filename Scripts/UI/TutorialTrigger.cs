using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private sbyte _priority;
    [SerializeField] private UIManager.TutorialType _tutorialType;
    [SerializeField] private float _displayTime = 10f;
    [SerializeField] private InteractionObject.PlayerType _playerType;
    private GameObject _player;

    private void Start()
    {
        if (_playerType == InteractionObject.PlayerType.smallPlayer) _player = PlayerController.Instance.gameObject;
        else _player = BeastPlayerController.Instance.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_player != other.gameObject) return;
		
        UIManager.Instance.ShowTutorial(_tutorialType, _displayTime, _priority);
    }
}
