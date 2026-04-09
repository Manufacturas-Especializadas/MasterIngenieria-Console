namespace Core.Entities;

public partial class Master
{
    public int Id { get; set; }

    public string? ParentPartNumber { get; set; }

    public string? ChildPartNumber { get; set; }

    public string? ExternalDiameter { get; set; }

    public string? WallThickness { get; set; }

    public string? Development { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public string? Family { get; set; }

    public string? Client { get; set; }

    public int? Line { get; set; }

    public string? PartOfPurchase { get; set; }

    public int? QuantityXquantity { get; set; }

    public string? Operation { get; set; }

    public int? Sequence { get; set; }

    public string? ProcessComments { get; set; }

    public string? MajorSetup { get; set; }

    public string? MinorSetup { get; set; }

    public decimal? OperSetup { get; set; }

    public decimal? TCiclo { get; set; }

    public decimal? Oper { get; set; }

    public int? PzsHr { get; set; }

    public string? Verification { get; set; }
}