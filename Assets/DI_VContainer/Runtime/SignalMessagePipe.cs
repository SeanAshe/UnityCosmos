using VContainer;
using VContainer.Unity;
using MessagePipe;
using System;


namespace Cosmos.DI
{
    public abstract class BaseSingal<T>
    {
        [Inject]
        readonly IPublisher<T> publisher;
        public void Dispatch(T message) => publisher.Publish(message);
    }
    public abstract class BaseCommand<T> : IStartable, IDisposable
    {
        [Inject]
        readonly ISubscriber<T> subscriber;
        private readonly DisposableBagBuilder bag;
        private IDisposable disposable;
        public BaseCommand()
        {
            bag = DisposableBag.CreateBuilder();
        }
        void IStartable.Start()
        {
            subscriber.Subscribe(Execute).AddTo(bag);
            disposable = bag.Build();
        }
        void IDisposable.Dispose() => disposable.Dispose();
        public abstract void Execute(T message);
    }
}