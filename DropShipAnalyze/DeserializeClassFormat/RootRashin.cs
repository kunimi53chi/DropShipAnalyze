using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShipAnalyze.DeserializeClassFormat
{
    /// <summary>
    /// 羅針盤データの逆シリアル化用のクラス
    /// </summary>
    public class ApiRashin
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
        /// セル番号
        /// </summary>
        public int api_no { get; set; }
    }

    public class RootRashin
    {
        public ApiRashin api_data { get; set; }
    }
}
