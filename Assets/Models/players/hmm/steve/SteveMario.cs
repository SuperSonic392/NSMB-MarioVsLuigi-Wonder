using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;

public class SteveMario : MonoBehaviourPun, IPunObservable
{
    public Mesh neo, old, slim;
    public SkinnedMeshRenderer mesh;
    private Texture2D skin;
    private string minecraftName;

    public PlayerController playerController;
    public Color iceFlowerTint = Color.blue;
    public Color fireFlowerTint = Color.red;


    // Define custom names and corresponding textures
    public string[] customNames;
    public Texture2D[] customTextures;

    private bool shouldFlipUpsideDown = false;
    public GameObject models;
    [Tooltip("The height added to the model's position when flipped")]
    public float flipHeight; 

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Get the Minecraft name from PlayerPrefs or set a default name if it's not set
            minecraftName = PlayerPrefs.GetString("mc-name");
            if (string.IsNullOrEmpty(minecraftName))
            {
                minecraftName = "MHF_Steve"; // Set a default name if PlayerPrefs is empty
                PlayerPrefs.SetString("mc-name", minecraftName);
            }

            // Sync the Minecraft name across the network using custom properties
            photonView.Owner.CustomProperties["mc-name"] = minecraftName;
            photonView.Owner.SetCustomProperties(photonView.Owner.CustomProperties);

            StartCoroutine(GetTexture());
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(minecraftName);
        }
        else
        {
            minecraftName = (string)stream.ReceiveNext();
            StartCoroutine(GetTexture());
        }
    }

    IEnumerator GetTexture()
    {
        if (mesh != null)
        {

            // Check if the username matches any custom name
            int customIndex = -1;
            for (int i = 0; i < customNames.Length; i++)
            {
                if (minecraftName.ToLower().Equals(customNames[i].ToLower()))
                {
                    customIndex = i;
                    break;
                }
            }

            if (customIndex != -1)
            {
                // Use the custom texture instead of fetching from the internet
                skin = customTextures[customIndex];
            }
            else
            {
                // Fetch the skin from the internet
                UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://minotar.net/skin/" + minecraftName);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log("Minecraft name invalid or not found!");
                }
                else
                {
                    skin = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    skin.filterMode = FilterMode.Point; // We don't like blurry skins.
                }
            }

            mesh.materials[0].mainTexture = skin;

            Mesh desiredMesh = null;
            if (skin.height == 64)
            {
                if (skin.GetPixel(50, 44).a == 0)
                {
                    desiredMesh = slim; // Set the mesh to the slim layout.
                }
                else
                {
                    desiredMesh = neo; // Set the mesh to the new 64 x 64 layout.
                }
            }
            else
            {
                desiredMesh = old; // Set the skin to the old 64 x 32 layout.
            }

            // Set the desired mesh only once after the height check.
            if (desiredMesh != null)
            {
                mesh.sharedMesh = desiredMesh;
            }
        }
    }
    public string oldName;
    private void Update()
    {
        if (minecraftName.Equals("Dinnerbone") || minecraftName.Equals("Grumm"))
        {
            shouldFlipUpsideDown = true;
        }

        if (shouldFlipUpsideDown)
        {
            mesh.transform.parent.localScale = new Vector3(mesh.transform.parent.localScale.x, -Mathf.Abs(mesh.transform.parent.localScale.y), mesh.transform.parent.localScale.z);
            mesh.transform.parent.localPosition = Vector3.up * flipHeight;
        }
        if (oldName != minecraftName)
        {
            StartCoroutine(GetTexture());
            oldName = minecraftName;
        }
        if (playerController != null)
        {
            if (playerController.state == Enums.PowerupState.IceFlower)
            {
                mesh.materials[0].color = iceFlowerTint;
            }
            else if (playerController.state == Enums.PowerupState.FireFlower)
            {
                mesh.materials[0].color = fireFlowerTint;
            }
            else
            {
                // Reset the tint to white if no specific powerup state applies
                mesh.materials[0].color = Color.white;
            }
        }
    }
}
