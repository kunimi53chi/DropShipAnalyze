using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropShipAnalyze.DeserializeClassFormat;

namespace DropShipAnalyze
{
    /// <summary>
    /// ドロップ条件の構造体
    /// </summary>
    public struct DropCondition : IEquatable<DropCondition>
    {
        /// <summary>
        /// 海域番号（5-4なら5）
        /// </summary>
        public int api_maparea_id { get; set; }
        /// <summary>
        /// マップ番号（5-4なら4）
        /// </summary>
        public int api_mapinfo_no { get; set; }
        /// <summary>
        /// 作戦難易度
        /// </summary>
        public MapDifficulty Difficulty { get; set; }
        /// <summary>
        /// セル番号
        /// </summary>
        public int api_no { get; set; }
        /// <summary>
        /// 勝敗条件
        /// </summary>
        public WinRank WRank { get; set; }
        /// <summary>
        /// 該当艦の所持数
        /// </summary>
        public int TargetShipPossess { get; set; }

        /// <summary>
        /// ドロップ条件のインスタンスを作成
        /// </summary>
        /// <param name="rashinData">羅針盤データ</param>
        /// <param name="mapDifficulty">作戦難易度</param>
        /// <param name="winRank">勝敗</param>
        /// <param name="targetShipPossess">同じ艦の所持数</param>
        /// <returns>ドロップ条件のインスタンス</returns>
        public static DropCondition CreateInstance(ApiRashin rashinData, int mapDifficulty, string winRank, int targetShipPossess)
        {
            var instance = new DropCondition();

            //羅針盤データのセット
            instance.api_maparea_id = rashinData.api_maparea_id;
            instance.api_mapinfo_no = rashinData.api_mapinfo_no;
            instance.api_no = rashinData.api_no;

            //作戦難易度
            instance.Difficulty = (MapDifficulty)mapDifficulty;

            //勝敗条件
            instance.WRank = WinRankEx.GetWinRank(winRank);

            //該当艦の所持数
            instance.TargetShipPossess = targetShipPossess;

            return instance;
        }

        public string GetDisplayString()
        {
            return api_maparea_id + "\t" + api_mapinfo_no + "\t" + api_no + "\t" + Difficulty.ToKanji() + "\t" + WRank.ToStr() + "\t" + TargetShipPossess + "隻";
        }


        #region IEquatableインターフェイス、オーバーライド
        public bool Equals(DropCondition other)
        {
            return
                (this.api_maparea_id == other.api_maparea_id) &&
                (this.api_mapinfo_no == other.api_mapinfo_no) &&
                (this.Difficulty == other.Difficulty) &&
                (this.api_no == other.api_no) &&
                (this.WRank == other.WRank) &&
                (this.TargetShipPossess == other.TargetShipPossess);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DropCondition)) return false;
            else return Equals((DropCondition)obj);
        }

        public override int GetHashCode()
        {
            return
                this.api_maparea_id * 10000000 +
                this.api_mapinfo_no * 1000000 +
                this.api_no * 10000 +
                (int)this.Difficulty * 1000 +
                (int)this.WRank * 100 +
                this.TargetShipPossess;
        }

        public static bool operator !=(DropCondition d1, DropCondition d2)
        {
            return !d1.Equals(d2);
        }

        public static bool operator ==(DropCondition d1, DropCondition d2)
        {
            return d1.Equals(d2);
        }
        #endregion
    }
}
