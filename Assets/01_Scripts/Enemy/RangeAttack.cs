using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : EnemyAttack
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private float _playTime = 3f;
    private Action<bool> CallBack = null;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = _brain.GetComponent<SpriteRenderer>();
    }

    public override void Attack(Action<bool> Callback)
    {
        _spriteRenderer.color = Color.red;
        this.CallBack = Callback;
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(_playTime);
        _spriteRenderer.color = Color.white;
        CallBack(true);
    }
}
