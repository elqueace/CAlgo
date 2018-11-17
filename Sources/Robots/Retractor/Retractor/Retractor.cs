using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Retractor : Robot
    {
        [Parameter(DefaultValue = 2000)]
        public int InitialVolume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 10)]
        public int SL { get; set; }

        [Parameter("Take Profit", DefaultValue = 120)]
        public int TP { get; set; }

        [Parameter(DefaultValue = 1)]
        public int increaseAfter { get; set; }


        [Parameter("IF profit > ", DefaultValue = 80)]
        public int trailTrigger { get; set; }

        [Parameter("THEN trail at ", DefaultValue = 100)]
        public int TrailingStop { get; set; }


        [Parameter(DefaultValue = 70)]
        public int limitOverbuy { get; set; }

        [Parameter(DefaultValue = 30)]
        public int limitOversell { get; set; }

        private Random random = new Random();
        private int currentVolume, counterLoss = 0;
        //private double balance = 0;
        //private bool overbuy, oversell = false;
        public string state = "WaitingCross", smaValue = "";
        private int Today;
        private bool OneShot = true;
        private int day, currentDay;
        private double PipsLossBag = 0;
        //private RelativeStrengthIndex rsiMin10;
        //private RelativeStrengthIndex rsi;
        private SimpleMovingAverage sma;
        protected override void OnStart()
        {
            Today = Server.Time.Day;
            // rsi = Indicators.RelativeStrengthIndex(MarketData.GetSeries(TimeFrame.Hour).Close, 14);
            sma = Indicators.SimpleMovingAverage(MarketData.GetSeries(TimeFrame.Minute5).Close, 500);

            Positions.Closed += OnPositionsClosed;

            currentVolume = InitialVolume;

        }

        protected override void OnBar()
        {
            CheckNewDay();
            // Print(" distance SMA  = " + (Symbol.Bid - (sma.Result.LastValue - 0.001) + " sma.Result.LastValue  = " + (sma.Result.LastValue - 0.001) + " Symbol   = " + Symbol.Bid));
            //Print("day =" + day + "currentday =" + currentDay);
        }
        protected override void OnTimer()
        {

        }

        protected override void OnTick()
        {
//            Print(" SMA  = " + sma.Result.LastValue + " Symbol   = " + Symbol.Bid);

            //Method to check market pair's spread
            //CheckPairSpread();

            //change state to cross when market cross SMA
            CheckCross();

            if (noActivePositions() && state.Equals("Crossed") && OneShot)
            {
                if (CheckDistanceFromSMA().Equals("AboveTarget"))
                {
                    //take a buy
                    openBuy(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                    openSell(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                    OneShot = false;
                    Print(OneShot);
                }
                if (CheckDistanceFromSMA().Equals("BelowTarget"))
                {
                    //take a sell
                    openSell(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                    openBuy(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                    OneShot = false;
                    Print(OneShot);
                }
            }
        }

        //
        private void CheckNewDay()
        {
            if (Server.Time.Day != Today)
            {
                OneShot = true;
                Print(OneShot);
                Today = Server.Time.Day;
            }
        }

        //Close position whn market touches the SMA
        private void CloseWhenCrossSMA()
        {
            foreach (var p in Positions)
            {
                //if market cross SMA by a crtain target price and there is an opened Sell position
                if (CheckDistanceFromSMA().Equals("AboveTarget") && p.TradeType == TradeType.Sell)
                {
                    //if position is won
                    if (p.Pips > 0 || p.NetProfit > 0)
                    {
                        currentVolume = InitialVolume;
                        counterLoss = 0;
                        PipsLossBag = 0;
                        Print("piplossbag win = " + PipsLossBag);
                    }
                    //if position is lost
                    if (p.Pips < 0)
                    {
                        //put the loss in the loss bag
                        PipsLossBag += p.Pips;
                        Print("piplossbag Loss = " + PipsLossBag);
                        if (PipsLossBag <= -20)
                        {
                            //increment the loss counter
                            counterLoss++;
                            //update  next positions volume
                            currentVolume *= 2;
                            //reinitialize lossBag for next position
                            PipsLossBag = 0;
                            Print("piplossbag Loss -20 = " + PipsLossBag);
                        }
                    }
                    //change bot state to wit for another cross SMA
                    state = "WaitingCross";
                    //Close the position
                    ClosePosition(p);
                }

                if (CheckDistanceFromSMA().Equals("BelowTarget") && p.TradeType == TradeType.Buy)
                {
                    if (p.Pips > 0)
                    {
                        currentVolume = InitialVolume;
                        counterLoss = 0;
                        PipsLossBag = 0;
                        Print("piplossbag win = " + PipsLossBag);
                    }
                    //position is lost
                    if (p.Pips < 0)
                    {
                        //put the loss in the loss bag
                        PipsLossBag += p.Pips;
                        Print("piplossbag Loss = " + PipsLossBag);
                        if (PipsLossBag <= -20)
                        {
                            //increment the loss counter
                            counterLoss++;
                            //update  next positions volume
                            currentVolume *= 2;
                            PipsLossBag = 0;
                            Print("piplossbag Loss -20 = " + PipsLossBag);
                        }
                    }
                    ClosePosition(p);
                }
            }
        }

        //TESTED
        private void CheckCross()
        {
            if (Symbol.Bid - sma.Result.LastValue > -0.0001 && Symbol.Bid - sma.Result.LastValue < 0.0001)
            {
                state = "Crossed";
            }
        }

        //TESTED
        private string CheckDistanceFromSMA()
        {
            if (Symbol.Bid - (sma.Result.LastValue + 0.0) > 0)
            {
                return "AboveTarget";
            }
            if (Symbol.Bid - (sma.Result.LastValue - 0.0) < 0)
            {
                return "BelowTarget";
            }

            return "--";

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

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            OneShot = false;
            state = "WaitingCross";
            //Print("Closed");
            var position = args.Position;
            if (position.Pips > 0 || position.NetProfit > 0)
            {
                currentVolume = InitialVolume;
                counterLoss = 0;

                Print("piplossbag win = " + PipsLossBag);
            }
            else
            {
                //increment the loss counter
                counterLoss++;

                if (counterLoss % increaseAfter == 0)
                {
                    //updateVolumePositionsManual();
                    currentVolume *= 2;

                }
            }
        }
//
        protected void dealWithPositiveClosedPositions()
        {

        }
        //mis a 1 pos autorise
        protected bool noActivePositions()
        {
            int counterPosition = 0;

            foreach (var p in Positions)
            {
                counterPosition++;
            }

            if (counterPosition <= 1)
            {
                return true;
            }
            else
            {
                return false;
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
                    currentVolume = 5000;
                    break;
                case 5000:
                    currentVolume = 9000;
                    break;
                case 9000:
                    currentVolume = 16000;
                    break;
                case 16000:
                    currentVolume = 28000;
                    break;
                case 28000:
                    currentVolume = 49000;
                    break;
                case 49000:
                    currentVolume = 86000;
                    break;
                case 86000:
                    currentVolume = 150000;
                    break;
                //10eme MISE
                case 150000:
                    currentVolume = 263000;
                    break;
                case 263000:
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

        protected void openBuy(TradeType tradeType, Symbol symbol, int volume, string label, double SL, double TP)
        {
            //Print("Take Buy");
            var overSoldBuy = ExecuteMarketOrder(TradeType.Buy, symbol, volume, label, SL, TP);
        }

        protected void openSell(TradeType tradeType, Symbol symbol, int volume, string label, double SL, double TP)
        {
            //Print("Take Sell");
            var overSoldSell = ExecuteMarketOrder(TradeType.Sell, symbol, volume, label, SL, TP);
        }
    }
}

