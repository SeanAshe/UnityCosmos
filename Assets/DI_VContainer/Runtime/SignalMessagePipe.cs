using VContainer;
using VContainer.Unity;
using MessagePipe;
using System;

namespace Cosmos.DI
{
    public abstract class BaseSingal<T>: IDisposable
    {
        [Inject] internal readonly IPublisher<Guid, T> publisher;
        private Guid guid;
        private event Action<T> Listener = delegate { };
        public void AddListener(Action<T> callback) => Listener += callback;
        public void RemoveListener(Action<T> callback) => Listener -= callback;
        public void RemoveAllListeners() => Listener = delegate { };
        public void Bind(BaseCommand<T> command) => guid = command.guid;
        void IDisposable.Dispose() => Listener = delegate { };
        public void Dispatch(T message)
        {
            publisher.Publish(guid, message);
            Listener(message);
        }

    }
    public abstract class BaseCommand<T> : IStartable, IDisposable
    {
        [Inject] internal readonly ISubscriber<Guid, T> subscriber;
        private readonly DisposableBagBuilder bag;
        internal readonly Guid guid;
        private IDisposable disposable;
        public BaseCommand()
        {
            guid = Guid.NewGuid();
            bag = DisposableBag.CreateBuilder();
        }
        void IStartable.Start()
        {
            subscriber.Subscribe(guid, Execute).AddTo(bag);
            disposable = bag.Build();
        }
        void IDisposable.Dispose() => disposable.Dispose();
        public abstract void Execute(T message);
    }
}