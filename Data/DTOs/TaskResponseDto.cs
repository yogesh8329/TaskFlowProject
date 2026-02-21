using TaskFlow.Api.Common.Enums;

namespace TaskFlow.Api.DTOs
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public Common.Enums.TaskItemStatus Status { get; set; }

    }
}
