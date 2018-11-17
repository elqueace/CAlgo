using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Sniffer : Robot
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

        [Parameter("SmaAmplitude", DefaultValue = 35)]
        public double SmaAmplitude { get; set; }


        [Parameter(DefaultValue = 70)]
        public int limitOverbuy { get; set; }

        [Parameter(DefaultValue = 30)]
        public int limitOversell { get; set; }

        private Random random = new Random();
        private int currentVolume, counterLoss = 0;
        private double balance, overallGain, overallPips = 0;
        private bool overbuy, oversell = false;
        public string phase, smaValue = "";
        public int increaseAfterDefault;

        //private RelativeStrengthIndex rsiMin10;
        private RelativeStrengthIndex rsi;
        private SimpleMovingAverage sma;
        protected override void OnStart()
        {
            increaseAfterDefault = increaseAfter;
            phase = "Cross";
            Positions.Closed += OnPositionsClosed;
            rsi = Indicators.RelativeStrengthIndex(MarketData.GetSeries(TimeFrame.Hour).Close, 14);
            sma = Indicators.SimpleMovingAverage(MarketData.GetSeries(TimeFrame.).Close, 666);
            currentVolume = InitialVolume;
            balance = Account.Balance;
            Timer.Start(1);
        }

        protected override void OnBar()
        {

        }
        protected override void OnTimer()
        {
            //if (Server.Time.Hour >= 4 && Server.Time.Hour <= 4)
            // {
            indicator("SMA");

            //}

            //trailAllPositions();
            ///phase = "Cross";
            //  Print("Timer");
            // Timer.Stop();

        }

        protected override void OnTick()
        {

        }

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            //Print("Closed");
            var position = args.Position;
            overallPips += position.Pips;
            Print("overallPips = " + overallPips);
            if (overallPips < -SL)
            {
                dealWithNegativeClosedPositions();
                //dealWithPositiveClosedPositions();
            }
            if (overallPips > -5)
            {

            }

            if (position.NetProfit > 0 && position.Pips > 100)
            {
                dealWithPositiveClosedPositions();
            }
            ////overallGain += position.NetProfit;
            //Print("overallGain = " + overallGain);


        }

        protected void dealWithPositiveClosedPositions()
        {
            currentVolume = InitialVolume;
            overallPips = 0;
            Print("overallPips reset Win" + overallPips);
        }
        protected void dealWithNegativeClosedPositions()
        {
            updateVolumePositionsManual();
            //currentVolume *= 2;
            overallPips = 0;
            Print("overallPips reset Loss " + overallPips);

        }
        protected void trailAllPositions()
        {

            foreach (var position in Positions)
            {

                //if position is a buy
                if (position.TradeType == TradeType.Buy)
                {
                    //check the actual gain/loss
                    double distance = Symbol.Bid - position.EntryPrice;

                    if (distance >= trailTrigger * Symbol.PipSize)
                    {
                        var newStopLossPrice = Math.Round(position.EntryPrice + TrailingStop * Symbol.PipSize, Symbol.Digits);
                        ModifyPosition(position, newStopLossPrice, position.TakeProfit);

                    }
                }
                if (position.TradeType == TradeType.Sell)
                {

                    //check the actual gain/loss
                    double distance = position.EntryPrice - Symbol.Ask;
                    // Print("distance" + distance);
                    if (distance >= trailTrigger * Symbol.PipSize)
                    {
                        var newStopLossPrice = Math.Round(position.EntryPrice - TrailingStop * Symbol.PipSize, Symbol.Digits);
                        //Print("trailTrigger * Symbol.PipSize" + newStopLossPrice);

                        ModifyPosition(position, newStopLossPrice, position.TakeProfit);


                    }
                }

            }
        }
        protected void updateVolumePositionsManual()
        {
            switch (currentVolume)
            {
                case 1000:
                    currentVolume = 2000;
                    break;
                case 2000:
                    currentVolume = 3000;
                    break;
                case 3000:
                    currentVolume = 4000;
                    break;
                case 4000:
                    currentVolume = 8000;
                    break;
                case 8000:
                    currentVolume = 16000;
                    break;
                case 16000:
                    currentVolume = 20000;
                    break;
                case 20000:
                    currentVolume = 32000;
                    break;
                case 32000:
                    currentVolume = 45000;
                    break;
                case 45000:
                    currentVolume = 64000;
                    break;
                //10eme MISE
                case 64000:
                    currentVolume = 128000;
                    break;
                case 128000:
                    currentVolume = 170000;
                    break;
                case 170000:
                    currentVolume = 240000;
                    break;
                case 240000:
                    currentVolume = 360000;
                    break;
                case 360000:
                    currentVolume = 512000;
                    break;
                case 512000:
                    currentVolume = 760000;
                    break;

                case 760000:
                    currentVolume = 1000000;
                    break;
                case 1000000:
                    currentVolume = 1500000;
                    break;
                case 1500000:
                    currentVolume = 2000000;
                    break;
                case 2000000:
                    currentVolume = 2500000;
                    break;
                //15eme MISE
                case 2500000:
                    currentVolume = 3000000;
                    break;
                case 36000:
                    currentVolume = 43000;
                    break;
                case 43000:
                    currentVolume = 50000;
                    break;
                case 50000:
                    currentVolume = 60000;
                    break;
                case 65000:
                    currentVolume = 72000;
                    break;
                case 72000:
                    currentVolume = 80000;
                    break;
                case 81000:
                    currentVolume = 100000;
                    break;
            }
        }
        protected void takeHedgePositionsWithConditions(string condition, int nbOpened)
        {
            int counterPosition = 0;
            foreach (var position in Positions)
            {
                counterPosition++;
            }

            switch (condition)
            {
                case "<":
                    if (counterPosition < nbOpened)
                    {
                        openBuy(TradeType.Buy, Symbol, currentVolume, "overSoldBuy", SL, TP);
                        openSell(TradeType.Sell, Symbol, currentVolume, "overSoldSell", SL, TP);
                    }
                    break;
                case "<=":
                    if (counterPosition <= nbOpened)
                    {
                        openBuy(TradeType.Buy, Symbol, currentVolume, "overSoldBuy", SL, TP);
                        openSell(TradeType.Sell, Symbol, currentVolume, "overSoldSell", SL, TP);
                    }
                    break;
                case "==":
                    if (counterPosition == nbOpened)
                    {
                        openBuy(TradeType.Buy, Symbol, currentVolume, "overSoldBuy", SL, TP);
                        openSell(TradeType.Sell, Symbol, currentVolume, "overSoldSell", SL, TP);
                    }
                    break;
                case ">=":
                    if (counterPosition >= nbOpened)
                    {
                        openBuy(TradeType.Buy, Symbol, currentVolume, "overSoldBuy", SL, TP);
                        openSell(TradeType.Sell, Symbol, currentVolume, "overSoldSell", SL, TP);
                    }
                    break;
                case ">":
                    if (counterPosition > nbOpened)
                    {
                        openBuy(TradeType.Buy, Symbol, currentVolume, "overSoldBuy", SL, TP);
                        openSell(TradeType.Sell, Symbol, currentVolume, "overSoldSell", SL, TP);
                    }
                    break;
            }
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

        protected void phishingPhase()
        {
            foreach (var p in Positions)
            {
                if (p.Pips < 0)
                {
                    ClosePosition(p);
                }
            }
        }

        protected void crossPhase()
        {
            if (Symbol.Bid > sma.Result.LastValue + (SmaAmplitude / 10000))
            {
                //Print(" SMA Higher = " + sma100.Result.LastValue);
                if (smaValue.Equals("low"))
                {
                    //takeHedgePositionsWithConditions("<=", 0);
                    if (!phase.Equals("Pause"))
                    {
                        foreach (var p in Positions)
                        {
                            ClosePosition(p);
                        }

                        //openSell(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                        openBuy(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                    }

                }
                smaValue = "high";
            }
            else if (Symbol.Bid < sma.Result.LastValue - (SmaAmplitude / 10000))
            {
                //Print(" SMA Lower = " + sma100.Result.LastValue);
                if (smaValue.Equals("high"))
                {
                    //takeHedgePositionsWithConditions("<=", 0);
                    if (!phase.Equals("Pause"))
                    {
                        foreach (var p in Positions)
                        {
                            ClosePosition(p);
                        }
                        //openBuy(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                        openSell(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                    }

                }
                smaValue = "low";
            }

            //
            foreach (var p in Positions)
            {
                if (p.Pips > 915)
                {
                    phase = "phishing";
                }
            }
        }

        protected void indicator(string name)
        {

            switch (name)
            {
                case                 /*
                case "rsi":
                    if (rsi.Result.LastValue >= limitOverbuy)
                    {
                        //Print(" RSI touched 70)");
                        overbuy = true;

                    }
                    if (rsi.Result.LastValue < limitOverbuy && overbuy == true)
                    {
                        //Print("RSI crossed below " + limitOverbuy);
                        overbuy = false;

                        balance = Account.Balance;
                        takeHedgePositionsWithConditions("<=", 0);
                        //if (noActivePositions())
                        //{
                        //   openBuy(TradeType.Buy, Symbol, currentVolume, "overSoldBuy", SL, TP);

                        //  }
                    }
                    if (rsi.Result.LastValue > limitOversell && oversell == true)
                    {
                        //Print("RSI crossed above " + limitOversell);
                        oversell = false;

                        balance = Account.Balance;
                        takeHedgePositionsWithConditions("<=", 0);
                        // if (noActivePositions())
                        // {
                        //     openSell(TradeType.Sell, Symbol, currentVolume, "overSoldSell", SL, TP);
                        //  }

                    }
                    if (rsi.Result.LastValue < limitOversell)
                    {
                        //Print(" RSI touched 30");
                        oversell = true;

                    }

                    break;
*/"SMA":

                    if (phase.Equals("Cross"))
                    {
                        crossPhase();
                    }

                    if (phase.Equals("phishing"))
                    {
                        phishingPhase();
                    }
                    break;
            }

        }

        protected void openBuy(TradeType tradeType, Symbol symbol, int volume, string label, double SL, double TP)
        {
            var overSoldBuy = ExecuteMarketOrder(TradeType.Buy, symbol, volume, label, SL, TP);

        }

        protected void openSell(TradeType tradeType, Symbol symbol, int volume, string label, double SL, double TP)
        {
            var overSoldSell = ExecuteMarketOrder(TradeType.Sell, symbol, volume, label, SL, TP);

        }
    }
}

