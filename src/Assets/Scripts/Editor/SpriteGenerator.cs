#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteGenerator
{
    [MenuItem("Game Jam/Generate Sprites")]
    public static void Generate()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Sprites")) AssetDatabase.CreateFolder("Assets", "Sprites");
        if (!AssetDatabase.IsValidFolder("Assets/Sprites/Heroes")) AssetDatabase.CreateFolder("Assets/Sprites", "Heroes");
        if (!AssetDatabase.IsValidFolder("Assets/Sprites/Bosses")) AssetDatabase.CreateFolder("Assets/Sprites", "Bosses");
        MakeHero("BronzeWarrior", new Color(0.8f, 0.5f, 0.2f));
        MakeHero("ShadowDancer", new Color(0.3f, 0.2f, 0.5f));
        MakeHero("FlameBearer", new Color(0.9f, 0.3f, 0.1f));
        MakeBoss("BronzeMask", new Color(0.7f, 0.45f, 0.15f));
        MakeBoss("ChaosTotem", new Color(0.5f, 0.15f, 0.3f));
        AssetDatabase.Refresh();
        Debug.Log("Sprites created!");
    }

    static void MakeHero(string n, Color c)
    {
        var t = new Texture2D(64, 64);
        Clear(t); Circle(t, 32, 52, 8, c); Rect(t, 24, 20, 40, 44, c);
        Rect(t, 16, 28, 24, 42, c); Rect(t, 40, 28, 48, 42, c);
        Rect(t, 26, 4, 31, 20, c); Rect(t, 33, 4, 38, 20, c);
        t.SetPixel(29, 54, Color.white); t.SetPixel(35, 54, Color.white);
        t.Apply(); Save(t, "Assets/Sprites/Heroes/" + n + ".png");
    }

    static void MakeBoss(string n, Color c)
    {
        var t = new Texture2D(128, 128);
        Clear(t); Oval(t, 64, 70, 40, 50, c);
        var e = new Color(1f, 0.9f, 0.4f);
        Circle(t, 48, 80, 10, e); Circle(t, 80, 80, 10, e);
        Circle(t, 48, 80, 4, Color.black); Circle(t, 80, 80, 4, Color.black);
        t.Apply(); Save(t, "Assets/Sprites/Bosses/" + n + ".png");
    }

    static void Clear(Texture2D t) { for (int x = 0; x < t.width; x++) for (int y = 0; y < t.height; y++) t.SetPixel(x, y, Color.clear); }
    static void Circle(Texture2D t, int cx, int cy, int r, Color c) { for (int x = cx-r; x <= cx+r; x++) for (int y = cy-r; y <= cy+r; y++) if (In(t,x,y) && Dist(x,y,cx,cy) <= r*r) t.SetPixel(x, y, c); }
    static void Oval(Texture2D t, int cx, int cy, int rx, int ry, Color c) { for (int x = cx-rx; x <= cx+rx; x++) for (int y = cy-ry; y <= cy+ry; y++) { float dx = (float)(x-cx)/rx, dy = (float)(y-cy)/ry; if (In(t,x,y) && dx*dx + dy*dy <= 1f) t.SetPixel(x, y, c); } }
    static void Rect(Texture2D t, int x1, int y1, int x2, int y2, Color c) { for (int x = x1; x <= x2; x++) for (int y = y1; y <= y2; y++) if (In(t,x,y)) t.SetPixel(x, y, c); }
    static bool In(Texture2D t, int x, int y) => x >= 0 && x < t.width && y >= 0 && y < t.height;
    static int Dist(int x, int y, int cx, int cy) => (x-cx)*(x-cx)+(y-cy)*(y-cy);

    static void Save(Texture2D t, string p)
    {
        File.WriteAllBytes(p, t.EncodeToPNG());
        Object.DestroyImmediate(t);
        AssetDatabase.ImportAsset(p);
        var i = AssetImporter.GetAtPath(p) as TextureImporter;
        if (i != null) { i.textureType = TextureImporterType.Sprite; i.spritePixelsPerUnit = 64; i.filterMode = FilterMode.Point; i.SaveAndReimport(); }
    }
}
#endif
