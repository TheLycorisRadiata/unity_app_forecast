using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlagApi : MonoBehaviour
{
    public string countryCode;
    public Image goFlagImage;

    public void UpdateFlag()
    {
        StartCoroutine(LoadFlag());
    }

    IEnumerator LoadFlag()
    {
        string url = "https://countryflagsapi.com/png/" + countryCode;

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            goFlagImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.Log("Flag image loading error: " + request.error);
        }
    }
}
