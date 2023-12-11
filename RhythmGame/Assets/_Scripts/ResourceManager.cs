using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceManager : MonoBehaviour
{
    public event Action AllResourcesLoaded;

    [SerializeField]
    private string _resourcePath;

    [SerializeField]
    private string _url;

    [field: SerializeField]
    public AudioClip HitSound { get; private set; } = null;

    private async void Awake()
    {
        _resourcePath = Application.persistentDataPath;
        _url = "file:///" + _resourcePath;
        await LoadResources();
    }

    private async Task LoadResources()
    {
        // check if the directory exists
        if (!System.IO.Directory.Exists(_resourcePath))
        {
            // if not, create it
            System.IO.Directory.CreateDirectory(_resourcePath);
        }
        // load resources using UnityWebRequest
        HitSound = await LoadAudioResource("Audio/hitsound", AudioType.WAV);

        AllResourcesLoaded?.Invoke();
    }

    // Loads a local audio file using UnityWebRequest from the persistent data path
    private async Task<AudioClip> LoadAudioResource(string filename, AudioType extension)
    {
        string path = _url + "/" + filename + "." + extension;
        Debug.Log("Loading audio file from " + path);
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, extension);

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
            return null;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
        return clip;
    }
}