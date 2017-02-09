using System;
using UnityEngine;

namespace Assets.Map
{
    public class TextureSet : ICloneable
    {
        public Texture2D AlbedoMap { get; set; }
        public Texture2D HeightMap { get; set; }
        public Texture2D NormalMap { get; set; }
        public Texture2D AmbOccMap { get; set; }
        public Texture2D GlossyMap { get; set; }
        public Texture2D MetallMap { get; set; }


        public object Clone()
        {
            return new TextureSet
            {
                AlbedoMap = AlbedoMap,
                AmbOccMap = AmbOccMap,
                GlossyMap = GlossyMap,
                HeightMap = HeightMap,
                MetallMap = MetallMap,
                NormalMap = NormalMap
            };
        }
    }
}