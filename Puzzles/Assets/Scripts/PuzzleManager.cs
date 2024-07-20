using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public RawImage rawImage;
    public GameObject piecePrefab;
    public Transform puzzleParent;

    public void DivideImage(int rows, int cols)
    {
        Texture2D texture = rawImage.texture as Texture2D;
        int pieceWidth = texture.width / cols;
        int pieceHeight = texture.height / rows;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Rect pieceRect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
                Sprite pieceSprite = Sprite.Create(texture, pieceRect, new Vector2(0.5f, 0.5f));
                GameObject piece = Instantiate(piecePrefab, puzzleParent);
                piece.GetComponent<Image>().sprite = pieceSprite;
            }
        }
    }
}