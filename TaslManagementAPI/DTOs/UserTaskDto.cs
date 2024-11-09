﻿namespace TaslManagementAPI.DTOs
{
    public class UserTaskDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UserId { get; set; }
    }

    public class UserTaskResultDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string UserId { get; set; }
        public string UserName  { get; set; }
    }
}
