using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;


/*
                            //////////////////////////GOAL////////////////////////////
                            
Serve to develop modules which works independently from an overall system logic

detect if the inclination of the SMA on X candles is +25 degrees or _25 degrees

When it works : 


*
*/
namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class UnitTestBot : Robot
    {
        [Parameter(DefaultValue = 30)]
        public double TP { get; set; }

        [Parameter(DefaultValue = 5)]
        public double SL { get; set; }

        [Parameter(DefaultValue = 1000)]
        public int InitialVolume { get; set; }

        [Parameter(DefaultValue = 2)]
        public int increaseAfter { get; set; }

        [Parameter(DefaultValue = 100)]
        public int TrailingStop { get; set; }

        [Parameter(DefaultValue = 80)]
        public int trailTrigger { get; set; }

        [Parameter(DefaultValue = 70)]
        public int limitOverbuy { get; set; }

        [Parameter(DefaultValue = 30)]
        public int limitOversell { get; set; }

        private SimpleMovingAverage sma;
        public string smaValue = "";

        //--------Unit test variables-----------
        private int Today;
        private bool OneShot;
        //--------------------------------------

        protected override void OnStart()
        {
            Today = Server.Time.Day;

            sma = Indicators.SimpleMovingAverage(MarketData.GetSeries(TimeFrame.Minute5).Close, 30);
        }

        protected override void OnTimer()
        {

        }
        protected override void OnBar()
        {
            CheckNewDay();
        }
        private void CheckNewDay()
        {

            if (Server.Time.Day != Today)
            {
                OneShot = true;
                Print(OneShot);
                Today = Server.Time.Day;
            }
            else
            {
                OneShot = false;
                Print(OneShot);
            }
        }
        protected override void OnTick()
        {

        }

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            var position = args.Position;
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

        }

    }
}
