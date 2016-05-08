using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShipAnalyze.DeserializeClassFormat
{
    public class RootBattleResult
    {
        public ApiData api_data { get; set; }

        public class ApiGetShip
        {
            public int api_ship_id { get; set; }
            public string api_ship_type { get; set; }
            public string api_ship_name { get; set; }
            public string api_ship_getmes { get; set; }
        }
        public class ApiData
        {
            public string api_win_rank { get; set; }
            public ApiGetShip api_get_ship { get; set; }
        }
    }
}
