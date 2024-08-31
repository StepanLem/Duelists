using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySceneInstaller : MonoInstaller
{
    [SerializeField]
    private Enemy _enemy;

    public override void InstallBindings()
    {
        this.Container
            .Bind<Enemy>()
            .FromInstance(_enemy)
            .AsSingle();
    }
}
