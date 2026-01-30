using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Frame-by-frame sprite animation controller.
/// Handles character animations without requiring Unity Animator.
/// </summary>
public class SpriteAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float defaultFrameRate = 12f;
    [SerializeField] private bool playOnAwake = true;

    private SpriteRenderer spriteRenderer;
    private Dictionary<string, AnimationData> animations = new Dictionary<string, AnimationData>();
    private string currentAnimation = "";
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private bool isPlaying = false;

    public string CurrentAnimation => currentAnimation;
    public bool IsPlaying => isPlaying;

    public event System.Action<string> OnAnimationComplete;
    public event System.Action<string, int> OnFrameChange;

    [System.Serializable]
    public class AnimationData
    {
        public string name;
        public Sprite[] frames;
        public float frameRate;
        public bool loop;
        public bool pingPong;
        private bool goingForward = true;

        public AnimationData(string name, Sprite[] frames, float frameRate = 12f, bool loop = true, bool pingPong = false)
        {
            this.name = name;
            this.frames = frames;
            this.frameRate = frameRate;
            this.loop = loop;
            this.pingPong = pingPong;
        }

        public int GetNextFrame(int current)
        {
            if (pingPong)
            {
                if (goingForward)
                {
                    if (current >= frames.Length - 1)
                    {
                        goingForward = false;
                        return current - 1;
                    }
                    return current + 1;
                }
                else
                {
                    if (current <= 0)
                    {
                        goingForward = true;
                        return 1;
                    }
                    return current - 1;
                }
            }
            return (current + 1) % frames.Length;
        }

        public bool IsComplete(int frame)
        {
            return !loop && !pingPong && frame >= frames.Length - 1;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (playOnAwake && animations.Count > 0)
        {
            Play(currentAnimation);
        }
    }

    private void Update()
    {
        if (!isPlaying || !animations.ContainsKey(currentAnimation)) return;

        var anim = animations[currentAnimation];
        if (anim.frames == null || anim.frames.Length == 0) return;

        frameTimer += Time.deltaTime;
        float frameInterval = 1f / anim.frameRate;

        if (frameTimer >= frameInterval)
        {
            frameTimer -= frameInterval;

            if (anim.IsComplete(currentFrame))
            {
                isPlaying = false;
                OnAnimationComplete?.Invoke(currentAnimation);
                return;
            }

            currentFrame = anim.GetNextFrame(currentFrame);
            UpdateSprite();
            OnFrameChange?.Invoke(currentAnimation, currentFrame);
        }
    }

    private void UpdateSprite()
    {
        if (!animations.ContainsKey(currentAnimation)) return;
        var anim = animations[currentAnimation];
        if (anim.frames != null && currentFrame < anim.frames.Length && anim.frames[currentFrame] != null)
        {
            spriteRenderer.sprite = anim.frames[currentFrame];
        }
    }

    public void AddAnimation(string name, Sprite[] frames, float frameRate = 12f, bool loop = true, bool pingPong = false)
    {
        animations[name] = new AnimationData(name, frames, frameRate, loop, pingPong);
        if (string.IsNullOrEmpty(currentAnimation))
        {
            currentAnimation = name;
        }
    }

    public void Play(string animationName, bool restart = false)
    {
        if (!animations.ContainsKey(animationName))
        {
            Debug.LogWarning($"[SpriteAnimator] Animation '{animationName}' not found");
            return;
        }

        if (currentAnimation == animationName && isPlaying && !restart)
        {
            return;
        }

        currentAnimation = animationName;
        currentFrame = 0;
        frameTimer = 0f;
        isPlaying = true;
        UpdateSprite();
    }

    public void Stop()
    {
        isPlaying = false;
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Resume()
    {
        isPlaying = true;
    }

    public void SetFrame(int frame)
    {
        if (!animations.ContainsKey(currentAnimation)) return;
        var anim = animations[currentAnimation];
        currentFrame = Mathf.Clamp(frame, 0, anim.frames.Length - 1);
        UpdateSprite();
    }

    public bool HasAnimation(string name)
    {
        return animations.ContainsKey(name);
    }

    public void ClearAnimations()
    {
        animations.Clear();
        currentAnimation = "";
        isPlaying = false;
    }

    /// <summary>
    /// Quick setup for character with standard animation set
    /// </summary>
    public void SetupCharacterAnimations(Sprite[] idle, Sprite[] walk, Sprite[] attack, Sprite[] dodge, Sprite[] hurt)
    {
        if (idle != null && idle.Length > 0)
            AddAnimation("idle", idle, 8f, true, true);
        if (walk != null && walk.Length > 0)
            AddAnimation("walk", walk, 12f, true, false);
        if (attack != null && attack.Length > 0)
            AddAnimation("attack", attack, 16f, false, false);
        if (dodge != null && dodge.Length > 0)
            AddAnimation("dodge", dodge, 16f, false, false);
        if (hurt != null && hurt.Length > 0)
            AddAnimation("hurt", hurt, 12f, false, false);

        if (idle != null && idle.Length > 0)
            Play("idle");
    }

    /// <summary>
    /// Quick setup for boss with standard animation set
    /// </summary>
    public void SetupBossAnimations(Sprite[] idle, Sprite[] attack, Sprite[] hurt, Sprite[] transition)
    {
        if (idle != null && idle.Length > 0)
            AddAnimation("idle", idle, 6f, true, true);
        if (attack != null && attack.Length > 0)
            AddAnimation("attack", attack, 12f, false, false);
        if (hurt != null && hurt.Length > 0)
            AddAnimation("hurt", hurt, 10f, false, false);
        if (transition != null && transition.Length > 0)
            AddAnimation("transition", transition, 8f, false, false);

        if (idle != null && idle.Length > 0)
            Play("idle");
    }
}
