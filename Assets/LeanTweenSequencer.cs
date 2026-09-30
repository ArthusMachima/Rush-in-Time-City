using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeanTweenSequencer : MonoBehaviour
{
    [SerializeField] LeanTweenKeyframe[] keyframes;
    [SerializeField] bool playOnStart;
    [SerializeField] bool saveDefaultValue;
    [SerializeField] bool deleteOnAnimationEnd;

    [Header("Debug")]
    [SerializeField] bool previewAnimation;

    [Header("Initial Values")]
    [SerializeField] Vector3 ogPos;
    [SerializeField] Quaternion ogRot;
    [SerializeField] Vector3 ogScale;
    [SerializeField] Vector4 ogColor;

    private Coroutine _sequenceCoroutine;

    private void Update()
    {
        if (previewAnimation)
        {
            PlayKeyframes();
            previewAnimation = false;
        }
    }

    public void Start()
    {
        if (saveDefaultValue)
        {
            ogPos = transform.position;
            ogRot = transform.rotation;
            ogScale = transform.localScale;
            ogColor = GetColor(gameObject);
        }

        if (playOnStart) PlayKeyframes();
    }

    public void PlayKeyframes()
    {
        if (_sequenceCoroutine != null) StopCoroutine(_sequenceCoroutine);
        LeanTween.cancel(gameObject);

        if (saveDefaultValue)
        {
            transform.SetPositionAndRotation(ogPos, ogRot);
            transform.localScale = ogScale;
            SetColor(gameObject, ogColor);
        }

        _sequenceCoroutine = StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        var groups = new List<List<LeanTweenKeyframe>>();
        var currentGroup = new List<LeanTweenKeyframe>();

        foreach (var key in keyframes)
        {
            currentGroup.Add(key);
            if (!key.isLayer)
            {
                groups.Add(currentGroup);
                currentGroup = new List<LeanTweenKeyframe>();
            }
        }

        // Leftover layers with no non-layer anchor — play them as a final group
        if (currentGroup.Count > 0)
            groups.Add(currentGroup);

        foreach (var group in groups)
        {
            bool groupDone = false;

            // The last keyframe in the group is the "anchor" (isLayer = false)
            // All others are layers (run concurrently)
            for (int i = 0; i < group.Count; i++)
            {
                LeanTweenKeyframe key = group[i];
                bool isAnchor = i == group.Count - 1;

                PlayKeyframe(key, isAnchor ? () => groupDone = true : null);
            }

            yield return new WaitUntil(() => groupDone);
        }

        _sequenceCoroutine = null;
        if (deleteOnAnimationEnd)
        {
            Destroy(gameObject);
            if (gameObject.transform.parent.TryGetComponent<LeanTweenSequenceGroup>(out LeanTweenSequenceGroup group))
            {
                if (group == null) yield break;
                Destroy(group.gameObject);
            }
        }
    }

    private void PlayKeyframe(LeanTweenKeyframe key, Action onComplete)
    {
        switch (key.transType)
        {
            case TransformType.Position:
                gameObject.LeanMove((Vector3)key.vectors, key.dur)
                    .setDelay(key.delay)
                    .setEase(key.easeType)
                    .setOnComplete(onComplete);
                break;

            case TransformType.Rotation:
                gameObject.LeanRotate((Vector3)key.vectors, key.dur)
                    .setDelay(key.delay)
                    .setEase(key.easeType)
                    .setOnComplete(onComplete);
                break;

            case TransformType.Scale:
                gameObject.LeanScale((Vector3)key.vectors, key.dur)
                    .setDelay(key.delay)
                    .setEase(key.easeType)
                    .setOnComplete(onComplete);
                break;

            case TransformType.Color:
                gameObject.LeanColor(key.vectors, key.dur)
                    .setDelay(key.delay)
                    .setEase(key.easeType)
                    .setOnComplete(onComplete);
                break;
        }
    }

    public static Color GetColor(GameObject obj)
    {
        if (obj.TryGetComponent<Graphic>(out var graphic)) return graphic.color;
        if (obj.TryGetComponent<SpriteRenderer>(out var sr)) return sr.color;
        if (obj.TryGetComponent<MeshRenderer>(out var mr)) return mr.material.color;

        Debug.LogWarning($"[ColorUtility] No colorable component found on {obj.name}");
        return Color.white;
    }

    public static void SetColor(GameObject obj, Color color)
    {
        if (obj.TryGetComponent<Graphic>(out var graphic)) { graphic.color = color; return; }
        if (obj.TryGetComponent<SpriteRenderer>(out var sr)) { sr.color = color; return; }
        if (obj.TryGetComponent<MeshRenderer>(out var mr)) { mr.material.color = color; return; }

        Debug.LogWarning($"[ColorUtility] No colorable component found on {obj.name}");
    }
}

[Serializable]
public class LeanTweenKeyframe
{
    public TransformType transType;
    public LeanTweenType easeType;
    public float dur;
    public float delay;
    public Vector4 vectors;
    public bool isLayer;
}

public enum TransformType
{
    Position,
    Rotation,
    Scale,
    Color
}