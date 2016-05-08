using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShipAnalyze.DeserializeClassFormat
{
    public class ApiMapInfo
    {
        public int api_id { get; set; }
        public int api_cleared { get; set; }
        public int api_exboss_flag { get; set; }
        public int api_defeat_count { get; set; }
        public ApiEventmap api_eventmap { get; set; }
    }

    public class ApiEventmap
    {
        public int api_now_maphp { get; set; }
        public int api_max_maphp { get; set; }
        public int api_state { get; set; }
        public int api_selected_rank { get; set; }
    }

    public class RootMap
    {
        public List<ApiMapInfo> api_data { get; set; }
    }
}
