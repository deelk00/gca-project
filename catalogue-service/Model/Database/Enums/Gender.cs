namespace CatalogueService.Model.Database.Enums
{
    [Flags]
    public enum Gender
    {
        Other = 0,
        Uni = 1,
        Male = 2,
        Female = 4,
        MaleCompatible = 3,
        FemaleCompatible = 5
    }
}
