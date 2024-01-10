using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SteveUI : MonoBehaviour
{
    public UnityEngine.UI.Image image;
    public TMP_InputField usernameInput;

    private string username;

    // Define custom names and corresponding textures
    public string[] customNames;
    public Sprite[] customTextures;

    private void Awake()
    {
        usernameInput.text = PlayerPrefs.GetString("mc-name");
        UpdateHelmIcon();
    }

    // Call this function when the OK button is pressed in the UI.
    public void OnOKButtonPressed()
    {
        // Save the new username in PlayerPrefs.
        PlayerPrefs.SetString("mc-name", usernameInput.text);

        Debug.Log(usernameInput.text);

        // Update the helm icon.
        UpdateHelmIcon();
    }

    private void UpdateHelmIcon()
    {
        Debug.Log("UpdateHelmIcon called.");
        username = PlayerPrefs.GetString("mc-name");
        Debug.Log("Username: " + username);
        StartCoroutine(ApplyHelm());
    }


    IEnumerator ApplyHelm()
    {
        // Check if the username matches any custom name
        int customIndex = -1;
        for (int i = 0; i < customNames.Length; i++)
        {
            if (username.Equals(customNames[i].ToLower()))
            {
                customIndex = i;
                break;
            }
        }
        
        if (customIndex != -1)
        {
            image.sprite = customTextures[customIndex];
        }
        else
        {
            Debug.Log("ApplyHelm coroutine started.");
            Debug.Log("Username: " + username); // Verify the value of the username.
            UnityWebRequest helm = UnityWebRequestTexture.GetTexture("https://minotar.net/helm/" + username);
            yield return helm.SendWebRequest();
            Debug.Log("just returned send web request");

            if (helm.isNetworkError || helm.isHttpError)
            {
                Debug.Log("Error downloading skin!");
                // Set the helm icon to a default image or show an error message.
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)helm.downloadHandler).texture;
                image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                Debug.Log("Helm icon applied successfully.");
            }
        }
    }
}
