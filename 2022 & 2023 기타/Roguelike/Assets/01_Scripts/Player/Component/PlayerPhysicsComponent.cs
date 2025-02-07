﻿using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerPhysicsComponent : IPlayerComponent
{
    private IObservable<int> levelUpStream;

    private Subject<PlayerData> playerDataStream = new ();

    private Subject<int> playerLevelStream = new ();

    private float maxHp = 10;

    private float hp = 10;

    private float speed = .5f;

    private Vector3 velocity;

    public PlayerPhysicsComponent(GameObject player, PlayerData playerData) : base(player, playerData)
    {

    }

    public override void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.INIT:
                Init();

                break;
            case GameState.STANDBY:
                playerData.hp = playerData.maxhp;
                playerData.level = 0;
                playerDataStream.OnNext(playerData);
                break;
        }
    }

    private void Init()
    {
        GameManager.Instance.GetGameComponent<PlayerComponent>().PlayerMoveSubscribe(playerPosition =>
        {
            var direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            direction *= Time.deltaTime * playerData.speed;
            direction += velocity * Time.deltaTime * 2;

            UpdateTranslate(direction);
        });

        GameManager.Instance.GetGameComponent<EnemyComponent>().EnemyDestorySubscribe(enemy => playerData.level += 0.1f);

        levelUpStream = Observable.EveryUpdate()
            .Select(stream => (int)playerData.level)
            .DistinctUntilChanged();

        player.OnCollisionEnter2DAsObservable().Subscribe(col =>
        {
            if (!col.collider.tag.Equals("Enemy")) return;

            hp --;

            playerDataStream.OnNext(playerData);

            var normalized = (player.transform.position - col.transform.position).normalized;
            DOTween.To(() => normalized, x => velocity = x, Vector3.zero, 1);

            if (hp <= 0)
                GameManager.Instance.UpdateState(GameState.GAMEOVER);

        }).AddTo(GameManager.Instance);
    }

    private void UpdateTranslate(Vector2 direction) { player.transform.Translate(direction); }


    public void PlayerDataSubscribe(Action<PlayerData> action) { playerDataStream.Subscribe(action); }

    public void PlayerLevelUpSubscibe(Action<int> action)
    {
        playerLevelStream.Subscribe(action);
    }
}