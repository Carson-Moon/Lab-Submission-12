using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BillboardManager : MonoBehaviour
{
    // Singleton
    public static BillboardManager instance {get; private set;}

    // Dictionary to store our downloaded images.
    [SerializeField] Dictionary<string, Texture2D> downloadedImages = new Dictionary<string, Texture2D>();

    // Queue for our downloads.
    [SerializeField] Queue<DownloadQueue> downloadQueue = new Queue<DownloadQueue>();
    private bool isDequeuing = false;


    private void Awake() {
        // Singleton setup.
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    
    private void Update() {
        // If we are not dequeueing and have something in queue, start dequeueing!
        if(!isDequeuing && downloadQueue.Count > 0)
        {
            isDequeuing = true;

            // Grab first from queue.
            DownloadQueue thisQueue = downloadQueue.Dequeue();
            StartCoroutine(DownloadImage(thisQueue.callback, thisQueue.url));
        }
    }

    // Add this to our download queue.
    public void AddToDownloadQueue(string url, Action<Texture2D> callback)
    {
        DownloadQueue newQueue = new DownloadQueue(url, callback);
        downloadQueue.Enqueue(newQueue);
    }

    // Return a texture to callback. Download if not already downloaded.
    public IEnumerator DownloadImage(Action<Texture2D> callback, string imageURL)
    {
        // If this image already exists in our dictionary, do not redownload!
        if(!downloadedImages.ContainsKey(imageURL))
        {
            print("Downloading new image!");

            // Download our iamge.
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);
            yield return request.SendWebRequest();

            downloadedImages.Add(imageURL, DownloadHandlerTexture.GetContent(request));
        }
        else
        {
            print("Image already downloaded!");
        }

        // Return our texture.
        callback(downloadedImages[imageURL]);

        // Go next if there is something else in queue, otherwise stop dequeueing.
        if(downloadQueue.Count > 0)
        {
            // Grab first from queue.
            DownloadQueue thisQueue = downloadQueue.Dequeue();
            StartCoroutine(DownloadImage(thisQueue.callback, thisQueue.url));
        }
        else
        {
            print(downloadQueue.Count);
            isDequeuing = false;
        }
    }
}

public class DownloadQueue
{
    public string url;
    public Action<Texture2D> callback;

    public DownloadQueue(string url, Action<Texture2D> callback)
    {
        this.url = url;
        this.callback = callback;
    }
}
