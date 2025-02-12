﻿using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;

        public TasksService(ITasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
        }

        // Отримати всі завдання
        public async Task<List<TaskEntity>> GetAll()
        {
            return await _tasksRepository.GetAll();
        }

        // Отримати завдання за ID
        public async Task<TaskEntity?> GetById(Guid id)
        {
            return await _tasksRepository.GetById(id);
        }

        // Додати нове завдання
        public async Task Add(TaskEntity task)
        {
            await _tasksRepository.Add(task);
        }

        // Оновити завдання
        public async Task Update(TaskEntity task)
        {
            await _tasksRepository.Update(task);
        }

        // Видалити завдання
        public async Task Delete(Guid id)
        {
            await _tasksRepository.Delete(id);
        }

        // Отримати відфільтровані завдання
        public async Task<List<TaskEntity>> GetFilteredTasks(string? search, int? status, int? priority, DateTime? deadline, string? project, string? tag)
        {
            return await _tasksRepository.GetFilteredTasks(search, status, priority, deadline, project, tag);
        }

        public async Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.TaskStatus status)
        {
            await _tasksRepository.UpdateStatus(id, status);
        }

        public async Task UpdatePriority(Guid id, TaskManagerAPIPractice.Core.Model.TaskPriority priority)
        {
            await _tasksRepository.UpdatePriority(id, priority);
        }

        public async Task<List<TaskEntity>> GetAllByUser(Guid userId)
        {
            return await _tasksRepository.GetAllByUser(userId);
        }
    }
}