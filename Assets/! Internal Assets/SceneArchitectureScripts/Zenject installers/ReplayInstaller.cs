using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;
using UnityEngine;

public class ReplayInstaller : MonoInstaller
{
    [SerializeField] private UpdateTicker UpdateTicker;
    [SerializeField] private FixedUpdateTicker FixedUpdateTicker;

    public override void InstallBindings()
    {
        Container.BindInstance(UpdateTicker).AsSingle();
        Container.BindInstance(FixedUpdateTicker).AsSingle();
        Container.BindInstance<IRecordingTargetFactory>(new RecordingTargetFactory(Container)).AsSingle();
        Container.BindInterfacesAndSelfTo<PlaybackTargetFactory>().AsSingle();
    }
}