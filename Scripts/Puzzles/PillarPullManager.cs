using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarPullManager : MonoBehaviour
{
    public List<GameObject> AllPillars = new List<GameObject>();
    public GameObject BigWall;

    public GameObject Exit;

    [SerializeField] private bool _sectionThree;
    private bool _solved;

    private void Awake()
    {
        Exit.SetActive(false);
    }

    void Update()
    {
        if (AllPillars.Count == 0)
        {
            if (!_sectionThree && !_solved)
            {
                BigWall.SetActive(false);
                DialogueManagerScript.Instance.Event8();
                BeastPlayerController.Instance._currentPlayerSpeed = 20;
                AudioManager.Instance.PlaySound("PuzzleAlert", 0, 1, 0);
            }

            if (_sectionThree && !_solved)
            {
                Exit.SetActive(true);
            }

            _solved = true;
        }
    }
}
