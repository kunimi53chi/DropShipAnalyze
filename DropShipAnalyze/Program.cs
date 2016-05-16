using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using KCVDB.LocalAnalyze;

namespace DropShipAnalyze
{
    class Program
    {

        static object lockObject = new object();
        
        static void Main(string[] args)
        {
            //------ パラメーター設定
            var files = Directory.GetFiles(@"Enter your log-file directory");
            int id = 155;//U-511 431　ターゲットのID
            int[] sameids =  new int[] { 155, 403 }; //同一とみなす艦のID
            string shipname = "伊401";
            string date = "2016/05/03";
            //-------------------------

            //--同一艦
            //new int[] { 431, 334, 436 };//U-511 431
            //new int[] { 155, 403 };//伊401 155
            //new int[] { 421, 330 };//秋月 421
            //new int[] { 422, 346 };//照月 421


            var totalsummary = new DropSummary();
            int cnt = 1;

            Console.Title = "Started : " + DateTime.Now;

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 2 }, f =>
            {
                Console.WriteLine("Analyzing {0} / {1}", cnt, files.Length);
                cnt++;

                var ext = Path.GetExtension(f);
                if (ext != ".gz" && ext != ".log") return;


                var fullline = KCVDBLogFile.ParseAllLines(f);
                var analyzer = new Analyzer(fullline);

                //解析
                analyzer.Analyze(id, sameids);

                //テーブルのマージ
                if (analyzer.Summary != null)
                {
                    lock(lockObject)
                        totalsummary = totalsummary.Merge(analyzer.Summary);
                }
            });


            //結果表示用
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ID = {0} {1}({2})", id, shipname, date).AppendLine();
            foreach (var d in totalsummary.OrderBy(x => x.Key.GetHashCode()))
            {
                //if (d.Value.GetDropRatio(id) == 0.0) continue;

                var prob = d.Value.GetDropRatio(id);
                var num = d.Value.GetDropNum(id);
                var ci95 = Statics.ConfidenceIntervalBinomial(prob, d.Value.TotalNum);

                sb.Append(d.Key.GetDisplayString());
                sb.Append("\t");
                sb.Append(ci95.Lower.ToString("P2"));
                sb.Append("\t");
                sb.Append(prob.ToString("P2"));
                sb.Append("\t");
                sb.Append(ci95.Upper.ToString("P2"));
                sb.Append("\t");
                sb.Append(num);
                sb.Append("\t");
                sb.Append(d.Value.TotalNum);

                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());

            File.WriteAllText(id + "_" + date.Replace("/", "") + ".txt", sb.ToString());
        }
        
    }
}
