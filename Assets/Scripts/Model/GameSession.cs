using UnityEngine;
using MainNameSpace.Model.Data;
namespace MainNameSpace.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;
        public PlayerData data => _data;
        private PlayerData _save;

        private void Awake()
        {
            if (IsSessionExit())
            {
                Destroy(gameObject);
            }
            else
            {
                Save();
                DontDestroyOnLoad(this);
            }
        }

        public void Save()
        {
            _save = data.Clone();
        }
        public void LoadLastSave()
        {
            _data = _save.Clone();
        }

        private bool IsSessionExit()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var gameSession in sessions)
            {
                if (gameSession != this)
                    return true;
            }
            return false;
        }
    }
}

