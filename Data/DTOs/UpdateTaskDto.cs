using TaskFlow.Api.Common.Enums;

namespace TaskFlow.Api.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
       // public int Status { get; set; }
        public TaskItemStatus Status { get; set; }

       
    }
}