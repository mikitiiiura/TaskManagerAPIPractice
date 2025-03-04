﻿using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.abstruct
{
    public interface ITasksRepository
    {
        Task Add(TaskEntity task);
        Task Delete(Guid id);
        Task<List<TaskEntity>> GetAll();
        Task<TaskEntity?> GetById(Guid id);
        Task<List<TaskEntity>> GetFilteredTasks(Guid userId, string? search, int? status, int? priority, DateTime? deadline, string? project, string? tag);
        Task Update(TaskEntity task);
        Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.TaskStatus status);
        Task UpdatePriority(Guid id, TaskManagerAPIPractice.Core.Model.TaskPriority priority);
        Task<List<TaskEntity>> GetAllByUser(Guid userId);
    }
}