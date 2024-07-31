using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveImageManager : MonoBehaviour
{
    public Button saveButton; // Asocia el botón en el Inspector

    private void Start()
    {
        saveButton.onClick.AddListener(OnSaveButtonClicked);
    }

    private void OnDestroy()
    {
        // Remove listener to prevent potential memory leaks or duplicated calls.
        saveButton.onClick.RemoveListener(OnSaveButtonClicked);
    }

    private void OnSaveButtonClicked()
    {
        StartCoroutine(SaveImageCoroutine());
    }

    private IEnumerator SaveImageCoroutine()
    {
        yield return new WaitForEndOfFrame();

        Sprite selectedImage = ContentManager.selectedImage;

        if (selectedImage == null)
        {
            Debug.LogError("Selected image is null, cannot save image.");
            ShowToast("Failed to save image: no image selected.");
            yield break;
        }

        // Convertir el Sprite a una textura legible
        Texture2D texture = CreateReadableTexture(selectedImage);

        // Guardar la imagen en la galería
        string folderPath = Path.Combine(GetGalleryPath(), "Puzzle");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Generar un nombre de archivo único utilizando timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(folderPath, "savedImage_" + timestamp + ".png");
        File.WriteAllBytes(filePath, texture.EncodeToPNG());

        // Agregar la imagen a la galería
        AddImageToGallery(filePath);

        Debug.Log("Image saved at: " + filePath);
        if (File.Exists(filePath))
        {
            ShowToast("Image saved successfully!");
            Debug.Log("File saved successfully.");
        }
        else
        {
            ShowToast("Failed to save the image.");
            Debug.LogError("Failed to save the image.");
        }
    }

    private Texture2D CreateReadableTexture(Sprite sprite)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            sprite.texture.width,
            sprite.texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(sprite.texture, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTexture = new Texture2D(sprite.texture.width, sprite.texture.height);
        readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTexture;
    }

    private string GetGalleryPath()
    {
        using (AndroidJavaClass env = new AndroidJavaClass("android.os.Environment"))
        {
            return env.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", env.GetStatic<string>("DIRECTORY_PICTURES")).Call<string>("getAbsolutePath");
        }
    }

    private void AddImageToGallery(string filePath)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject mediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection");
            mediaScannerConnection.CallStatic("scanFile", activity, new string[] { filePath }, null, null);
        }
    }

    private void ShowToast(string message)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", message);
            AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, javaString, toastClass.GetStatic<int>("LENGTH_SHORT"));
            toast.Call("show");
        }
    }
}
