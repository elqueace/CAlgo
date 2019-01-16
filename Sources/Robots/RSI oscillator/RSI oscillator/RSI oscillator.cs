using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RSIoscillator : Robot
    {

        [Parameter(DefaultValue = 5)]
        public double SL { get; set; }

        [Parameter(DefaultValue = 30)]
        public double TP { get; set; }

        [Parameter(DefaultValue = 1000)]
        public int InitialVolume { get; set; }

        [Parameter(DefaultValue = 2)]
        public int increaseAfter { get; set; }

        [Parameter(DefaultValue = 70)]
        public int limitOverbuy { get; set; }

        [Parameter(DefaultValue = 55)]
        public int NeutralSuperior { get; set; }

        [Parameter(DefaultValue = 45)]
        public int NeutralInferior { get; set; }

        [Parameter(DefaultValue = 30)]
        public int limitOversell { get; set; }

        [Parameter(DefaultValue = 30)]
        public int MoneyTarget { get; set; }

        private bool overbuy, oversell = false;
        private string lastDirection = "";
        private RelativeStrengthIndex rsi;
        private int currentVolume, counterLoss = 0;
        protected override void OnStart()
        {
            Positions.Closed += OnPositionsClosed;
            currentVolume = InitialVolume;
            rsi = Indicators.RelativeStrengthIndex(MarketSeries.Close, 14);
        }

        protected override void OnTick()
        {

        }
        protected override void OnBar()
        {
            if (rsi.Result.LastValue >= limitOverbuy)
            {
                //Print(" RSI touched 70)");
                overbuy = true;

            }

            if (rsi.Result.LastValue < limitOversell)
            {
                //Print(" RSI touched 30");
                oversell = true;
            }

            if (rsi.Result.LastValue < NeutralSuperior && overbuy == true)
            {
                //Print("RSI crossed below " + limitOverbuy);
                overbuy = false;
                //if (Server.Time.Hour >= 6 && Server.Time.Hour <= 17)
                //{
                //  if (lastDirection.Equals("Buy") || lastDirection.Equals(""))
                //  {
                OpenPosition(TradeType.Sell);
                //OpenPosition(TradeType.Buy);
                //takeHedgePositionsWithConditions("<=", 0);
                // }
                //}
            }

            if (rsi.Result.LastValue > NeutralInferior && oversell == true)
            {
                //Print("RSI crossed above " + limitOversell);
                oversell = false;
                //if (Server.Time.Hour >= 6 && Server.Time.Hour <= 17)
                //{
                //if (lastDirection.Equals("Sell") || lastDirection.Equals(""))
                //{
                OpenPosition(TradeType.Buy);
                // OpenPosition(TradeType.Sell);
                //takeHedgePositionsWithConditions("<=", 0);
                //}
                //}
            }


            foreach (var p in Positions)
            {

                /////////////////////////////////////////////////////////////Setting up Stop Loss////////////////////////////////////////////////////////
                if (p.TradeType == TradeType.Buy && rsi.Result.LastValue <= limitOversell)
                {
                    ClosePosition(p);
                }

                if (p.TradeType == TradeType.Sell && rsi.Result.LastValue >= limitOverbuy)
                {
                    ClosePosition(p);
                }

                //////////////////////////////////////////////////////////Setting up Take Profit////////////////////////////////////////////////////////

                if (p.TradeType == TradeType.Buy && rsi.Result.LastValue > limitOverbuy)
                {
                    ClosePosition(p);
                }

                if (p.TradeType == TradeType.Sell && rsi.Result.LastValue < limitOversell)
                {
                    ClosePosition(p);
                }
            }
        }

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            // Print("newBalance = " + newBalance + " - oldBalance = " + oldBalance);

            var position = args.Position;

            //IF a win position triggered with TP THEN reinitialize the volume and count the gain
            if (position.Pips >= 30)
            {
                counterLoss = 0;
                currentVolume = InitialVolume;
            }


            //IF position lost THEN increment the right counter to update thr volume
            if (position.Pips < -1)
            {

                counterLoss++;

                if (counterLoss % increaseAfter == 0)
                {
                    currentVolume *= 2;
                }
            }

        }
        protected void OpenPosition(TradeType tradeType)
        {

            int counterPosition = 0;
            foreach (var position in Positions)
            {
                counterPosition++;
            }

            if (counterPosition <= 0)
            {
                var openedPosition = ExecuteMarketOrder(tradeType, Symbol, currentVolume, "Label", SL, TP);

                if (tradeType == TradeType.Buy)
                {
                    lastDirection = "Buy";
                }
                else
                {
                    lastDirection = "Sell";
                }

            }
        }
    }
}
