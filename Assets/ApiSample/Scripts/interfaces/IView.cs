using System;
using UnityEngine;

namespace Graffity.Groq.Sample.Common
{
    public interface IViewProperty : IDisposable 
    { }

    public interface IView<TViewProperty> where TViewProperty : IViewProperty 
    {
        void Bind(TViewProperty property);
    }
}
