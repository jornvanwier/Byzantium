using System.Collections;

namespace Assets.Map
{
    public class TileData
    {
        private readonly BitArray _data = new BitArray(32);

        public TileData(int bufferSourceBytes)
        {
            _data = new BitArray(new[] {bufferSourceBytes});
        }

        public TileData(TileType type, bool selected)
        {
            SetTileType(type);
            SetSelected(selected);
        }

        public void SetTileType(TileType type)
        {
            byte[] b = {(byte) type};
            var t = new BitArray(b);

            for (var i = 0; i < t.Length; ++i)
                _data.Set(i, t[i]);
        }

        public void SetSelected(bool value)
        {
            _data[8] = value;
        }

        public int GetAsInt()
        {
            var array = new int[1];
            _data.CopyTo(array, 0);
            return array[0];
        }

        public override string ToString()
        {
            return _data.ToString();
        }
    }
}