using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowEnemyBehaviour : MonoBehaviour
{
    enum EnemyState{Chasing, Stunned, Attacking}

    private EnemyState _currentState;

    void SetState(EnemyState newState)
    {
        if (newState == EnemyState.Stunned && _currentState == EnemyState.Attacking)
        {
            if (_currentTarget == _smallPlayer) _smallPlayer.GetComponent<PlayerController>().RestoreSpeed();
            else _bigPlayer.GetComponent<BeastPlayerController>().RestoreSpeed();
        }

        if (newState == EnemyState.Attacking)
        {
            if (_currentTarget == _smallPlayer) _smallPlayer.GetComponent<PlayerController>().SlowDown();
            else _bigPlayer.GetComponent<BeastPlayerController>().SlowDown();
        }
        
        _currentState = newState;
        _stateTimer = 0;
    }
    
    [SerializeField] private float _baseSpeed = 5f, _maxRange = 25f;
    [Range(0, 1f)][SerializeField] private float _smallPlayerBias = 0f, _bigPlayerBias = .1f;
    [Range(0, 1f)] [SerializeField] private float _changeToPickRandom = .7f, _chanceToKeepTarget = .5f;
    [SerializeField] private float _distanceOffset = 2f, _sinMultiplier = .5f, _sinSpeed = .25f;
    
    private bool _targetInRange = false;
    private float _targetDistance;
    private float _targetDistance2D;
    private float _sinOffset;

    private Vector3 _stunForce;
    private const float StunTime = 2f;

    private Vector3 _attackDirection;

    private Animator _shadowAnimator;
    private Transform _currentTarget;
    private Transform _smallPlayer;
    private Transform _bigPlayer;
    private float _targetRadius;
    private const float BigPlayerRadius = 10f, SmallPlayerRadius = 3f;

    private float _stateTimer = 0f;
    private float _enemyTimer = 0f;
    private float _selectTargetTimestamp = 0f;
    private const float SelectTargetInterval = 1f;
    
    void Start()
    {
        _smallPlayer = PlayerController.Instance.transform;
        _bigPlayer = BeastPlayerController.Instance.transform;
        _shadowAnimator = GetComponentInChildren<Animator>();

        _selectTargetTimestamp = _enemyTimer + SelectTargetInterval;
        
        var coinFlip = MathHelpers.CoinFlip();
        _currentTarget = coinFlip ? _smallPlayer : _bigPlayer;
        _targetRadius = coinFlip ? SmallPlayerRadius : BigPlayerRadius;

        SetState(EnemyState.Chasing);

        _sinOffset = Random.Range(-2f, 2f);
    }
    
    void Update()
    {
        switch (_currentState)
        {
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Stunned:
                Stunned();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }
        
        _enemyTimer += Time.deltaTime;
        _stateTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        _targetInRange = TargetInRange();
    }

    void ChasePlayer()
    {
        if(!_targetInRange)
        {
            Vector3 offset = Vector3.zero;
            offset.y += Mathf.Sin(_sinOffset + _enemyTimer * _sinSpeed * Mathf.PI)*_sinMultiplier;

            transform.position += offset * (_baseSpeed * Time.deltaTime);
            
            return;
        }

        AnimationController.Instance.SetAnimatorBool(_shadowAnimator, "Hit", false);
        AnimationController.Instance.SetAnimatorBool(_shadowAnimator, "Dizzy", false);

        Vector3 playerDir = PlayerDirection();

        Vector3 pos = transform.position;

        float acceleration = Mathf.Clamp01(.3f + .5f * _stateTimer);

        pos += playerDir * (_baseSpeed * Time.deltaTime*acceleration);
        pos.y += Mathf.Sin(_sinOffset + _enemyTimer * _sinSpeed * Mathf.PI) * _sinMultiplier* Time.deltaTime;

        transform.position = pos;
        
        transform.forward = Vector3.RotateTowards(transform.forward, playerDir, 2f*Time.deltaTime*acceleration, 1f);
        
        if(_targetDistance > 0) return;

        _attackDirection = -playerDir;
        SetState(EnemyState.Attacking);
    }

    void Stunned()
    {
        float drag = 5f;
        
        float moveSignZ = Mathf.Sign(_stunForce.z);
        float moveSignY = Mathf.Sign(_stunForce.y);
        float moveSignX = Mathf.Sign(_stunForce.x);

        _stunForce.z -= moveSignZ * Time.deltaTime * drag;
        _stunForce.y -= moveSignY * Time.deltaTime * drag;
        _stunForce.x -= moveSignX * Time.deltaTime * drag;

        transform.position += _stunForce * Time.deltaTime;

        AnimationController.Instance.SetAnimatorBool(_shadowAnimator, "Dizzy", true);

        if (_stateTimer >= StunTime) SetState(EnemyState.Chasing);
    }

    void Attack()
    {
        AnimationController.Instance.SetAnimatorBool(_shadowAnimator, "Walk", false);

        if (_currentState != EnemyState.Stunned)
        {
            AnimationController.Instance.SetAnimatorBool(_shadowAnimator, "Hit", true);
        }

        var targetPos = _currentTarget.position;
        var selfPos = transform.position;
        Vector3 pos = targetPos + _attackDirection * _targetRadius;
        Vector3 playerDir = (targetPos - selfPos).normalized;
        
        transform.position = Vector3.MoveTowards(selfPos, pos, _baseSpeed*5f*Time.deltaTime);
        transform.forward = Vector3.RotateTowards(transform.forward, playerDir, 4f*Time.deltaTime, 1f);
    }
    
    Vector3 PlayerDirection()
    {
        var selfPos = transform.position;
        var targetPos = _currentTarget.position;
        Vector3 dir = (targetPos - selfPos);
        _targetDistance = dir.magnitude - _targetRadius;
        _targetDistance.ClampZero();
        _targetDistance2D = (selfPos.PlanePosition() - targetPos.PlanePosition()).magnitude - _targetRadius;
        _targetDistance2D.ClampZero();

        Vector3 offset = Vector3.zero;
        
        offset.y += (Mathf.Pow(Mathf.Max(_targetDistance2D, 1f), .8f) - 1) * _distanceOffset;

        Vector3 offsetDir = (targetPos - selfPos + offset);

        return offsetDir.normalized;
    }

    bool TargetInRange()
    {
        Vector2 smallPlayer2DPos = _smallPlayer.position.PlanePosition();
        Vector2 bigPlayer2DPos = _bigPlayer.position.PlanePosition();
        Vector2 self2DPos = transform.position.PlanePosition();

        var smallPlayerDist = (self2DPos - smallPlayer2DPos).magnitude - SmallPlayerRadius;
        smallPlayerDist.ClampZero();
        var bigPlayerDist = (self2DPos - bigPlayer2DPos).magnitude - BigPlayerRadius;
        bigPlayerDist.ClampZero();

        var closestDist = 1000f;

        Transform newTarget;
        float newRadius;
        
        //Only target small
        closestDist = smallPlayerDist;
        _currentTarget = _smallPlayer;
        _targetRadius = SmallPlayerRadius;

        if (closestDist > _maxRange) return false;
        else return true;
        //

        if (smallPlayerDist + _bigPlayerBias > bigPlayerDist + _smallPlayerBias)
        {
            closestDist = bigPlayerDist;
            newTarget = _bigPlayer;
            newRadius = BigPlayerRadius;
        }
        else
        {
            closestDist = smallPlayerDist;
            newTarget = _smallPlayer;
            newRadius = SmallPlayerRadius;
        }

        if (closestDist > _maxRange) return false;

        if (!(_enemyTimer >= _selectTargetTimestamp)) return true;
        
        _selectTargetTimestamp = _enemyTimer + SelectTargetInterval;
        
        if (_currentState == EnemyState.Attacking) return true;
        
        if (Random.Range(0, 1f) <= _changeToPickRandom)
        {
            var coinFlip = MathHelpers.CoinFlip();
            _currentTarget = coinFlip ? _smallPlayer : _bigPlayer;
            _targetRadius = coinFlip ? SmallPlayerRadius : BigPlayerRadius;
        }
        else if(!(Random.Range(0, 1f) <= _chanceToKeepTarget))
        {
            _currentTarget = newTarget;
            _targetRadius = newRadius;
        }

        return true;
    }

    public void StunEnemy(Vector3 force)
    {
        AnimationController.Instance.SetAnimatorBool(_shadowAnimator, "Walk", false);
        AnimationController.Instance.SetAnimatorBool(_shadowAnimator, "Hit", false);

        transform.forward = -force.normalized;
        _stunForce = force;
        
        SetState(EnemyState.Stunned);
    }
}
