using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class UNIT_CheckHour : Robot
    {
        private SimpleMovingAverage sma;

        protected override void OnStart()
        {
            sma = Indicators.SimpleMovingAverage(MarketData.GetSeries(TimeFrame.Hour).Close, 50);
            Timer.Start(3);
        }

        protected override void OnTick()
        {
            // Put your core logic here
        }
        protected override void OnBar()
        {
            CheckHour();
        }
//TESTED
        private bool CheckHour()
        {
            if (Server.Time.Hour >= 5 && Server.Time.Hour <= 17)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
