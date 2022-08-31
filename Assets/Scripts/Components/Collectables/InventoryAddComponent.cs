using MainNameSpace.Creature.Character;
using UnityEngine;
namespace MainNameSpace.components.Collectables
{
    public class InventoryAddComponent : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private int _count;

        public void Add(GameObject go)
        {
            var character = go.GetComponent<Character>();
            if (character != null) character.AddInInventory(_id, _count);
        }
    }
}

