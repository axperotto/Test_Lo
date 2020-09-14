using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_Lo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chart1.Series.Clear();
            Int32 timeStampStart = (Int32)(DateTime.UtcNow.AddMonths(-12).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Int32 timeStampNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            String yahooAPI = $"https://query1.finance.yahoo.com/v7/finance/download/BTC-EUR?period1={timeStampStart}&period2={timeStampNow}&interval=1d&events=history";
            using (var client = new WebClient())
            {
                client.DownloadFile(yahooAPI, "fileLOG");
            }

            String csvFile = @"fileLOG";
            List<Ohlc> ohlcList = new List<Ohlc>();
            StreamReader r = new StreamReader(csvFile);
            String file = r.ReadToEnd();
            String[] pricesClose = file.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            chart1.Series.Add("Price").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
           // chart1.Series["Price"].Color = Color.Black;
            int j = 0;
            List<DateTime> dates = new List<DateTime>();
            foreach (var s in pricesClose)
            {
                String[] pricesOscillation = s.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    int index = 0;
                    DateTime date = DateTime.Parse(pricesOscillation[index++]);
                    double open = double.Parse(pricesOscillation[index++]);
                    double high = double.Parse(pricesOscillation[index++]);
                    double low = double.Parse(pricesOscillation[index++]);
                    double close = double.Parse(pricesOscillation[index++]);
                    double adjClose = double.Parse(pricesOscillation[index++]);
                    double volume = double.Parse(pricesOscillation[index++]);

                    Ohlc newOhlc = new Ohlc();
                    newOhlc.Open = open;
                    newOhlc.High = high;
                    newOhlc.Low = low;
                    newOhlc.Close = close;
                    newOhlc.AdjClose = adjClose;
                    newOhlc.Volume = volume;

                    ohlcList.Add(newOhlc);

                    dates.Add(date);
                    chart1.Series["Price"].Points.AddXY(date, close);
                }
                catch (Exception err)
                {

                }
            }

            
            BollingerBand bollingerBand = new BollingerBand();
            bollingerBand.Load(ohlcList);
            BollingerBandSerie serie = bollingerBand.Calculate();

            MACD macd = new MACD();
            macd.Load(ohlcList);
            MACDSerie macdserie = macd.Calculate();

            EMA ema = new EMA();
            ema.Load(ohlcList);
            SingleDoubleSerie singleDoubleSerie = ema.Calculate();


            RSI rsi = new RSI(14);
            rsi.Load(ohlcList);
            RSISerie serieRSI = rsi.Calculate();

            chart1.Series.Add("bUP").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("bDOWN").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("bMIDDLE").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("RSI").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("EMA").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("MACD").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("MACDHistogram").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            for (int i = 0; i < serie.BandWidth.Count; i++)
            {
                if (macdserie.MACDLine[i] != null)
                {
                    chart1.Series["MACD"].Points.AddXY(dates[i], macdserie.MACDLine[i]);
                }

                if (macdserie.MACDHistogram[i] != null)
                {
                    chart1.Series["MACDHistogram"].Points.AddXY(dates[i], macdserie.MACDHistogram[i]);
                }

                if (singleDoubleSerie.Values[i] != null)
                {
                    chart1.Series["EMA"].Points.AddXY(dates[i], singleDoubleSerie.Values[i]);
                }

                if (serie.UpperBand[i] != null)
                {
                    chart1.Series["bUP"].Points.AddXY(dates[i], serie.UpperBand[i]);
                }

                if(serie.MidBand[i] != null)
                {
                    chart1.Series["bMIDDLE"].Points.AddXY(dates[i], serie.MidBand[i]);
                }

                if(serie.LowerBand[i] != null)
                {
                    chart1.Series["bDOWN"].Points.AddXY(dates[i], serie.LowerBand[i]);
                }
                
                if (serieRSI.RSI[i] != null)
                {
                    chart1.Series["RSI"].Points.AddXY(dates[i], serieRSI.RSI[i]);
                }
            }
        }
    }
}
