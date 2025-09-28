using UnityEngine;

namespace Cosmos.Unity
{
    public interface IGamePlayModel
    {
        /// <summary>
        /// 不能在初始化的时候依赖其他Model
        /// </summary>
        void Initialize();
    }
}
