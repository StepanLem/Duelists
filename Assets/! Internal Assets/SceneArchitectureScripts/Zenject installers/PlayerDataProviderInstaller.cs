using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerDataProviderInstaller : MonoInstaller
{
    [SerializeField] private PlayerDataProvider _playerDataProvider;
    public override void InstallBindings()
    {
        Container.BindInstance<IPlayerDataProvider>(_playerDataProvider).AsSingle();
    }
}