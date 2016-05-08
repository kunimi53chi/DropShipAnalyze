using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace DropShipAnalyze
{
    class Program
    {        
        static void Main(string[] args)
        {
            //------ パラメーター設定
            var files = Directory.GetFiles(@"Enter your log-file directory");
            int id = 431;//U-511 431　ターゲットのID
            int[] sameids =  new int[] { 431, 334, 436 }; //同一とみなす艦のID
            string shipname = "U-511";
            string date = "2016/05/07";
            //-------------------------

            //--同一艦
            //new int[] { 431, 334, 436 };//U-511 431
            //new int[] { 155, 403 };//伊401 155
            //new int[] { 421, 330 };//秋月 421
            //new int[] { 422, 346 };//照月 421


            var totalsummary = new DropSummary();
            int cnt = 1;

            foreach (var f in files)
            {
                Console.WriteLine("Analyzing {0} / {1}", cnt, files.Length);
                cnt++;

                //テキストの読み込み
                Analyzer analyzer = null;
                switch(Path.GetExtension(f))
                {
                    //.logファイルならそのまま読み込み
                    case ".log":
                        try
                        {
                            var fullline = File.ReadAllLines(f);

                            analyzer = new Analyzer(fullline);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(".log-file read failed");
                            Console.WriteLine(ex.ToString());
                            continue;
                        }
                        break;
                    //.gzファイルは解凍して読み込み
                    case ".gz":
                        try
                        {
                            int num;
                            byte[] buf = new byte[65536];
                            var lines = new List<string>();
                            using(var inStream = new FileStream(f, FileMode.Open, FileAccess.Read))
                            using(var decompStream = new GZipStream(inStream, CompressionMode.Decompress))
                            using(var ms = new MemoryStream())
                            {
                                //解凍
                                while ((num = decompStream.Read(buf, 0, buf.Length)) > 0)
                                    ms.Write(buf, 0, num);
                                //Streamのポジションを戻す
                                ms.Position = 0;
                                //ストリームの読み込み
                                using(var sr = new StreamReader(ms))
                                {
                                    while (sr.Peek() >= 0)
                                    {
                                        var line = sr.ReadLine();
                                        if (!string.IsNullOrEmpty(line)) lines.Add(line);
                                    }
                                }
                            }

                            analyzer = new Analyzer(lines);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(".gz-file read failed");
                            Console.WriteLine(ex.ToString());
                            continue;
                        }
                        break;
                }

                if(analyzer == null)
                {
                    Console.WriteLine("analyzer create failed");
                    continue;
                }
                //解析
                analyzer.Analyze(id, sameids);

                //テーブルのマージ
                if(analyzer.Summary != null) totalsummary = totalsummary.Merge(analyzer.Summary);
            }


            //結果表示用
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ID = {0} {1}({2})", id, shipname, date).AppendLine();
            foreach (var d in totalsummary.OrderBy(x => x.Key.GetHashCode()))
            {
                if (d.Value.GetDropRatio(id) == 0.0) continue;

                sb.Append(d.Key.GetDisplayString());
                sb.Append("\t");
                sb.Append(d.Value.GetDropRatio(id).ToString("P2"));
                sb.Append("\t");
                sb.Append(d.Value.GetDropNum(id));
                sb.Append(" / ");
                sb.Append(d.Value.TotalNum);

                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());

            File.WriteAllText(id + "_" + date.Replace("/", "") + ".txt", sb.ToString());
        }
        
    }
}
