namespace DynamicQL.Attributes.Enums;

[Flags]
public enum QueryOptions
{
    None = 0,
    SingleQuery = 1,
    MultiQuery = 2,
    Query = 3,
    SingleCreate = 4,
    MultiCreate = 8,
    Create = 12,
    SingleUpdate = 16,
    MultiUpdate = 32,
    Update = 48,
    SingleDelete = 64,
    MultiDelete = 128,
    Delete = 192,
    SingleCrud = 85,
    MultiCrud = 170,
    Crud = 255,
    Subscribe = 256,
    All = 511
}