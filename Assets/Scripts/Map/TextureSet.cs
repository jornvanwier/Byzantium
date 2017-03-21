using System;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class TextureSet : ICloneable
    {
        private static TextureSet _default;

        public Texture2D AlbedoMap { get; set; } = _default?.AlbedoMap;
        public Texture2D NormalMap { get; set; } = _default?.NormalMap;
        public Texture2D AmbOccMap { get; set; } = _default?.AmbOccMap;
        public Texture2D GlossyMap { get; set; } = _default?.GlossyMap;
        public Texture2D MetallMap { get; set; } = _default?.MetallMap;


        public object Clone()
        {
            return new TextureSet
            {
                AlbedoMap = AlbedoMap,
                AmbOccMap = AmbOccMap,
                GlossyMap = GlossyMap,
                MetallMap = MetallMap,
                NormalMap = NormalMap
            };
        }

        public static void SetDefaultTextures(TextureSet set)
        {
            _default = set;
        }
    }
}