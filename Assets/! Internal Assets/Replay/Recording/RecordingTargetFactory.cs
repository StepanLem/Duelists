 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

public class RecordingTargetFactory : IRecordingTargetFactory
{
    private readonly DiContainer _container;
    public RecordingTargetFactory(DiContainer container)
    {
        _container = container;
    }
    public IRecordingTarget<T> CreateTarget<T>(Func<T> valueGetter)
    {
        var interpolator = _container.Resolve<IInterpolator<T>>();
        var serializer = _container.Resolve<IBinarySerializer<T>>();
        return new RecordingTarget<T>(valueGetter, interpolator, serializer);
    }
}