// HammyBot Sharp - HammyBot Sharp
//     Copyright (C) 2021 Thomas Duckworth <tduck973564@gmail.com>
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace HammyBot_Sharp
{
    public static class Covid19Service
    {
        private const string LgaCsv          = "covidCasesLga.csv";
        private const string PostcodeCsv     = "covidCasesPostcode.csv";
        public  const string ModellingImage  = "covidModelling.png";
        public static void StartHourlyDownloadService()
        {
            new Thread(() => HourlyDownloadService()).Start();
        }

        private static void HourlyDownloadService()
        {
            for (; ; )
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile("https://docs.google.com/spreadsheets/d/e/2PACX-1vQ9oKYNQhJ6v85dQ9qsybfMfc-eaJ9oKVDZKx-VGUr6szNoTbvsLTzpEaJ3oW_LZTklZbz70hDBUt-d/pub?gid=0&single=true&output=csv", LgaCsv);
                    webClient.DownloadFile("https://docs.google.com/spreadsheets/d/e/2PACX-1vTwXSqlP56q78lZKxc092o6UuIyi7VqOIQj6RM4QmlVPgtJZfbgzv0a3X7wQQkhNu8MFolhVwMy4VnF/pub?gid=0&single=true&output=csv", PostcodeCsv);
                    webClient.DownloadFile("https://github.com/chrisjbillington/chrisjbillington.github.io/blob/master/COVID_VIC_2021_vax_linear.png?raw=true", ModellingImage);
                }
                Thread.Sleep(3600000); // 1 hour
            }
        }
        
        public static int GetActiveCases()
        {
            var reader = new StreamReader(LgaCsv);
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<Covid19Lga>();

            return records.Select(x => x.Active).Sum();
        }

        public static int GetNewCases()
        {
            var reader = new StreamReader(LgaCsv);
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<Covid19Lga>();

            return records.Select(x => x.New).Sum();
        }

        public static int GetNewCasesPostcode(int postcode)
        {
            var reader = new StreamReader(PostcodeCsv);
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<Covid19Postcode>();

            return records.Where(x => x.Postcode == postcode).ToList().Select(x => x.New).Sum();
        }

        public static int GetActiveCasesPostcode(int postcode)
        {
            var reader = new StreamReader(PostcodeCsv);
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<Covid19Postcode>();

            return records.Where(x => x.Postcode == postcode).ToList().Select(x => x.Active).Sum();
        }
    }

    class Covid19Lga
    {
        [Name("LGA")]
        public string? Lga { get; set; }
        [Name("lga_pid")]
        public string? LgaPid { get; set; }
        [Name("population")]
        public string? Population { get; set; }
        [Name("active")]
        public int Active { get; set; }
        [Name("cases")]
        public int Cases { get; set; }
        [Name("rate")]
        public decimal Rate { get; set; }
        [Name("new")]
        public int New { get; set; }
        [Name("band")]
        public string? Band { get; set; }
        [Name("LGADisplay")]
        public string? LgaDisplay { get; set; }
        [Name("data_date")]
        public string? DataDate { get; set; }
        [Name("file_processed_date")]
        public string? FileProcessedDate { get; set; }
    }

    class Covid19Postcode
    {
        [Name("postcode")]
        public int Postcode { get; set; }
        [Name("population")]
        public string? Population { get; set; }
        [Name("active")]
        public int Active { get; set; }
        [Name("cases")]
        public int Cases { get; set; }
        [Name("rate")]
        public decimal Rate { get; set; }
        [Name("new")]
        public int New { get; set; }
        [Name("band")]
        public string? Band { get; set; }
        [Name("data_date")]
        public string? DataDate { get; set; }
        [Name("file_processed_date")]
        public string? FileProcessedDate { get; set; }
    }
}
