
namespace RabbitMQLib.Models
{
    /// <summary>
    /// Pass parameters through this class. Use Name to pass the name of the value and 
    /// the value property to pass the value
    /// </summary>
    public class Parameters
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}