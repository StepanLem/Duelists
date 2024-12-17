using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerDataRecorderInstaller : MonoInstaller
{
    [SerializeField] private PlayerRecorder _playerRecorder;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerDataInterpolator>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerDataSerializer>().AsSingle();
        Container.BindInstance(_playerRecorder).AsSingle();
    }
}
