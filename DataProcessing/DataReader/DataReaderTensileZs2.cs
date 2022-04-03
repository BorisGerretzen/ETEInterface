using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Zs2Decode;

namespace DataProcessing.DataReader
{
    internal class DataReaderTensileZs2 : AbstractDataReader
    {
        private readonly List<double> specimenThickness = new();
        private readonly List<double> specimenWidth = new();

        public DataReaderTensileZs2(string fileName) : base(fileName) { }

        public override List<List<(double, double)>> ReadData(string sheetName = "") {
            var returnList = new List<List<(double, double)>>();

            var decoder = new Zs2Decoder(FileName);
            var rootChunk = decoder.ReadData();

            // Get the width and thickness from each sample
            var seriesChunk = rootChunk.Navigate("/Body/batch/Series/SeriesElements");
            foreach (var series in seriesChunk.ListElements) {
                Chunk valParThickness = series.Navigate("EvalContext/ParamContext/ParameterListe/Elem0/QS_ValPar");
                string array = valParThickness.Value;
                var thickness = double.Parse(array.Split(", ")[1]);
                specimenThickness.Add(thickness);

                Chunk valParWidth = series.Navigate("EvalContext/ParamContext/ParameterListe/Elem1/QS_ValPar");
                array = valParWidth.Value;
                var width = double.Parse(array.Split(", ")[1]);
                specimenWidth.Add(width);
            }


            // Get sensors
            var sensorLoadCell = rootChunk.Sensors.First(sensor => sensor.Id == 40402);
            var sensorExtensometer = rootChunk.Sensors.First(sensor => sensor.Id == 40403);

            // Loop over all samples and calculate stuff we want
            for (int sampleIndex = 0; sampleIndex < sensorLoadCell.Values.Count; sampleIndex++) {
                var strainList = ExtensometerToStrain(sensorExtensometer.Values[sampleIndex]);
                var stressList = LoadCellToStress(sensorLoadCell.Values[sampleIndex], sampleIndex);

                returnList.Add(strainList.Zip(stressList, (d, d1) => (d, d1)).ToList());
            }

            return returnList;
        }

        private List<double> ExtensometerToStrain(List<string> values) {
            return values.Select(val => double.Parse(val) / 20).ToList(); 
        }

        private List<double> LoadCellToStress(List<string> values, int sampleIndex) {
            return values.Select(val => double.Parse(val) / (specimenThickness[sampleIndex]*specimenWidth[sampleIndex])).ToList();
        }
    }
}
