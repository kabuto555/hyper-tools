using Cysharp.Threading.Tasks;

namespace HyperTools
{
    public interface ICommandTask<T>
    {
        public T Receiver { get; set; }

        public UniTask ExecuteCommand();
    }
}
