namespace TodoApp.Models
{
    public class TodoItem
    {
        public Guid id { get; set; }
        public required string text { get; set; }
    }
}
