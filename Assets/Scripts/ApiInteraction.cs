using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiInteraction : MonoBehaviour
{
    private string jsonText = null;

    public string FetchData(string uri)
    {
        StartCoroutine(GetRequest(uri));
        return jsonText;
    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
                jsonText = webRequest.downloadHandler.text;
            else
                jsonText = null;
        }
    }
}
