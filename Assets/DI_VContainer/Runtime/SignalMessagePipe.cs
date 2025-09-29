using VContainer;
using VContainer.Unity;
using MessagePipe;
using System;


namespace Cosmos.DI
{
    public abstract class BaseSingal<TCommand, T> where TCommand : BaseCommand<T>
    {
        [Inject]
        readonly IPublisher<T> publisher;
        [Inject]
        readonly TCommand command;
        public BaseSingal()
        {
            command.Initialize();
        }

        public void Dispatch(T message)
        {
            publisher.Publish(message);
        }
    }

    public abstract class BaseCommand<T> : IDisposable
    {
        [Inject]
        ISubscriber<T> subscriber;
        IDisposable disposable;
        DisposableBagBuilder bag;
        public void Initialize()
        {
            bag = DisposableBag.CreateBuilder();
            subscriber.Subscribe(Execute).AddTo(bag);
            disposable = bag.Build();
        }
        public abstract void Execute(T message);
        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}