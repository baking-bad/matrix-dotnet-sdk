namespace Matrix.Sdk.Core
{
    using System;
    using System.Collections.Generic;

    public class MatrixEventNotifier<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> _observers = new();

        public IDisposable? Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber<T>(_observers, observer);
        }

        public void NotifyAll(T matrixEvent)
        {
            foreach (IObserver<T> eventObserver in _observers)
                eventObserver.OnNext(matrixEvent);
        }
    }
}