﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
    internal abstract class AbstractDataReader {
        protected string FileName;
        protected string SheetName;

        protected AbstractDataReader(string fileName, string sheetName) {
            FileName = fileName;
            SheetName = sheetName;
        }

        protected AbstractDataReader(string fileName) {
            FileName = fileName;
        }

        public abstract List<List<(double, double)>> ReadData(string sheetName = "");
    }
}
