using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class drawline : Robot
    {
        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }

        protected override void OnStart()
        {

            var name = "myObject";
            var low = MarketSeries.Low.Count - 10;
            var text = low.ToString();
            var xPos = MarketSeries.Low.Count;
            var yPos = low;
            var vAlign = VerticalAlignment.Bottom;
            var hAlign = HorizontalAlignment.Right;
            ChartObjects.DrawText(name, text, xPos, yPos, vAlign, hAlign, Colors.Red);

        }

        protected override void OnTick()
        {
            // Put your core logic here
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
