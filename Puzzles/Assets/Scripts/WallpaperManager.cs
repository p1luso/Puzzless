using System.Collections;
using System.IO;
using UnityEngine;

public class WallpaperManager : MonoBehaviour
{
    public void SetWallpaper(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("Selected image is null, cannot set wallpaper.");
            return;
        }

        StartCoroutine(SetWallpaperCoroutine(sprite));
    }

    private IEnumerator SetWallpaperCoroutine(Sprite sprite)
    {
        yield return new WaitForEndOfFrame();

        // Convertir el Sprite a una textura legible
        Texture2D texture = CreateReadableTexture(sprite);

        // Guardar la imagen en el almacenamiento externo
        string path = Path.Combine(Application.persistentDataPath, "wallpaper.png");
        File.WriteAllBytes(path, texture.EncodeToPNG());

        Debug.Log("Image saved at: " + path);
        if (File.Exists(path))
        {
            Debug.Log("File exists and ready to be used as wallpaper.");
            // Establecer la imagen como fondo de pantalla
            SetWallpaperWithIntent(path);
        }
        else
        {
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

    private void SetWallpaperWithIntent(string imagePath)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
            {
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", intentClass.GetStatic<string>("ACTION_ATTACH_DATA"));

                using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
                {
                    AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
                    intent.Call<AndroidJavaObject>("setDataAndType", uri, "image/png");
                    intent.Call<AndroidJavaObject>("putExtra", "mimeType", "image/png");

                    currentActivity.Call("startActivity", intent);
                }
            }
        }
    }
}
