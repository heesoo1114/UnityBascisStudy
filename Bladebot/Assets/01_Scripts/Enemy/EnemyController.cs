using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : PoolAbleMono
{
    [SerializeField]
    private CommonAIState _currentState;

    [SerializeField]
    protected EnemyDataSO _enemyData;
    public EnemyDataSO EnemyData => _enemyData;

    protected EnemyHealth _enemyHealth;
    public EnemyHealth EnemyHealth => _enemyHealth;

    private Transform _targetTrm;
    public Transform TargetTrm => _targetTrm;

    private NavAgentMovement _navMovement;
    public NavAgentMovement NavMovement => _navMovement;

    private AgentAnimator _agentAnimator; 
    public AgentAnimator AgentAnimator => _agentAnimator;

    private EnemyVFXManager _vfxManager;
    public EnemyVFXManager VFXManager => _vfxManager;

    private CommonAIState _initState;
    private AIActionData _actionData;

    protected virtual void Awake()
    {
        List<CommonAIState> states = new List<CommonAIState>();
        transform.Find("AI").GetComponentsInChildren<CommonAIState>(states);

        states.ForEach(s => s.SetUp(transform));

        _navMovement = GetComponent<NavAgentMovement>();
        _agentAnimator = transform.Find("Visual").GetComponent<AgentAnimator>();
        _vfxManager = GetComponent<EnemyVFXManager>();
        _enemyHealth = GetComponent<EnemyHealth>();

        _actionData = transform.Find("AI").GetComponent<AIActionData>();

        _initState = _currentState;
    }

    protected virtual void Start()
    {
        _targetTrm = GameManager.Instance.PlayerTrm;
        // 나중에 직접 오버랩 스피어로 변경가능

        _navMovement.SetInitData(_enemyData.MoveSpeed);
        _enemyHealth.SetMaxHP(_enemyData.MaxHP);
    }

    public void ChangeState(CommonAIState nextState)
    {
        // 스테이트 변경
        _currentState?.OnExitState();
        _currentState = nextState;
        _currentState?.OnEnterState();
    }

    private void Update()
    {
        if (_enemyHealth.IsDead) return;
        _currentState?.UpdateState();
    }

    public UnityEvent OnAfterDead = null;

    public void SetDead()
    {
        _navMovement.StopNavigation();
        _agentAnimator.StopAnimator(true);
        _navMovement.KnockBack(() =>
        {
            _agentAnimator.StopAnimator(false);
            _agentAnimator.SetDead();
            UtilMono.Instance.AddDelayCoroutine(() =>
            {
                OnAfterDead?.Invoke();
            }, 1f);
        });
    }

    public override void Init()
    {
        _enemyHealth.SetMaxHP(EnemyData.MaxHP);
        _navMovement.ResetNavAgent();
        ChangeState(_initState);
        _actionData.Init(); // 액션데이터도 초기화
    }

    public void GotoPool()
    {
        PoolManager.Instance.Push(this);
    }
}
