using System.Linq;
using UnityEngine;

namespace Assets.Globals
{
    public class SpriteFilter
    {
        // Returns only sprites that have at least one non-transparent pixel
        public Sprite[] RemoveEmpty(Texture2D sheet, Sprite[] sprites, float alphaThreshold = 0.01f)
        {
            // Copy the texture once into a readable texture (works without Read/Write)
            var rt = RenderTexture.GetTemporary(sheet.width, sheet.height, 0);
            var prev = RenderTexture.active;
            Graphics.Blit(sheet, rt);
            RenderTexture.active = rt;

            var readable = new Texture2D(sheet.width, sheet.height, TextureFormat.RGBA32, false);
            readable.ReadPixels(new Rect(0, 0, sheet.width, sheet.height), 0, 0);
            readable.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            var result = sprites.Where(s => !IsEmpty(readable, s, alphaThreshold)).ToArray();

            Object.DestroyImmediate(readable);
            return result;
        }

        static bool IsEmpty(Texture2D tex, Sprite s, float alphaThreshold)
        {
            Rect r = s.textureRect;
            Color[] px = tex.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
            for (int i = 0; i < px.Length; i++)
                if (px[i].a > alphaThreshold) return false;
            return true;
        }
    }
}