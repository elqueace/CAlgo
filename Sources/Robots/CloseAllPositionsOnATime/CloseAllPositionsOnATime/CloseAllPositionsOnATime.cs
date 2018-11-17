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

            //double tail;
            foreach (var position in Positions)
            {

                //ClosePosition(position);

            }

            //Accessing historical O-H-L-C prices from Robots
            // index = MarketSeries.Close.Count - 2;
            //close = MarketSeries.Close[index];
            //high = MarketSeries.High[index];
            //low = MarketSeries.Low[index];
            // open = MarketSeries.Open[index];

        }
        /*           
           if (Server.Time.Hour == 12 || Server.Time.Hour == 12 || Server.Time.Hour == 12 && counterPosition == 0)
            {

                //if previous candle is green
                if (close - open > 0)
                {
                    tail = high - close;
                    if (tail < 5)
                    {
                        Print("Green motherfucker" + "open : " + open + " - high : " + high + " - low : " + low + " - close : " + close + " - Tail : " + tail);

                        ExecuteMarketOrder(TradeType.Sell, Symbol, 10, "Sell", SL, TP);
                    }
                }
                //if previous candle is red 
                if (close - open < 0)
                {
                    tail = close - low;
                    if (tail < 5)
                    {
                        Print("Red motherfucker" + "open : " + open + " - high : " + high + " - low : " + low + " - close : " + close + " - Tail : " + tail);
                        ExecuteMarketOrder(TradeType.Buy, Symbol, 10, "Buy", SL, TP);
                    }

                }
            }
            */
        protected override void OnTimer()
        {
            counterPosition = 0;
            foreach (var position in Positions)
            {
                counterPosition++;
            }



            if (counterPosition <= 0 && counterPosition <= 2)
            {
                if (rsiMin10.Result.LastValue > 70 && rsi.Result.LastValue > 70)
                {
                    Print("Both RSI above 70)");
                    oneShot = false;
                }
                else if (rsi.Result.LastValue < 70 && rsi.Result.LastValue > 30 && oneShot == false && rsi.Result.LastValue > 60)
                {
                    Print("RSI Between 70 and 30");
                    ExecuteMarketOrder(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                    ExecuteMarketOrder(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                    oneShot = true;
                }
                else if (rsi.Result.LastValue < 70 && rsi.Result.LastValue > 30 && oneShot == false && rsi.Result.LastValue < 40)
                {
                    Print("RSI Between 70 and 30");
                    ExecuteMarketOrder(TradeType.Buy, Symbol, currentVolume, "Buy", SL, TP);
                    ExecuteMarketOrder(TradeType.Sell, Symbol, currentVolume, "Sell", SL, TP);
                    oneShot = true;
                }
                else if (rsiMin10.Result.LastValue < 30 && rsi.Result.LastValue < 30)
                {
                    Print(" Both RSI below 30");
                    oneShot = false;
                }
            }
            if (Server.Time.Hour == 12)
            {

                // Print("Its 12PM");
            }
            if (Server.Time.Hour == 15)
            {

                ////// Print("Its 15PM");
            }

            if (Server.Time.Hour == 18)
            {

                //Print("Its 18PM");
            }
            Timer.Start(60);
        }
        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            counterLoss++;
            var position = args.Position;

            if (position.GrossProfit > 0)
            {

                currentVolume = InitialVolume;
            }
            else
            {
                if (counterLoss % 2 == 0)
                {
                    switch (currentVolume)
                    {
                        case 1000:
                            currentVolume = 2000;
                            break;
                        case 2000:
                            currentVolume = 4000;
                            break;
                        case 4000:
                            currentVolume = 8000;
                            break;
                        case 8000:
                            currentVolume = 16000;
                            break;
                        case 16000:
                            currentVolume = 32000;
                            break;
                        case 32000:
                            currentVolume = 64000;
                            break;
                        case 64000:
                            currentVolume = 128000;
                            break;
                        case 128000:
                            currentVolume = 256000;
                            break;
                        case 256000:
                            currentVolume = 512000;
                            break;
                        //10eme MISE
                        case 512000:
                            currentVolume = 1024000;
                            break;
                        case 1024000:
                            currentVolume = 2048000;
                            break;
                        case 2048000:
                            currentVolume = 4096000;
                            break;
                        case 4096000:
                            currentVolume = 8192000;
                            break;
                        case 8192000:
                            currentVolume = 16394000;
                            break;
                        case 16394000:
                            currentVolume = 90000;
                            break;

                        case 90000:
                            currentVolume = 130000;
                            break;
                        case 130000:
                            currentVolume = 190000;
                            break;
                        case 190000:
                            currentVolume = 350000;
                            break;
                        case 350000:
                            currentVolume = 500000;
                            break;
                        //15eme MISE
                        case 309000:
                            currentVolume = 36000;
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
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
