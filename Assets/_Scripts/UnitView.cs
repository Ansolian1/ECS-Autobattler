using UnityEngine;

public class UnitView : MonoBehaviour
{
    [Header("—сылки")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform visualPivot;
    [SerializeField] private Transform hpBarFill;
    [SerializeField] private AudioSource audioSource;
    private Vector3 _targetScale;
    private void Awake()
    {
        if (visualPivot != null)
            _targetScale = visualPivot.localScale;
        else
            _targetScale = new Vector3(0.2f, 0.2f, 0.2f);
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetFacing(bool faceLeft)
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = faceLeft;
    }

    public void UpdateHealth(float current, float max)
    {
        if (hpBarFill != null)
        {
            float fill = Mathf.Clamp01(current / max);
            hpBarFill.localScale = new Vector3(fill, 1f, 1f);
        }
    }

    public void PlayAttack(AudioClip clip)
    {
        if (visualPivot != null)
        {
            visualPivot.localScale = _targetScale * 1.2f;
        }

        if (clip != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }

    public void ResetVisualScale()
    {
        if (visualPivot != null && visualPivot.localScale != Vector3.one)
        {
            visualPivot.localScale = Vector3.Lerp(visualPivot.localScale, _targetScale, Time.deltaTime * 10f);
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}