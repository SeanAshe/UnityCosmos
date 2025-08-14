
namespace Cosmos.unity
{
    public interface ISystem
    {
        void Initialize();
    }
    public class SystemManager : MonoSingleton<SystemManager>
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
