using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CountryFlag : MonoBehaviour
{
    [SerializeField] private Image flagImage;

    public void UpdateFlag(string countryCode)
    {
        StartCoroutine(LoadFlag(countryCode));
    }

    IEnumerator LoadFlag(string countryCode)
    {
        string uri = $"https://flagcdn.com/w320/{countryCode}.png";
        Texture2D texture;

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Country flag API failed to fetch image of country code \"{countryCode}\".\n" + webRequest.error);
                ResetFlag();
                yield break;
            }

            texture = DownloadHandlerTexture.GetContent(webRequest);
            flagImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    public void ResetFlag()
    {
        flagImage.sprite = null;
    }
}
