using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShipAnalyze
{
    /// <summary>
    /// マップの作戦難易度の列挙体
    /// </summary>
    public enum MapDifficulty
    {
        /// <summary>
        /// 難易度なし
        /// </summary>
        None = 0,
        /// <summary>
        /// 丙
        /// </summary>
        Hei = 1,
        /// <summary>
        /// 乙
        /// </summary>
        Otsu = 2,
        /// <summary>
        /// 甲
        /// </summary>
        Kou = 3,
    }

    public static class MapDifficultyEx
    {
        /// <summary>
        /// 作戦難易度を漢字に変換
        /// </summary>
        /// <param name="mapDif">MapDifficulty列挙体</param>
        /// <returns>作戦難易度の漢字</returns>
        public static string ToKanji(this MapDifficulty mapDif)
        {
            switch(mapDif)
            {
                case MapDifficulty.None: return "不明";
                case MapDifficulty.Hei: return "丙";
                case MapDifficulty.Otsu: return "乙";
                case MapDifficulty.Kou: return "甲";
                default: throw new NotImplementedException("MapDifficultyに対応するToKanjiが実装されていません");
            }
        }
    }

    
}
