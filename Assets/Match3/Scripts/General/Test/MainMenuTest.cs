

using General.InspectorButton;
using Persistance;
using Systems;
using UnityEngine;

namespace General.Test
{
    public class MainMenuTest : MonoBehaviour
    {
        [InspectorButton("DeleteFile")]
        public bool delete;
        private void DeleteFile()
        {
            ServiceLocator.Instance.Get<ISaveSystem>().Delete();
        }
    }
}
