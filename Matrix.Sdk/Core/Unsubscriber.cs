namespace Matrix.Sdk.Core
{
    using System;
    using System.Collections.Generic;

    internal class Unsubscriber<T> : IDisposable
    {
        private readonly IObserver<T> _observer;
        private readonly List<IObserver<T>> _observers;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}