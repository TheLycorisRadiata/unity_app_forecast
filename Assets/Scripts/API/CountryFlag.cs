using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CountryFlag : MonoBehaviour
{
    [SerializeField] private Image flagImage;
    [SerializeField] private LocationScriptableObject location;

    public void UpdateFlag()
    {
        StartCoroutine(LoadFlag());
    }

    IEnumerator LoadFlag()
    {
        string uri = "https://countryflagsapi.com/png/" + location.countryCode;
        Texture2D texture;

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Country flag API failed to fetch image of country code \"{location.countryCode}\".\n" + webRequest.error);
                flagImage.sprite = null;
                yield break;
            }

            texture = DownloadHandlerTexture.GetContent(webRequest);
            flagImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
