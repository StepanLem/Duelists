using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IInterpolator<T>
{
    public T Lerp(T start, T end, double t);
}
