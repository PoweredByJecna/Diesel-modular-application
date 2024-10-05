


using ClosedXML.Excel;
using Diesel_modular_application.Models;

public class ExcelHandling
    {

        public List<TableLokality> ParseExcelFile(Stream stream)
        {
            var lokalityies = new List<TableLokality>();

            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var lokality = new TableLokality
                    {
                        Lokalita = row.Cell(1).GetValue<string>(),
                        Klasifikace = row.Cell(2).GetValue<string>(),
                        Adresa = row.Cell(3).GetValue<string>(),
                        Baterie = row.Cell(4).GetValue<string>(),
                        DA = row.Cell(5).GetValue<string>(),
                        Zásuvka = row.Cell(6).GetValue<string>()

                    };
                    lokalityies.Add(lokality);
                }
            }

            return lokalityies;
        }
    }