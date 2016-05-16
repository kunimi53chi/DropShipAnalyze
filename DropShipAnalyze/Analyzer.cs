using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using DropShipAnalyze.DeserializeClassFormat;
using KCVDB.LocalAnalyze;

namespace DropShipAnalyze
{
    /// <summary>
    /// ファイル単位のアナライザー
    /// </summary>
    public class Analyzer
    {
        /// <summary>
        /// ファイルから読み込んだテキストデータ（行単位）
        /// </summary>
        public IEnumerable<KCVDBRow> ParsedFullTextLines { get; private set; }

        /// <summary>
        /// 出撃前に読み込まれるマップデータ
        /// </summary>
        public List<ApiMapInfo> MapInfo { get; private set; }
        /// <summary>
        /// 羅針盤のデータ
        /// </summary>
        public ApiRashin NowCell { get; private set; }
        /// <summary>
        /// 母港APIのResponseBody
        /// </summary>
        public string PortResponseBody { get; private set; }

        /// <summary>
        /// ドロップ集計
        /// </summary>
        public DropSummary Summary { get; private set; }

        //コンストラクタ
        public Analyzer(IEnumerable<KCVDBRow> parsedFullTextLines)
        {
            if (parsedFullTextLines == null) throw new NullReferenceException("parsedFullTextDataがNullです");

            this.ParsedFullTextLines = parsedFullTextLines;

            this.MapInfo = new List<ApiMapInfo>();
            this.NowCell = new ApiRashin();

            this.Summary = new DropSummary();
        }

        /// <summary>
        /// 解析処理
        /// </summary>
        /// <param name="targetShipid">探索する船のID</param>
        /// <param name="sameShipIds">同一艦のID一覧</param>
        public void Analyze(int targetShipid, int[] sameShipIds)
        {
            //行単位の分析
            foreach (var l in ParsedFullTextLines)
            {
                switch (l.KcsapiParent)
                {
                    case "api_port":
                        ReadPort(l.ResponseValue);
                        break;
                    case "api_get_member":
                        if (l.KcsapiChildren == "mapinfo")
                        {
                            ReadMapInfo(l.ResponseValue);
                        }
                        break;
                    case "api_req_map":
                        switch (l.KcsapiChildren)
                        {
                            case "select_eventmap_rank":
                                ReadSelectEventRank(l.RequestValue);
                                break;
                            case "start":
                            case "next":
                                ReadMapStartOrNext(l.ResponseValue);
                                break;
                        }
                        break;
                    default:
                        if (l.KcsapiChildren == "battleresult" && NowCell != null 
                            && NowCell.api_maparea_id == 34 && NowCell.api_mapinfo_no == 3 && (NowCell.api_no == 4 || NowCell.api_no == 6))
                        {
                            ReadBattleResult(l.ResponseValue, targetShipid, sameShipIds);
                        }
                        break;
                }
            }
        }

        public void ReadMapInfo(string responseBody)
        {
            var strjson = responseBody.Replace("svdata=", "");
            var root = JsonConvert.DeserializeObject<RootMap>(strjson);
            if (root != null && root.api_data != null) MapInfo = root.api_data;
        }

        public void ReadPort(string responseBody)
        {
            NowCell = new ApiRashin();
            PortResponseBody = responseBody;
        }

        public void ReadSelectEventRank(string requestBody)
        {
            if (MapInfo == null) return;

            //マップNo
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(requestBody, @"api%5Fmap%5Fno=([0-9]+)");
            int map_no = Convert.ToInt32(match.Groups[1].Value);
            //ランク
            match = System.Text.RegularExpressions.Regex.Match(requestBody, @"api%5Frank=([0-9]+)");
            int rank = Convert.ToInt32(match.Groups[1].Value);
            //エリアID
            match = System.Text.RegularExpressions.Regex.Match(requestBody, @"api%5Fmaparea%5Fid=([0-9]+)");
            int area = Convert.ToInt32(match.Groups[1].Value);

            //イベントデータの変更
            var eventdata = MapInfo.Where(x => x.api_id == area * 10 + map_no).FirstOrDefault();
            if (eventdata != null) eventdata.api_eventmap.api_selected_rank = rank;
        }


        public void ReadMapStartOrNext(string responseBody)
        {
            var strjson = responseBody.Replace("svdata=", "");
            var rashin = JsonConvert.DeserializeObject<RootRashin>(strjson);

            //セルデータの変更
            NowCell = rashin.api_data;
        }

        public void ReadBattleResult(string responseBody, int targetShipId, int[] sameShipIds)
        {
            if (NowCell == null) return;

            var strjson = responseBody.Replace("svdata=", "");
            var battleResult = JsonConvert.DeserializeObject<RootBattleResult>(strjson);
            if (battleResult.api_data == null) return;

            //勝敗ランクの取得
            var winrank = WinRankEx.GetWinRank(battleResult.api_data.api_win_rank);
            if (winrank != WinRank.S) return;

            //作戦難易度
            if (NowCell == null || MapInfo == null) return;
            var map = MapInfo.Where(x => x.api_id == NowCell.api_maparea_id * 10 + NowCell.api_mapinfo_no).FirstOrDefault();
            var difficulty = 0;
            if (map != null && map.api_eventmap != null)
            {
                difficulty = map.api_eventmap.api_selected_rank;
            }

            //ドロップの取得
            int id = 0;
            if (battleResult.api_data.api_get_ship != null)
            {
                id = battleResult.api_data.api_get_ship.api_ship_id;
            }

            //同型艦の所持数
            int numTargetShip = 0;
            if (PortResponseBody != null)
            {
                foreach (var i in sameShipIds)
                {
                    var match = System.Text.RegularExpressions.Regex.Matches(PortResponseBody, "\"api_ship_id\":" + i + ",\"api_lv\"");
                    numTargetShip += match.Count;
                }
            }

            //サーチする艦以外はドロップなしとする
            if (id != targetShipId) id = 0;


            //ドロップ条件の取得
            var condition = DropCondition.CreateInstance(NowCell, difficulty, battleResult.api_data.api_win_rank, numTargetShip);

            //テーブルに追加
            DropResult result;
            if (!Summary.TryGetValue(condition, out result)) result = new DropResult();
            result.AddTable(id);

            Summary[condition] = result;

        }
    }

}
