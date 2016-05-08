using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShipAnalyze
{
    /// <summary>
    /// ドロップ集計結果のクラス
    /// </summary>
    public class DropResult
    {
        /// <summary>
        /// ドロップテーブル（KeyはShipID）
        /// </summary>
        public SortedDictionary<int, DropTableItem> Table { get; private set; }
        /// <summary>
        /// この条件のもとでの試行回数合計
        /// </summary>
        public int TotalNum { get; private set; }

        public DropResult()
        {
            Table = new SortedDictionary<int, DropTableItem>();
        }

        /// <summary>
        /// ドロップテーブルに追加
        /// </summary>
        /// <param name="shipid"></param>
        public void AddTable(int shipid)
        {
            //テーブルに追加
            DropTableItem item;
            if(Table.TryGetValue(shipid, out item))
            {
                item.FoundNum++;
            }
            else
            {
                item = new DropTableItem()
                {
                    ShipId = shipid,
                    FoundNum = 1,
                };
            }

            Table[shipid] = item;

            //総試行回数のプラス
            TotalNum++;
        }

        /// <summary>
        /// ドロップ率の計算
        /// </summary>
        /// <param name="shipid">対象の艦のID</param>
        /// <returns>ドロップ率</returns>
        public double GetDropRatio(int shipid)
        {
            DropTableItem item;
            if(Table.TryGetValue(shipid, out item))
            {
                return (double)item.FoundNum / (double)this.TotalNum;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// ドロップ数の取得
        /// </summary>
        /// <param name="shipid">対象艦のID</param>
        /// <returns>ドロップ数</returns>
        public int GetDropNum(int shipid)
        {
            DropTableItem item;
            if (Table.TryGetValue(shipid, out item))
            {
                return item.FoundNum;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// ２つのレコードをマージ
        /// </summary>
        /// <param name="target">マージするレコード</param>
        /// <returns>マージされたレコード</returns>
        public DropResult Merge(DropResult target)
        {
            //自分自身をディープコピー
            var result = new DropResult();
            foreach(var d in this.Table)
            {
                result.Table[d.Key] = d.Value.DeepCopy();
            }
            result.TotalNum = this.TotalNum;

            //ターゲットをマージする
            foreach(var d in target.Table)
            {
                DropTableItem item;
                //マージ元にあった場合
                if (result.Table.TryGetValue(d.Key, out item))
                {
                    result.Table[d.Key] = item.Merge(d.Value);
                }
                //なかった場合
                else
                {
                    result.Table[d.Key] = d.Value.DeepCopy();
                }
            }
            result.TotalNum += target.TotalNum;

            return result;
        }

        /// <summary>
        /// ディープコピー
        /// </summary>
        /// <returns>ディープコピーされたインスタンス</returns>
        public DropResult DeepCopy()
        {
            var result = new DropResult();
            foreach(var t in this.Table)
            {
                result.Table[t.Key] = t.Value;
            }
            result.TotalNum = this.TotalNum;

            return result;
        }
    }

    /// <summary>
    /// ドロップ集計のTableのクラス
    /// </summary>
    public class DropTableItem
    {
        /// <summary>
        /// 船のID
        /// </summary>
        public int ShipId { get; set; }
        /// <summary>
        /// 発見数
        /// </summary>
        public int FoundNum { get; set; }

        /// <summary>
        /// 2つのレコードをマージ
        /// </summary>
        /// <param name="target">マージするターゲット</param>
        /// <returns>マージされたレコード</returns>
        public DropTableItem Merge(DropTableItem target)
        {
            if (this.ShipId != target.ShipId) throw new ArgumentException("マージするIDが異なります");

            var result = new DropTableItem();

            result.ShipId = this.ShipId;
            result.FoundNum = this.FoundNum + target.FoundNum;

            return result;
        }

        public DropTableItem DeepCopy()
        {
            return new DropTableItem()
            {
                ShipId = this.ShipId,
                FoundNum = this.FoundNum,
            };
        }
    }
}
