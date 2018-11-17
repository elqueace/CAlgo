using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class CloseAllPositionsOnATime : Robot
    {
        [Parameter(DefaultValue = 30)]
        public double TP { get; set; }

        [Parameter(DefaultValue = 5)]
        public double SL { get; set; }

        [Parameter(DefaultValue = 1000)]
        public int InitialVolume { get; set; }

        [Parameter(DefaultValue = 2)]
        public int increaseAfter { get; set; }


        //private int index;
        //private double open, high, low, close;
        private int counterPosition, counterLoss = 0;
        private int currentVolume;
        private bool oneShot = false;
        private RelativeStrengthIndex rsiMin10;
        private RelativeStrengthIndex rsi;
        protected override void OnStart()
        {
            currentVolume = InitialVolume;
            Positions.Closed += OnPositionsClosed;
            Timer.Start(60);
            var marketSeriesMin10 = MarketData.GetSeries(TimeFrame.Minute10);
            rsiMin10 = Indicators.RelativeStrengthIndex(marketSeriesMin10.Close, 14);
            rsi = Indicators.RelativeStrengthIndex(MarketSeries.Close, 14);
        }


        protected override void OnBar()
        {
            counterPosition = 0;
            foreach (var position in Positions)
            {
                counterPosition++;
            }

            if (counterPosition <= 0 && counterPosition <= 2)
            {
                if (rsi.Result.LastValue >= 70)
                {
                    Print(" RSI touched 70)");
                    oneShot = false;
                }
                else if (rsi.Result.LastValue < 70 && rsi.Result.LastValue > 55 && oneShot == false)
                {
                    Print("RSI Between 70 and 60");
                    ExecuteMarketOrder(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                    ExecuteMarketOrder(TradeType.Buy, Symbol, currentVolume, "Sell", SL, TP);

                    oneShot = true;
                }
                else if (rsi.Result.LastValue > 30 && rsi.Result.LastValue < 45 && oneShot == false)
                {
                    Print("RSI Between 40 and 30");
                    ExecuteMarketOrder(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                    ExecuteMarketOrder(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);

                    oneShot = true;
                }
                else if (rsi.Result.LastValue < 30)
                {
                    Print(" RSI touched 30");
                    oneShot = false;
                }
            }
            if (Server.Time.Hour == 12)
            {

                // Print("Its 12PM");
            }
            if (Server.Time.Hour == 15)
            {

                //-----//gn Print("Its 15PM");
            }

            if (Server.Time.Hour == 18)
            {

                //Print("Its 18PM");
            }
            //Timer.Start(60);
        }
        private void OnPositionsClosed(PositionClosedEventArgs args)
        {

            var position = args.Position;

            if (position.GrossProfit > 0)
            {
                currentVolume = InitialVolume;
                counterLoss = 0;
            }
            else
            {
                counterLoss++;
                if (counterLoss % increaseAfter == 0)
                {
                    currentVolume *= 2;
                }
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
