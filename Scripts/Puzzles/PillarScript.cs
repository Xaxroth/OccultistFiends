using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;

public class PillarScript : MonoBehaviour
{
    [SerializeField] private PillarPullManager _pillarPullManager;
    private PlayerController _player;

    private InputManager _inputManager;

    [SerializeField] private bool _isTimed;
    [SerializeField] private float _timeUntilReset = 10;
    public bool _pillarDestroyed;

    [SerializeField] private ParticleSystem _fireBurst;
    [SerializeField] private ParticleSystem _fire1;
    [SerializeField] private ParticleSystem _fire2;
    [SerializeField] private ParticleSystem _fire3;

    [SerializeField] private SkullPillarScript _skullPillar;

    [SerializeField] private GameObject _associatedSpawner;

    private void Awake()
    {
        _inputManager = InputManager.Instance;
        _pillarPullManager.AllPillars.Add(gameObject);
    }
    void Start()
    {
        _player = PlayerController.Instance;
    }

    void Update()
    {
        var input = _inputManager.GetInputData(1);

        if (_player.Gripping && _player._grippingObject == gameObject && !_pillarDestroyed && input.CheckButtonPress(ButtonMap.PullBack))
        {
            AnimationController.Instance.SetAnimatorBool(PlayerController.Instance.CultistAnimator, "Hanging", false);
            AudioManager.Instance.PlaySoundWorld("Puzzle", transform.position, 15f);
            EffectsManager.Instance.SpawnParticleEffect("PillarExplode", transform.position, Quaternion.identity);
            _skullPillar.DestroyPillar();
            _pillarDestroyed = true;
            _pillarPullManager.AllPillars.Remove(gameObject);
            if (_isTimed)
            {
                StartCoroutine(ResetPillar());
            }
            else
            {
                if (_associatedSpawner != null)
                {
                    _associatedSpawner.SetActive(false);
                }

                if (TryGetComponent(out Collider col))
                {
                    col.enabled = false;
                }
            }
        }

        if (_pillarPullManager.AllPillars.Count <= 0)
        {
            _fire1.Play();
        }
    }

    private IEnumerator ResetPillar()
    {
        _fireBurst.Play();
        _fire1.Play();

        yield return new WaitForSeconds(_timeUntilReset / 3);

        _fire1.Stop();
        _fire2.Play();

        yield return new WaitForSeconds(_timeUntilReset / 3);

        _fire2.Stop();
        _fire3.Play();

        yield return new WaitForSeconds(_timeUntilReset / 3);
        if (_pillarPullManager.AllPillars.Count <= 0) yield break;
        
        _fire3.Stop();
        _skullPillar.RestorePillar();
        EffectsManager.Instance.SpawnParticleEffect("PillarReset", transform.position, Quaternion.identity);
        _pillarPullManager.AllPillars.Add(gameObject);
        _pillarDestroyed = false;
    }
}
