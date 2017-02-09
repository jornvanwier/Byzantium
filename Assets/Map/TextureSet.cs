using UnityEngine;

namespace Assets.Map
{
    public class TextureSet
    {
        public Texture2D AlbedoMap { get; set; }
        public Texture2D HeightMap { get; set; }
        public Texture2D NormalMap { get; set; }
        public Texture2D AmbOccMap { get; set; }
        public Texture2D GlossyMap { get; set; }
        public Texture2D MetallMap { get; set; }
    }
}