using Input;
using UnityEngine;

public class LockPuzzleButton : InteractionObject
{
    private bool _hasBeenSpawned = false;

    [SerializeField] private GameObject _lockPuzzlePrefab;

    private void Update()
    {
        if(!playerInRange) return;

        if (_hasBeenSpawned) return;
        
        _lockPuzzlePrefab.SetActive(true);
        _lockPuzzlePrefab.GetComponentInChildren<LockPuzzle>().enabled = true;
        _hasBeenSpawned = true;
    }
}
