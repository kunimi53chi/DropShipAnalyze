using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShipAnalyze
{
    /// <summary>
    /// ドロップ集計用クラス
    /// </summary>
    public class DropSummary : Dictionary<DropCondition, DropResult>
    {
        /// <summary>
        /// 複数のドロップサマリーのマージ
        /// </summary>
        /// <param name="target">対象のサマリー</param>
        /// <returns>マージされたサマリー</returns>
        public DropSummary Merge(DropSummary target)
        {
            //自分自信をディープコピー
            var result = new DropSummary();
            foreach(var r in this)
            {
                result[r.Key] = r.Value.DeepCopy();
            }

            //ターゲットをマージ
            foreach(var t in target)
            {
                DropResult item;
                //マージ可能な場合
                if(result.TryGetValue(t.Key, out item))
                {
                    result[t.Key] = item.Merge(t.Value);
                }
                else
                {
                    result[t.Key] = t.Value.DeepCopy();
                }
            }

            return result;
        }
    }
}
