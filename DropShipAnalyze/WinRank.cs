using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShipAnalyze
{
    /// <summary>
    /// 勝利条件の列挙体
    /// </summary>
    public enum WinRank
    {
        /// <summary>
        /// 不明
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// S勝利
        /// </summary>
        S = 1,
        /// <summary>
        /// A勝利
        /// </summary>
        A = 2,
        /// <summary>
        /// B勝利
        /// </summary>
        B = 3,
        /// <summary>
        /// C敗北
        /// </summary>
        C = 4,
        /// <summary>
        /// D敗北
        /// </summary>
        D = 5,
        /// <summary>
        /// E敗北
        /// </summary>
        E = 6,
    }
    /// <summary>
    /// WinRankの拡張クラス
    /// </summary>
    public static class WinRankEx
    {
        /// <summary>
        /// WinRank列挙体を取得
        /// </summary>
        /// <param name="winRankStr">勝敗条件の文字列</param>
        /// <returns>WinRank列挙体</returns>
        public static WinRank GetWinRank(string winRankStr)
        {
            switch (winRankStr)
            {
                case "S": return WinRank.S;
                case "A": return WinRank.A;
                case "B": return WinRank.B;
                case "C": return WinRank.C;
                case "D": return WinRank.D;
                case "E": return WinRank.E;
                default: return WinRank.Unknown;
            }
        }

        /// <summary>
        /// WinRankを文字列化
        /// </summary>
        /// <param name="winRank">WinRank列挙体</param>
        /// <returns>文字列</returns>
        public static string ToStr(this WinRank winRank)
        {
            switch(winRank)
            {
                case WinRank.S: return "S";
                case WinRank.A: return "A";
                case WinRank.B: return "B";
                case WinRank.C: return "C";
                case WinRank.D: return "D";
                case WinRank.E: return "E";
                case WinRank.Unknown: return "不明";
                default: throw new NotImplementedException();
            }
        }
    }
}
