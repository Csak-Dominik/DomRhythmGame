using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip _hitsound;

    [SerializeField]
    private ResourceManager _resourceManager;

    private void Awake()
    {
        _resourceManager.AllResourcesLoaded += OnResourcesLoaded;
    }

    private void OnResourcesLoaded()
    {
        _hitsound = _resourceManager.HitSound;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }
}