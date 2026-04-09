using Application.Dtos;
using ClosedXML.Excel;
using Core.Interfaces;
using Infrastructure.Common.Extension;

namespace Infrastructure.Services
{
    public class ClosedXmlReader : IExcelReader
    {
        public IEnumerable<ExcelRowDto> ReadExcel(string filePath)
        {
            var rows = new List<ExcelRowDto>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var range = worksheet.RangeUsed();

                foreach(var row in range!.RowsUsed().Skip(2))
                {
                    rows.Add(new ExcelRowDto
                    {
                        ParentPartNumber = row.Cell(1).GetValueOrEmptyString(),
                        ChildPartNumber = row.Cell(2).GetValueOrEmptyString(),
                        ExternalDiameter = row.Cell(5).GetValueOrEmptyString(),
                        WallThickness = row.Cell(6).GetValueOrEmptyString(),
                        Development = row.Cell(7).GetValueOrEmptyString(),
                        Description = row.Cell(8).GetValueOrEmptyString(),
                        Type = row.Cell(9).GetValueOrEmptyString(),
                        Family = row.Cell(10).GetValueOrEmptyString(),
                        Client = row.Cell(11).GetValueOrEmptyString(),
                        Line = row.Cell(12).GetValueOrNullInt(),
                        PartOfPurchase = row.Cell(13).GetValueOrEmptyString(),
                        QuantityXquantity = row.Cell(14).GetValueOrNullInt(),
                        Operation = row.Cell(15).GetValueOrEmptyString(),
                        Sequence = row.Cell(16).GetValueOrNullInt(),
                        ProcessComments = row.Cell(17).GetValueOrEmptyString(),
                        MajorSetup = row.Cell(18).GetValueOrEmptyString(),
                        MinorSetup = row.Cell(19).GetValueOrEmptyString(),
                        OperSetup = row.Cell(20).GetValueOrNullDecimal(),
                        TCiclo = row.Cell(21).GetValueOrNullDecimal(),
                        Oper = row.Cell(22).GetValueOrNullDecimal(),
                        PzsHr = row.Cell(23).GetValueOrNullInt(),
                        Verification = row.Cell(24).GetValueOrEmptyString(),
                    });
                }
            }

            return rows;
        }
    }
}
