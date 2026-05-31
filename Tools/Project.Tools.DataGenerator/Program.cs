using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Shared.Interfaces;

int contactCount = args.Length > 0 && int.TryParse(args[0], out var n) && n > 0 ? n : 50;
Console.WriteLine($"Seeding {contactCount} contacts...");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.RegisterDataLayer(ctx.Configuration);
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

// ── Custom field definitions (10 fixed fields for Contact) ───────────────────

var definitions = new List<CustomFieldDefinition>
{
    MakeDef("Company",           CustomFieldTypeE.Text,    0, required: true),
    MakeDef("Industry",          CustomFieldTypeE.Text,    1),
    MakeDef("Annual Revenue",    CustomFieldTypeE.Number,  2),
    MakeDef("Employee Count",    CustomFieldTypeE.Number,  3),
    MakeDef("Founded Date",      CustomFieldTypeE.Date,    4),
    MakeDef("Is Active",         CustomFieldTypeE.Boolean, 5, required: true),
    MakeDef("Website",           CustomFieldTypeE.Text,    6),
    MakeDef("Notes",             CustomFieldTypeE.Text,    7),
    MakeDef("Lead Score",        CustomFieldTypeE.Number,  8),
    MakeDef("Last Contact Date", CustomFieldTypeE.Date,    9),
};

db.CustomFieldDefinitions.AddRange(definitions);
await db.SaveChangesAsync(CancellationToken.None)
        .Finally(
        _ => Console.WriteLine($"  + {definitions.Count} custom field definitions"),
        err => { Console.Error.WriteLine($"Error saving definitions: [{err.Code}] {err.Message}"); Environment.Exit(1); }
        );

// ── Contacts ─────────────────────────────────────────────────────────────────

var faker = new Faker("en_GB");

var contacts = Enumerable.Range(0, contactCount).Select(_ => {
    string firstName = faker.Name.FirstName();
    string lastName = faker.Name.LastName();
    var contact = new Contact
    {
        FirstName = firstName,
        LastName  = lastName,
        Email     = faker.Internet.Email(firstName, lastName, "example.test"),
        Phone     = faker.Phone.PhoneNumber(),
    };

    foreach (var def in definitions)
    {
        // Always include required fields; skip ~30 % of optional ones
        if (!def.IsRequired && faker.Random.Bool(0.3f))
            continue;

        contact.CustomFieldValues.Add(new CustomFieldValue
        {
            CustomFieldDefinitionId = def.Id,
            Value = GenerateValue(faker, def.FieldType),
        });
    }

    return contact;
}).ToList();

db.Contacts.AddRange(contacts);
await db.SaveChangesAsync(CancellationToken.None).Finally(
    _ => Console.WriteLine($"  + {contactCount} contacts with custom field values"),
    err => Console.Error.WriteLine($"Error saving contacts: [{err.Code}] {err.Message}")
);

Console.WriteLine("Done.");

// ── Helpers ───────────────────────────────────────────────────────────────────

static CustomFieldDefinition MakeDef(string name, CustomFieldTypeE type, int order, bool required = false) =>
    new() { Name = name, FieldType = type, EntityType = "Contact", IsRequired = required, SortOrder = order };

static string GenerateValue(Faker faker, CustomFieldTypeE type) => type switch
{
    CustomFieldTypeE.Text    => faker.Lorem.Sentence(3),
    CustomFieldTypeE.Number  => faker.Random.Int(1, 1_000_000).ToString(),
    CustomFieldTypeE.Date    => faker.Date.Past(10).ToString("yyyy-MM-dd"),
    CustomFieldTypeE.Boolean => faker.Random.Bool().ToString().ToLower(),
    _                        => faker.Lorem.Word(),
};
