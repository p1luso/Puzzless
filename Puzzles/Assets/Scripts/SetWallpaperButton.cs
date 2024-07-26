using UnityEngine;

public class SetWallpaperButton : MonoBehaviour
{
    public WallpaperManager wallpaperManager;

    public void OnSetWallpaperButtonClicked()
    {
        if (wallpaperManager != null)
        {
            wallpaperManager.SetWallpaper();
        }
    }
}