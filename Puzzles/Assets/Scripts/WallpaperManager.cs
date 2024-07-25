using System.Collections;
using UnityEngine;
using System.IO;

public class WallpaperManager : MonoBehaviour
{
    public void SetWallpaper(Sprite sprite)
    {
        StartCoroutine(SetWallpaperCoroutine(sprite));
    }

    private IEnumerator SetWallpaperCoroutine(Sprite sprite)
    {
        yield return new WaitForEndOfFrame();

        // Convertir el Sprite a Texture2D
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
        texture.SetPixels(sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height));
        texture.Apply();

        // Guardar la imagen en el almacenamiento externo
        string path = Path.Combine(Application.persistentDataPath, "wallpaper.png");
        File.WriteAllBytes(path, texture.EncodeToPNG());

        // Establecer la imagen como fondo de pantalla
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass wallpaperManager = new AndroidJavaClass("android.app.WallpaperManager");
        AndroidJavaObject wm = wallpaperManager.CallStatic<AndroidJavaObject>("getInstance", currentActivity);

        AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");
        AndroidJavaObject fileObject = fileClass.CallStatic<AndroidJavaObject>("createTempFile", "wallpaper", ".png");
        fileObject.Call("delete");
        AndroidJavaObject fos = new AndroidJavaObject("java.io.FileOutputStream", fileObject);
        fos.Call("write", texture.EncodeToPNG());
        fos.Call("flush");
        fos.Call("close");

        wm.Call("setStream", new AndroidJavaObject("java.io.FileInputStream", fileObject));

        Debug.Log("Wallpaper set successfully!");
    }
}