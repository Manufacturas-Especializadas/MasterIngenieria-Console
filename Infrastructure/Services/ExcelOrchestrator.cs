using Application.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

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
            var excelData = _excelReader.ReadExcel(filePath).ToList();

            var dbData = _context.Masters
                .AsTracking()
                .ToLookup(m => $"{m.ParentPartNumber}|{m.ChildPartNumber}");

            var newRecords = new List<Master>();
            var updatedRecordsCount = 0;

            foreach (var excelRow in excelData)
            {
                var key = $"{excelRow.ParentPartNumber}|{excelRow.ChildPartNumber}";

                var existingRecords = dbData[key];

                if (!existingRecords.Any())
                {
                    newRecords.Add(MapToEntity(excelRow));
                }
                else
                {
                    foreach (var existingRecord in existingRecords)
                    {
                        if (HasChanged(existingRecord, excelRow))
                        {
                            Console.WriteLine($"Actualizando Id: {existingRecord.Id}");
                            UpdateEntity(existingRecord, excelRow);
                            updatedRecordsCount++;
                        }
                    }
                }

            }

            if (newRecords.Any())
            {
                await _context.Masters.AddRangeAsync(newRecords);
                Console.WriteLine($"[NUEVOS] {newRecords.Count} registros detectados.");
            }

            if (updatedRecordsCount > 0 || newRecords.Any())
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"[CAMBIOS] {updatedRecordsCount} registros actualizados en la base de datos.");
            }
            else
            {
                Console.WriteLine(">>> El sistema ya está actualizado. No se detectaron cambios.");
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
                   db.OperSetup != excel.OperSetup ||
                   db.TCiclo != excel.TCiclo ||
                   db.Oper != excel.Oper ||
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