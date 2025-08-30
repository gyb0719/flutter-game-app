using UnityEngine;

public class SpriteGenerator : MonoBehaviour
{
    public static Sprite CreateFarmItemSprite(int itemType)
    {
        Texture2D texture = new Texture2D(64, 64);
        
        switch (itemType)
        {
            case 0: // 씨앗
                CreateSeedSprite(texture);
                break;
            case 1: // 새싹
                CreateSproutSprite(texture);
                break;
            case 2: // 작은 나무
                CreateSmallTreeSprite(texture);
                break;
            case 3: // 큰 나무
                CreateBigTreeSprite(texture);
                break;
            case 4: // 과일나무
                CreateFruitTreeSprite(texture);
                break;
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
    
    static void CreateSeedSprite(Texture2D texture)
    {
        // 배경을 투명하게
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }
        
        // 작은 갈색 원 (씨앗)
        for (int x = 28; x < 36; x++)
        {
            for (int y = 28; y < 36; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (distance < 4)
                {
                    texture.SetPixel(x, y, new Color(0.4f, 0.2f, 0.1f, 1f)); // 갈색
                }
            }
        }
    }
    
    static void CreateSproutSprite(Texture2D texture)
    {
        // 배경 투명하게
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }
        
        // 작은 녹색 줄기
        for (int x = 30; x < 34; x++)
        {
            for (int y = 20; y < 35; y++)
            {
                texture.SetPixel(x, y, new Color(0.2f, 0.6f, 0.2f, 1f)); // 녹색
            }
        }
        
        // 작은 잎
        for (int x = 26; x < 38; x++)
        {
            for (int y = 35; y < 42; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), new Vector2(32, 38)) < 6)
                {
                    texture.SetPixel(x, y, new Color(0.3f, 0.7f, 0.3f, 1f)); // 밝은 녹색
                }
            }
        }
    }
    
    static void CreateSmallTreeSprite(Texture2D texture)
    {
        // 배경 투명하게
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }
        
        // 갈색 줄기
        for (int x = 29; x < 35; x++)
        {
            for (int y = 15; y < 40; y++)
            {
                texture.SetPixel(x, y, new Color(0.4f, 0.2f, 0.1f, 1f)); // 갈색
            }
        }
        
        // 녹색 잎사귀
        for (int x = 20; x < 44; x++)
        {
            for (int y = 35; y < 52; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 43));
                if (distance < 12)
                {
                    texture.SetPixel(x, y, new Color(0.2f, 0.6f, 0.2f, 1f)); // 녹색
                }
            }
        }
    }
    
    static void CreateBigTreeSprite(Texture2D texture)
    {
        // 배경 투명하게
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }
        
        // 두꺼운 갈색 줄기
        for (int x = 27; x < 37; x++)
        {
            for (int y = 10; y < 40; y++)
            {
                texture.SetPixel(x, y, new Color(0.3f, 0.15f, 0.05f, 1f)); // 진한 갈색
            }
        }
        
        // 큰 녹색 잎사귀
        for (int x = 15; x < 49; x++)
        {
            for (int y = 30; y < 58; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 44));
                if (distance < 17)
                {
                    texture.SetPixel(x, y, new Color(0.1f, 0.5f, 0.1f, 1f)); // 진한 녹색
                }
            }
        }
    }
    
    static void CreateFruitTreeSprite(Texture2D texture)
    {
        // 큰 나무와 같은 기본 형태
        CreateBigTreeSprite(texture);
        
        // 빨간 과일 추가
        int[] fruitX = {20, 25, 40, 45, 35};
        int[] fruitY = {50, 45, 48, 42, 52};
        
        for (int i = 0; i < fruitX.Length; i++)
        {
            for (int x = fruitX[i] - 2; x <= fruitX[i] + 2; x++)
            {
                for (int y = fruitY[i] - 2; y <= fruitY[i] + 2; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(fruitX[i], fruitY[i]));
                    if (distance < 2.5f)
                    {
                        texture.SetPixel(x, y, new Color(0.8f, 0.1f, 0.1f, 1f)); // 빨간색
                    }
                }
            }
        }
    }
}