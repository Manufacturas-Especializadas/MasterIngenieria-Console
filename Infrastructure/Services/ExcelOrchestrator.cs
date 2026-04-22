using Application.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ExcelOrchestrator
    {
        private readonly IExcelReader _excelReader;
        private readonly AppDbContext _context;

        public ExcelOrchestrator(IExcelReader excelReader, AppDbContext context)
        {
            _excelReader = excelReader;
            _context = context;
        }

        public async Task RunAsync(string filePath)
        {
            var rawExcelData = _excelReader.ReadExcel(filePath).ToList();

            var excelData = rawExcelData
                .Where(e => !string.IsNullOrWhiteSpace(e.ParentPartNumber) || !string.IsNullOrWhiteSpace(e.ChildPartNumber))
                .GroupBy(e => $"{e.ParentPartNumber?.Trim() ?? ""}|{e.ChildPartNumber?.Trim() ?? ""}|{e.Operation?.Trim() ?? ""}|{e.Sequence}")
                .Select(grupo => grupo.OrderBy(x => x.TCiclo ?? decimal.MaxValue).First())
                .ToList();

            var dbData = _context.Masters
                .AsTracking()
                .ToLookup(m => $"{m.ParentPartNumber?.Trim() ?? ""}|{m.ChildPartNumber?.Trim() ?? ""}|{m.Operation?.Trim() ?? ""}|{m.Sequence}");

            var existingImprovements = _context.MasterImprovements
                .AsNoTracking()
                .Select(i => new { i.ParentPartNumber, i.Line, i.OldCycleTime, i.NewCycleTime })
                .ToHashSet();

            var newRecords = new List<Master>();
            var recordsToUpdate = new List<Master>();
            var improvements = new List<MasterImprovement>();
            var updatedRecordsCount = 0;
            var parentsWithImprovement = new HashSet<string>();

            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
            DateTime nowInMexico = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoTimeZone);

            foreach (var excelRow in excelData)
            {
                if (string.IsNullOrWhiteSpace(excelRow.ParentPartNumber) && string.IsNullOrWhiteSpace(excelRow.ChildPartNumber))
                    continue;

                var key = $"{excelRow.ParentPartNumber?.Trim() ?? ""}|{excelRow.ChildPartNumber?.Trim() ?? ""}|{excelRow.Operation?.Trim() ?? ""}|{excelRow.Sequence}";
                var existingRecords = dbData[key];

                if (!existingRecords.Any())
                {
                    newRecords.Add(MapToEntity(excelRow));
                }
                else
                {
                    foreach (var existingRecord in existingRecords)
                    {
                        bool hasGeneralChange = HasChanged(existingRecord, excelRow);
                        bool hasTimeImprovement = false;

                        if (excelRow.TCiclo.HasValue && existingRecord.TCiclo.HasValue)
                        {
                            var excelRounded = Math.Round(excelRow.TCiclo.Value, 3);
                            var dbRounded = Math.Round(existingRecord.TCiclo.Value, 3);
                            decimal minimumTolerance = 0.1m;

                            if (excelRounded < dbRounded && (dbRounded - excelRounded) >= minimumTolerance)
                            {
                                var parentKey = excelRow.ParentPartNumber?.Trim() ?? "N/A";
                                var lineKey = existingRecord.Line ?? 0;
                                var lockKey = $"{parentKey}-{lineKey}";

                                if (!parentsWithImprovement.Contains(lockKey) && parentKey != "N/A")
                                {
                                    var improvement = new MasterImprovement
                                    {
                                        ParentPartNumber = parentKey,
                                        Line = lineKey,
                                        OldCycleTime = dbRounded,
                                        NewCycleTime = excelRounded,
                                        ImprovementDate = nowInMexico,
                                        Process = existingRecord.Operation ?? "N/A",
                                        Description = "Optimización detectada en carga de Excel"
                                    };

                                    bool alreadyExists = existingImprovements.Any(e =>
                                        e.ParentPartNumber == improvement.ParentPartNumber &&
                                        e.Line == improvement.Line &&
                                        e.OldCycleTime == improvement.OldCycleTime &&
                                        e.NewCycleTime == improvement.NewCycleTime);

                                    if (!alreadyExists)
                                    {
                                        improvements.Add(improvement);
                                        parentsWithImprovement.Add(lockKey);
                                        hasTimeImprovement = true;
                                    }
                                }
                            }
                        }

                        if (hasGeneralChange || hasTimeImprovement)
                        {
                            var updatedRecord = existingRecord;
                            UpdateEntity(updatedRecord, excelRow);
                            recordsToUpdate.Add(updatedRecord);
                            updatedRecordsCount++;
                        }
                    }
                }
            }

            if (newRecords.Any())
            {
                await _context.Masters.AddRangeAsync(newRecords);
                Console.WriteLine($"[NEW] {newRecords.Count} new records added.");
            }

            if (recordsToUpdate.Any())
            {
                _context.Masters.UpdateRange(recordsToUpdate);
                Console.WriteLine($"[UPDATED] {recordsToUpdate.Count} records marked for update.");
            }

            if (improvements.Any())
            {
                await _context.MasterImprovements.AddRangeAsync(improvements);
                Console.WriteLine($"[IMPROVEMENTS] {improvements.Count} unique cycle time optimizations detected.");
            }

            if (updatedRecordsCount > 0 || newRecords.Any() || improvements.Any())
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"[OK] Process completed successfully. {updatedRecordsCount} records updated in total.");
            }
            else
            {
                Console.WriteLine(">>> System is already up to date. No changes detected.");
            }
        }

        private bool HasChanged(Master db, ExcelRowDto excel)
        {
            return db.ExternalDiameter != excel.ExternalDiameter ||
                   db.WallThickness != excel.WallThickness ||
                   db.Development != excel.Development ||
                   db.Description != excel.Description ||
                   db.Type != excel.Type ||
                   db.Family != excel.Family ||
                   db.Client != excel.Client ||
                   db.Line != excel.Line ||
                   db.PartOfPurchase != excel.PartOfPurchase ||
                   db.QuantityXquantity != excel.QuantityXquantity ||
                   db.Operation != excel.Operation ||
                   db.Sequence != excel.Sequence ||
                   db.ProcessComments != excel.ProcessComments ||
                   db.MajorSetup != excel.MajorSetup ||
                   db.MinorSetup != excel.MinorSetup ||
                   Math.Round(db.OperSetup ?? 0, 3) != Math.Round(excel.OperSetup ?? 0, 3) ||
                   Math.Round(db.TCiclo ?? 0, 3) != Math.Round(excel.TCiclo ?? 0, 3) ||
                   Math.Round(db.Oper ?? 0, 3) != Math.Round(excel.Oper ?? 0, 3) ||
                   db.PzsHr != excel.PzsHr ||
                   db.Verification != excel.Verification;
        }

        private void UpdateEntity(Master db, ExcelRowDto excel)
        {
            db.ExternalDiameter = excel.ExternalDiameter;
            db.WallThickness = excel.WallThickness;
            db.Development = excel.Development;
            db.Description = excel.Description;
            db.Type = excel.Type;
            db.Family = excel.Family;
            db.Client = excel.Client;
            db.Line = excel.Line;
            db.PartOfPurchase = excel.PartOfPurchase;
            db.QuantityXquantity = excel.QuantityXquantity;
            db.Operation = excel.Operation;
            db.Sequence = excel.Sequence;
            db.ProcessComments = excel.ProcessComments;
            db.MajorSetup = excel.MajorSetup;
            db.MinorSetup = excel.MinorSetup;
            db.OperSetup = excel.OperSetup;
            db.TCiclo = excel.TCiclo;
            db.Oper = excel.Oper;
            db.PzsHr = excel.PzsHr;
            db.Verification = excel.Verification;
        }

        private Master MapToEntity(ExcelRowDto excel)
        {
            return new Master
            {
                ParentPartNumber = excel.ParentPartNumber,
                ChildPartNumber = excel.ChildPartNumber,
                ExternalDiameter = excel.ExternalDiameter,
                WallThickness = excel.WallThickness,
                Development = excel.Development,
                Description = excel.Description,
                Type = excel.Type,
                Family = excel.Family,
                Client = excel.Client,
                Line = excel.Line,
                PartOfPurchase = excel.PartOfPurchase,
                QuantityXquantity = excel.QuantityXquantity,
                Operation = excel.Operation,
                Sequence = excel.Sequence,
                ProcessComments = excel.ProcessComments,
                MajorSetup = excel.MajorSetup,
                MinorSetup = excel.MinorSetup,
                OperSetup = excel.OperSetup,
                TCiclo = excel.TCiclo,
                Oper = excel.Oper,
                PzsHr = excel.PzsHr,
                Verification = excel.Verification
            };
        }
    }
}