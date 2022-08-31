using MainNameSpace.Model;
using MainNameSpace.Utils;
using MainNameSpace.components.ColliderBased;
using MainNameSpace.components.Health;
using UnityEngine;
using UnityEditor.Animations;
namespace MainNameSpace.Creature.Character
{
    public class Character : Creature
    {
       // [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private Vector3 _groundCheckPositionDelta;
        [SerializeField] private float _groundCheckRadius;
        [SerializeField] private LayerCheck _wallCheck;

        [SerializeField] private Cooldown _throwCooldown;
        [SerializeField] private AnimatorController _armedAnimController;
        [SerializeField] private AnimatorController _disArmedAnimContoller;

        [SerializeField] private CheckCircleOverlap _interactionCheck;
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactLayer;



        [Space]
        [Header("Particles")]
        [SerializeField] private ParticleSystem _hitParticles;
        

        private bool _allowDoubleJump;
        private GameSession _session;
        private bool _isOnWall;
        private float _defalutGravityScale;

        private static readonly int ThrowAttackKey = Animator.StringToHash("ThrowSword");
        private static readonly int IsOnWallKey = Animator.StringToHash("IsOnWall");
        private int CoinsCount => _session.data.Inventory.Count("Coin");
        private int SwordCount => _session.data.Inventory.Count("Sword");

        protected override void Awake()
        {
            base.Awake();
            _defalutGravityScale = Rigidbody2D.gravityScale;
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            var health = GetComponent<HealthComponent>();
            health.SetHealth(_session.data.Health);
            _session.data.Inventory.OnChanged += OnInventoryChanged;
            UpdateHeroWeapon();
        }

        private void OnInventoryChanged(string id, int value)
        {
            if (id == "Sword") UpdateHeroWeapon();
        }

        protected override void Update()
        {
            base.Update();
            var moveToSameDirection = CurrentDirection.x * transform.lossyScale.x > 0;
            if (_wallCheck.IsTouchingLayer && moveToSameDirection)
            {
                _isOnWall = true;
                Rigidbody2D.gravityScale = 2;
            }
            else
            {
                _isOnWall = false;
                Rigidbody2D.gravityScale = _defalutGravityScale;
            } return;

            Animator.SetBool(IsOnWallKey, _isOnWall);
        }
        public void OnHealthChanged(int currentHealth)
        {
            _session.data.Health = currentHealth;
        }

        protected override float CalculateYVelocity()
        {
            var yVelocity = Rigidbody2D.velocity.y;
            var IsJumpPressing = CurrentDirection.y > 0;

            if (IsGrounded)
            {
                _allowDoubleJump = true;
            }

            if (!IsJumpPressing && _isOnWall)
            {
                return 0f;
            }
            return base.CalculateYVelocity();
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
        if (!IsGrounded && _allowDoubleJump)
            {
                _particles.Spawn("DoubleJump");
                _allowDoubleJump = false;
                return JumpForce;
            }

            return base.CalculateJumpVelocity(yVelocity);
        }

        public void AddInInventory(string id, int value)
        {
            _session.data.Inventory.Add(id, value);
        }
        public override void TakeDamage()
        {
            base.TakeDamage();
            if (CoinsCount > 0) SpawnCoins();
        }
        public void Heal()
        {
//
        }
        private void SpawnCoins()
        {
            var numCoinsToSpawn = Mathf.Min(CoinsCount, 5);
            _session.data.Inventory.Remove("Coin", numCoinsToSpawn);

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToSpawn;
            _hitParticles.emission.SetBurst(0, burst);
            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void Interact()
        {
            _interactionCheck.Check();
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.IsInLayer(GroundLayer))
            {
                var contact = other.contacts[0];
                if(contact.relativeVelocity.y >= SlamDownVelocity)
                {
                    _particles.Spawn("SlamDown");
                }
/*                //FallDamage        if (contact.relativeVelocity.y >= _slamDownVelocity)
                {
                    GetComponent<HealthComponent>().ModifyHealth(-1);
                }*/
            }

        }
        public override void Attack()
        {
            if (SwordCount <= 0) return;
            base.Attack();
            _particles.Spawn("SwordAttackParticle");
            Debug.Log($"Melee Attack");

        }

        private void UpdateHeroWeapon()
        {
            Animator.runtimeAnimatorController = SwordCount > 0 ? _armedAnimController : _disArmedAnimContoller;
        }

        public void OnThrowAttack()
        {
            _particles.Spawn("ThrowSword");
        }
        public void Throw()
        {
            if (_throwCooldown.IsReady)
            {
                Animator.SetTrigger(ThrowAttackKey);
                _throwCooldown.Reset();
               // _session.data.Inventory.Remove("Sword", 1);
            }
        }
        private void OnDestroy()
        {
            _session.data.Inventory.OnChanged -= OnInventoryChanged;
        }
    }

}

