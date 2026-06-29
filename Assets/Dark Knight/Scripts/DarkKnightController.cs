using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TealFalconEnemySeries{

    public class DarkKnightController : MonoBehaviour
    {
        //Movement Settings
        public float currentSpeed = 0f;
        public float animationSpeed = 2f;
        public float acceleration = 4.8f;
        public float MaxWalkSpeed = 2f;
        public float MaxRunSpeed = 7f;
        public float BackStepPower = 200f;
        public float JumpForce = 2f; // Kekuatan Loncat
        public bool movingRight = true;

        // Components
        private Animator _animator = null;
        private Rigidbody2D _rigidBody = null;  
        public Material _matRef = null;
        private Material instanceMaterial = null;
        private Vector3 deathPlace;
        private MaterialPropertyBlock mpb;

        // Config
        public bool block = false;
        private bool isGrounded = true; // Cek apakah di tanah

        public enum MovementState {
            Idle,
            Walking,
            Running,
        }

        public enum FightingState {
            OnGuard,
            Attacking,
            Hurt,
            Death,
            Move,
            Idle
        }

        public MovementState CurrentMovementState = MovementState.Idle;
        public FightingState CurrentFightingState = FightingState.Idle;

        public UnityEvent OnHurt, OnDeath, OnCharged;
        
        // Color Settings
        public Color GlowColor;
        
        // Death Settings
        public Color DissolveColor;
        public GameObject ExplosionEffect;
        public Transform ExplosionRef;
        public float DissolveSpeed = 1f;
        private float DissolveStatus = 1f;
        public bool destroy = false;

        public float power = 10.0f;        // Explosion Power

        private Transform BeamAttackRef = null;
        public GameObject DarkBall;

        public List<SpriteRenderer> SRList = null;
        public List<Rigidbody2D>    RBList = null;

        public AudioSource _Channel = null;
        public AudioClip BeamSound, DeathExplosionSound, FootStepSound, PainSound, PowerLoadSound, SwordSound;

        // FootStep 
        private float stepTimer = 0f;
        public float baseStepSpeed = 3f;
        public float minStepSpeed = 0.1f;

        void Awake()
        {
           SetComponents();
        }

        void OnEnable()
        {
           SetComponents();
        }

        void SetComponents(){
            if(_animator == null)
                _animator = transform.Find("Root").GetComponent<Animator>();

            if(_rigidBody == null)
                _rigidBody = GetComponent<Rigidbody2D>();

            if(_matRef == null)
                Debug.LogWarning("NO Material Ref Setted!!!");

            if(SRList == null || SRList.Count == 0)
                SRList = GetAllSpriteRenderersInChildren(transform);

            if(RBList == null || RBList.Count == 0)
                RBList = GetAllRigidbodiesInChildren(transform);
            
            if(BeamAttackRef == null)
                BeamAttackRef = transform.Find("Root/Head_Pivot/BeamAttackRef");

            if(IsBuiltIn(_matRef))
                return;

            mpb = new MaterialPropertyBlock();

            if(_matRef.GetTexture("_MainTex") != null)
                mpb.SetTexture("_MainTex", _matRef.GetTexture("_MainTex"));

            mpb.SetTexture("_MagentaPNG",_matRef.GetTexture("_MagentaPNG"));
            mpb.SetTexture("_NormalMap",_matRef.GetTexture("_NormalMap"));
            mpb.SetTexture("_Emission",_matRef.GetTexture("_Emission"));
            mpb.SetFloat("_DissolveScale",_matRef.GetFloat("_DissolveScale"));
            mpb.SetColor("_Glow",GlowColor);
            mpb.SetColor("_DissolveColor",DissolveColor);

            ApplyChanges();
        }

        private void ApplyChanges(){
               if(IsBuiltIn(_matRef))
                return;

                foreach (SpriteRenderer sr in SRList)
                {
                    sr.SetPropertyBlock(mpb);
                }
        }

        void Update(){
            if(CurrentFightingState == FightingState.Death){
                DissolveStatus = Mathf.MoveTowards(DissolveStatus, 0f, DissolveSpeed * Time.deltaTime);

                if(!IsBuiltIn(_matRef))
                    mpb.SetFloat("_Dissolve",DissolveStatus);

                ApplyChanges();

                if(DissolveStatus == 0 && destroy)
                    Destroy(gameObject);
                
                return; // Berhenti jika mati
            }

            // --- INPUT USER UNTUK GERAKAN (MAJU, MUNDUR, LONCAT, SERANG) ---
            HandleInput();

            if(CurrentFightingState != FightingState.Idle && CurrentFightingState != FightingState.OnGuard)
                return;

            float targetSpeed = 0f;

            // Tentukan Kecepatan Berdasarkan State Gerakan
            switch(CurrentMovementState) {
                case MovementState.Idle:
                    targetSpeed = 0f;
                    break;
                case MovementState.Walking:
                    targetSpeed = MaxWalkSpeed;
                    break;
                case MovementState.Running:
                    targetSpeed = MaxRunSpeed;
                    break;
            }

            // Mengikuti arah hadap karakter (movingRight)
            float directionSign = movingRight ? 1f : -1f;
            
            // Mengatur akselerasi kecepatan horizontal
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed * directionSign, acceleration * Time.deltaTime);

            _animator.SetFloat("Speed", Mathf.Abs(currentSpeed) / animationSpeed);
            //_animator.SetBool("IsGrounded", isGrounded); // Kirim parameter tanah ke Animator
            
            // Terapkan kecepatan ke Rigidbody2D (Menjaga kecepatan Y saat loncat)
            _rigidBody.linearVelocity = new Vector2(currentSpeed, _rigidBody.linearVelocity.y);

            // Audio Langkah Kaki
            float stepSpeed = Mathf.Lerp(baseStepSpeed, minStepSpeed, Mathf.Abs(currentSpeed) / MaxRunSpeed);
            stepTimer += Time.deltaTime;

            if(stepTimer >= stepSpeed && Mathf.Abs(currentSpeed) > 0.1f && isGrounded){
                PlaySound(FootStepSound);
                stepTimer = 0f;
            }
        }

        // Fungsi membaca input (Bisa diganti dengan AI script jika ini untuk musuh)
        // Fungsi membaca input (Mendukung PC/Keyboard & Mobile UI)
        private void HandleInput()
        {
            // Ambil data input sekali tekan di awal
            bool mobileJump = MobileInput.GetJumpPressed();
            bool mobileAttack = MobileInput.GetAttackPressed();

            // PENTING: Jangan terima input gerak jika sedang menyerang atau terluka
            if (CurrentFightingState == FightingState.Attacking || CurrentFightingState == FightingState.Hurt) return;

            // 1. Gerak Maju / Mundur
            float moveInput = Input.GetAxisRaw("Horizontal"); 
            
            if (moveInput == 0)
            {
                moveInput = MobileInput.Horizontal;
            }

            if (moveInput != 0)
            {
                CurrentMovementState = Input.GetKey(KeyCode.LeftShift) ? MovementState.Running : MovementState.Walking;
                
                if (moveInput > 0 && !movingRight) Flip();
                else if (moveInput < 0 && movingRight) Flip();
            }
            else
            {
                CurrentMovementState = MovementState.Idle;
            }

            // 2. Loncat
            if ((Input.GetButtonDown("Jump") || mobileJump) && isGrounded)
            {
                Jump();
            }

            // 3. Serang Normal (DIPERBAIKI AGAR 100% ANTI-BOCOR DI MOBILE)
            // 3. Serang Normal (DIPERBAIKI LENGKAP)
            bool isOverUI = false;

            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                // Cek apakah mouse sedang di atas UI (Untuk PC/Editor)
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    isOverUI = true;
                }
                
                // Cek apakah ada jari yang sedang menyentuh UI (Untuk HP)
                if (Input.touchCount > 0)
                {
                    foreach (Touch touch in Input.touches)
                    {
                        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        {
                            isOverUI = true;
                            break;
                        }
                    }
                }
            }

            // PENTING: Jika tombol mobile horizontal TIDAK 0 (sedang ditekan), KUNCI pcAttack agar tidak bisa menyerang!
            bool isMovingMobile = MobileInput.Horizontal != 0f;

            // Karakter hanya boleh attack lewat klik mouse JIKA tidak di atas UI DAN tidak sedang menekan tombol jalan mobile
            bool pcAttack = Input.GetMouseButtonDown(0) && !isOverUI && !isMovingMobile;

            if (mobileAttack)
            {
                ActivateAttack();
            }
        }

        // Fungsi Loncat (Jump)
        public void Jump()
        {
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, JumpForce);
            isGrounded = false;
            _animator.SetTrigger("Jump"); // Pastikan ada trigger bernama "Jump" di Animator Anda
        }

        // Deteksi Menyentuh Tanah kembali menggunakan Collision 2D
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Sesuaikan dengan nama Tag lantai game Anda (contoh: "Ground")
            if (collision.gameObject.CompareTag("Ground")) 
            {
                isGrounded = true;
            }
        }

        public void ActivateRun(){
            if(CurrentFightingState != FightingState.Idle && CurrentFightingState != FightingState.OnGuard) return;
            CurrentMovementState = MovementState.Running;
            _animator.SetBool("Busy",false);
        }

        public void ActivateIdle(){
            CurrentMovementState = MovementState.Idle;
            CurrentFightingState = FightingState.Idle;
        }

        public void ActivateWalk(){
            if(CurrentFightingState != FightingState.Idle && CurrentFightingState != FightingState.OnGuard) return;
            CurrentMovementState = MovementState.Walking;
            _animator.SetBool("Busy",false);
        }

        public void ActivateGuard(){
            if(CurrentFightingState == FightingState.OnGuard){
                CurrentMovementState = MovementState.Idle;
                CurrentFightingState = FightingState.Idle;
                _animator.SetBool("Guard",false);
            }else{
                CurrentMovementState = MovementState.Idle;
                CurrentFightingState = FightingState.OnGuard;
                _animator.SetBool("Guard",true);
            }
        }

        public void ActivateBackStep(){       
            if(CurrentFightingState != FightingState.OnGuard) return;
            float direction = movingRight ? -1f : 1f;
            _rigidBody.AddForce(Vector2.right * direction * BackStepPower, ForceMode2D.Impulse);
            _animator.SetTrigger("BackStep");
        }

        public void ActivateAttack(){
            currentSpeed = 0f; 
            CurrentMovementState = MovementState.Idle;
            CurrentFightingState = FightingState.Attacking;
            // Dorong sedikit ke depan saat menyerang
            // float direction = movingRight ? 1f : -1f;
            // _rigidBody.AddForce(Vector2.right * direction * (BackStepPower * 0.5f), ForceMode2D.Impulse);
            
            StartCoroutine(AttackRoutine());
        }

        public void ActivateBeamAttack(){
            if(CurrentFightingState == FightingState.OnGuard){
                ActivateGuard();
            }

            CurrentMovementState = MovementState.Idle;
            CurrentFightingState = FightingState.Attacking;
                
            _animator.SetBool("Busy",true);
                    
            _animator.SetTrigger("BeamAttack");
            StartCoroutine(BeamShootRoutine());
        }

        public void ActivateHurt(){
            CurrentMovementState = MovementState.Idle;
            CurrentFightingState = FightingState.Hurt;
            StartCoroutine(OnHurtRoutine());
            OnHurt.Invoke();
            PlaySound(PainSound);
        }

        public void ActivateDeath(){
            CurrentFightingState = FightingState.Death;
            _animator.enabled = false;
            deathPlace = transform.position;
            OnDeath.Invoke();
            PlaySound(DeathExplosionSound);
        }

        public void Explode()
        {
            if (ExplosionEffect != null)
            {
                Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            }

            foreach (Rigidbody2D rb in RBList)
            {
                if (rb != null)
                {
                    Vector2 direction = new Vector2(rb.transform.position.x - ExplosionRef.position.x,rb.transform.position.y-ExplosionRef.position.y).normalized;
                    rb.gravityScale = 0f;
                    rb.AddForce(direction * power, ForceMode2D.Impulse);
                }
            }
        }

        public void ResetState(){
            if(DissolveStatus > 0) return;

            _animator.enabled = true;       
            DissolveStatus = 1f;
            instanceMaterial.SetFloat("_Dissolve",DissolveStatus);
            transform.position = deathPlace;
            _rigidBody.linearVelocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;    
            
            foreach (Rigidbody2D rb in RBList)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;    
            }

            CurrentMovementState = MovementState.Idle;
            CurrentFightingState = FightingState.Idle;
        }

        public void Flip()
        {
            movingRight = !movingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        IEnumerator AttackRoutine()
        {
            _animator.SetTrigger("Attack");
            PlaySound(SwordSound);

            yield return new WaitForSeconds(0.5f); 

            CurrentFightingState = FightingState.Idle;
        }

        IEnumerator OnHurtRoutine()
        {
            _animator.SetTrigger("Hurt");

            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
            {
                yield return null;
            }

            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            _animator.SetBool("Busy",false);
            CurrentFightingState = FightingState.Idle;
            CurrentMovementState = MovementState.Idle;
        }

        IEnumerator BeamShootRoutine()
        {
            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("BeamAttack"))
            {
                yield return null;
            }

            PlaySound(PowerLoadSound);
            yield return new WaitForSeconds(2.4f);
            PlaySound(BeamSound);

            if (DarkBall != null)
            {
                GameObject Ball = Instantiate(DarkBall, BeamAttackRef.position, Quaternion.identity);
                // Sesuaikan arah bola dengan arah hadap karakter
                Vector3 ballScale = Ball.transform.localScale;
                ballScale.x = movingRight ? Mathf.Abs(ballScale.x) : -Mathf.Abs(ballScale.x);
                Ball.transform.localScale = ballScale;
            }
            _animator.SetBool("Busy",false);
            CurrentFightingState = FightingState.Idle;
        }

        public List<Rigidbody2D> GetAllRigidbodiesInChildren(Transform parent)
        {
            List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>(); 
            Rigidbody2D[] rbArray = parent.GetComponentsInChildren<Rigidbody2D>();
            foreach (Rigidbody2D rb in rbArray) { rigidbodies.Add(rb); }
            return rigidbodies; 
        }    
        
        public List<SpriteRenderer> GetAllSpriteRenderersInChildren(Transform parent)
        {
            List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(); 
            SpriteRenderer[] srArray = parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srArray) { spriteRenderers.Add(sr); }
            return spriteRenderers; 
        }

        private void PlaySound(AudioClip _clip){
            if(_clip == null || _Channel == null) return;
            _Channel.PlayOneShot(_clip);
        }

        public bool IsBuiltIn(Material material)
        {
            if (material == null || material.shader == null) return true;
            return material.shader.name != "DarkKnight/DarkKnightShader";
        }
    }
}