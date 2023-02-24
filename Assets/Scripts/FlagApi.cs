using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlagApi : MonoBehaviour
{

    public string codeCountry; // Code de pays à charger
    public Image flagImage; // Référence à l'objet Image de l'UI

    public void UpdateFlag()
    {
        StartCoroutine(LoadFlag());
    }

    IEnumerator LoadFlag()
    {
        string url = "https://countryflagsapi.com/png/" + codeCountry;

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Mettre à jour l'image de l'UI avec la texture téléchargée
            flagImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.Log("Erreur lors du chargement de l'image : " + request.error);
        }
    }
}
