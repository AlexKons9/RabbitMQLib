using Consumer.Domain.Models;

namespace Consumer.Application.Services
{
    public class SomeService : BaseService
    {
        public void DoSomeTask(string value)
        {
            Thread.Sleep(2000);
            Console.WriteLine($"Service triggered. Value: {value}");
        }

        public Person EditPerson(Person person)
        {
            person.Name = "Edited";
            return person;
        }
    }
}
