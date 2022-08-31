using UnityEngine;
namespace MainNameSpace.components.ColliderBased
{
    public class LayerCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        [SerializeField] private bool _IsTouchingLayer;
        private Collider2D _collider;
        public bool IsTouchingLayer => _IsTouchingLayer;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            _IsTouchingLayer = _collider.IsTouchingLayers(_layer);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _IsTouchingLayer = _collider.IsTouchingLayers(_layer);
        }
    }
}

