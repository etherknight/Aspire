using Bogus;
using Project.Core.DataLayer.Entities;

namespace Project.Tools.DataGenerator.Fakes;

public class ContactFaker : Faker<Contact> {
    
    public ContactFaker() 
        : base("en_GB") {
        UseSeed(307051200);
        RuleFor(contact => contact.FirstName, faker => faker.Name.FirstName());
        RuleFor(contact => contact.LastName, f => f.Name.LastName());
        RuleFor(contact => contact.Phone, faker => faker.Phone.PhoneNumber());
        FinishWith((faker, contact) => {
            contact.Email = faker.Internet.Email(contact.FirstName, contact.LastName, "example.test");
            contact.CustomFieldValues = new ContactCustomFieldValueFaker().Generate(10);
        } );
    }
};

public class ContactCustomFieldValueFaker : Faker<CustomFieldValue> {
    public ContactCustomFieldValueFaker()
        : base("en_GB") {
        
    }
}