using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    [Header("Web Image Address")]
    [SerializeField] private string imageURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Cat_August_2010-4.jpg/2560px-Cat_August_2010-4.jpg";

    [Header("Billboard Raw Image")]
    [SerializeField] private RawImage billboardImage;

    private void Start() {
        BillboardManager.instance.AddToDownloadQueue(imageURL, SetupBillboard);
    }

    // Apply the given texture to our billboard image.
    private void SetupBillboard(Texture2D tex)
    {
        billboardImage.texture = tex;
    }
}
