using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
            String csvFile = @"C:\Users\leonardo.vitullo\Downloads\ETH.csv";
            BollingerBand bollingerBand = new BollingerBand();
            bollingerBand.Load(csvFile);
            BollingerBandSerie serie = bollingerBand.Calculate();

            RSI rsi = new RSI(14);
            rsi.Load(csvFile);
            RSISerie serieRSI = rsi.Calculate();

            List<Ohlc> ohlcList = new List<Ohlc>();
            ohlcList.Add(new Ohlc());

            chart1.Series.Clear();

            StreamReader r = new StreamReader(csvFile);
            String file = r.ReadToEnd();
            String[] pricesClose = file.Split(new String[] { "\r\n" , "\n"}, StringSplitOptions.RemoveEmptyEntries);
            chart1.Series.Add("Price").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            chart1.Series["Price"].Color = Color.Aqua;
            int j = 0;
            foreach (var s in pricesClose)
            {
                String[] pricesOscillation = s.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    double high = double.Parse(pricesOscillation[1]);
                    double low = double.Parse(pricesOscillation[2]);
                    double close = double.Parse(pricesOscillation[3]);
                    chart1.Series["Price"].Points.AddXY(j++, close);
                }
                catch( Exception err)
                {

                }
            }

            chart1.Series.Add("bUP").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("bDOWN").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("bMIDDLE").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add("RSI").ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            for (int i = 0; i < serie.BandWidth.Count; i++)
            {
                if (serie.UpperBand[i] != null)
                {
                    chart1.Series["bUP"].Points.AddXY(i, serie.UpperBand[i]);
                }

                if(serie.MidBand[i] != null)
                {
                    chart1.Series["bMIDDLE"].Points.AddXY(i, serie.MidBand[i]);
                }

                if(serie.LowerBand[i] != null)
                {
                    chart1.Series["bDOWN"].Points.AddXY(i, serie.LowerBand[i]);
                }

                if (serieRSI.RSI[i] != null)
                {
                    //chart1.Series["RSI"].Points.AddXY(i, serieRSI.RSI[i]);
                }
            }
        }
    }
}
