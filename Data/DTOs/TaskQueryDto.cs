using TaskFlow.Api.Common.Enums;

namespace TaskFlow.Api.DTOs
{
    public class TaskQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc";
        public string? Search { get; set; }

        public Common.Enums.TaskItemStatus ? Status { get; set; }
    }
}
