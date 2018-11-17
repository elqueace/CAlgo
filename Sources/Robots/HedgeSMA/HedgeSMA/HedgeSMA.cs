using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class HedgeSMA : Robot
    {
        [Parameter(DefaultValue = 2000)]
        public int InitialVolume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 30)]
        public int SL { get; set; }

        [Parameter("Take Profit", DefaultValue = 155)]
        public int TP { get; set; }

        [Parameter(DefaultValue = 1)]
        public int increaseAfter { get; set; }


        [Parameter("IF profit > ", DefaultValue = 80)]
        public int trailTrigger { get; set; }

        [Parameter("THEN trail at ", DefaultValue = 100)]
        public int TrailingStop { get; set; }

        private Random random = new Random();
        private int currentVolume, counterLoss = 0;


        public string phase, smaValue = "high";



        private bool CanTrade = true;

        private SimpleMovingAverage sma;
        protected override void OnStart()
        {
            sma = Indicators.SimpleMovingAverage(MarketData.GetSeries(TimeFrame.Hour).Close, 20);
        }

        protected override void OnTick()
        {
            crossPhase();
        }
        protected bool noActivePositions()
        {
            int counterPosition = 0;

            foreach (var p in Positions)
            {
                counterPosition++;
            }

            if (counterPosition == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void crossPhase()
        {
            if (Symbol.Bid > sma.Result.LastValue)
            {
                //Print(" SMA Higher = " + sma100.Result.LastValue);
                if (smaValue.Equals("low"))
                {
                    //takeHedgePositionsWithConditions("<=", 0);
                    if (noActivePositions() && CanTrade)
                    {
                        openSell(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                        openBuy(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                        //takeHedgePositionsWithConditions("<=", 0);
                        // CanTrade = false;
                    }

                }
                smaValue = "high";
            }
            else
            {
                //Print(" SMA Lower = " + sma100.Result.LastValue);
                if (smaValue.Equals("high"))
                {
                    //takeHedgePositionsWithConditions("<=", 0);
                    if (noActivePositions() && CanTrade)
                    {
                        openBuy(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                        openSell(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                        // takeHedgePositionsWithConditions("<=", 0);
                        //CanTrade = false;
                    }

                }
                smaValue = "low";
            }

            //
        }

        protected void openBuy(TradeType tradeType, Symbol symbol, int volume, string label, double SL, double TP)
        {
            var overSoldBuy = ExecuteMarketOrder(TradeType.Buy, symbol, volume, label, SL, TP);

        }

        protected void openSell(TradeType tradeType, Symbol symbol, int volume, string label, double SL, double TP)
        {
            var overSoldSell = ExecuteMarketOrder(TradeType.Sell, symbol, volume, label, SL, TP);

        }
        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
