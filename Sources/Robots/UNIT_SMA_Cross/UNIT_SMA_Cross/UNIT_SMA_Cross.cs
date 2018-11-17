using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class UNIT_SMA_Cross : Robot
    {
        private SimpleMovingAverage sma;

        protected override void OnStart()
        {
            sma = Indicators.SimpleMovingAverage(MarketData.GetSeries(TimeFrame.Hour).Close, 50);
        }

        protected override void OnTick()
        {
            // Put your core logic here
        }
        protected override void OnBar()
        {
            CheckDistanceFromSMA();
        }
//TESTED
        private string CheckDistanceFromSMA()
        {
            if (Symbol.Bid - (sma.Result.LastValue + 0.0) > 0)
            {
                Print("AbovweTarget");
                return "AboveTarget";
            }
            if (Symbol.Bid - (sma.Result.LastValue - 0.0) < 0)
            {
                Print("BelowTarget");
                return "BelowTarget";
            }
            Print("--");

            return "--";

        }
        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
