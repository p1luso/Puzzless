using UnityEngine;

public class PermissionManager : MonoBehaviour
{
    void Start()
    {
        RequestPermissions();
    }

    private void RequestPermissions()
    {
        // Comprobar y solicitar permiso para escribir en el almacenamiento externo
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }

        // Comprobar y solicitar permiso para leer del almacenamiento externo
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageRead))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
        }

        // Comprobar y solicitar permiso para establecer el fondo de pantalla
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.SET_WALLPAPER"))
        {
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.SET_WALLPAPER");
        }
    }
}