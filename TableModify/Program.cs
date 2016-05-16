using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KCVDB.LocalAnalyze;

namespace TableModify
{
    class Program
    {
        static SummaryItem[,] summaries = new SummaryItem[19, 3];

        //エクセルのピボットテーブルにコピペする用のデータ
        static void Main(string[] args)
        {
            var dir = @"analyzed-file directory";
            var savedir = @"destination directory";
            foreach (var t in Directory.GetFiles(dir))
            {
                //summariesの初期化
                foreach(var i in Enumerable.Range(0, summaries.GetLength(0)))
                foreach(var j in Enumerable.Range(0, summaries.GetLength(1)))
                {
                    summaries[i, j] = new SummaryItem();
                }

                var txt = File.ReadAllLines(t);
                foreach(var l in txt)
                {
                    ParseLine(l);
                }

                //95%CIの計算
                foreach(var i in Enumerable.Range(0, summaries.GetLength(0)))
                foreach(var j in Enumerable.Range(0, summaries.GetLength(1)))
                {
                    summaries[i, j].CalcCI();
                }

                //テキスト化
                var sb = new StringBuilder();
                foreach (var i in Enumerable.Range(0, summaries.GetLength(0)))
                {
                    var sb2 = new StringBuilder();
                    foreach (var j in Enumerable.Range(0, summaries.GetLength(1)))
                    {
                        sb2.Append(summaries[i, j].GetText());
                        sb2.Append("\t");
                    }
                    sb.AppendLine(sb2.ToString().TrimEnd('\t'));
                }

                //保存
                File.WriteAllText(savedir+"\\" + Path.GetFileName(t), sb.ToString());
            }
        }

        static void ParseLine(string line)
        {
            // 行：0～8番目までがD　9～17番目までがF 18番目が総計
            var cell = line.Split('\t');
            if (cell.Length < 11) return;
            //マスの取得
            int masu = int.Parse(cell[2]);

            //所持数
            int possess = int.Parse(cell[5].Replace("隻", ""));
            //個別の行インデックス
            int row = ((masu == 4) ? 0 : 1) * 9 + possess + 1;
            //小計の行インデックス
            int sumrow = ((masu == 4) ? 0 : 1) * 9;

            //ドロップ数
            int drop = int.Parse(cell[9]);
            //総数
            int trial = int.Parse(cell[10]);

            //難易度
            int difficulty = -1;
            switch(cell[3])
            {
                case "甲": difficulty = 0; break;
                case "乙": difficulty = 1; break;
                case "丙": difficulty = 2; break;
            }
            if(difficulty == -1) return;

            //個別集計に追加
            summaries[row, difficulty].Num += drop;
            summaries[row, difficulty].Total += trial;

            //小計に追加
            summaries[sumrow, difficulty].Num += drop;
            summaries[sumrow, difficulty].Total += trial;

            //合計に追加
            summaries[summaries.GetLength(0) - 1, difficulty].Num += drop;
            summaries[summaries.GetLength(0) - 1, difficulty].Total += trial;
        }
    }

    public class SummaryItem
    {
        public double Lower { get; set; }
        public double Middle { get; set; }
        public double Upper { get; set; }
        public int Num { get; set; }
        public int Total { get; set; }

        public void CalcCI()
        {
            if (Total == 0) return;

            var ci = Statics.ConfidenceIntervalBinomial((double)Num / (double)Total, Total);

            Lower = ci.Lower;
            Middle = ci.Median;
            Upper = ci.Upper;
        }

        public string GetText()
        {
            if (Total == 0) return string.Join("\t", Enumerable.Repeat("", 5));

            return Lower.ToString("P2") + "\t" + Middle.ToString("P2") + "\t" + Upper.ToString("P2") + "\t" + Num +"\t"+ Total;
        }
    }
}
