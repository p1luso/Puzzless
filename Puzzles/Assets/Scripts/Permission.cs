using UnityEngine;

public class PermissionManager : MonoBehaviour
{
    void Start()
    {
        // Solicitar permisos de almacenamiento externo
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageRead))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
        }

        // Solicitar permiso para gestionar el almacenamiento externo (Android 11+)
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.MANAGE_EXTERNAL_STORAGE"))
        {
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.MANAGE_EXTERNAL_STORAGE");
        }

        // Solicitar permiso para establecer el wallpaper
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.SET_WALLPAPER"))
        {
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.SET_WALLPAPER");
        }
    }
}