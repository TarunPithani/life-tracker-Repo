using LifeTracker.Application.Interfaces.Repositories;
using LifeTracker.Domain.Entities;
using LifeTracker.Infrastructure.Persistence;

namespace LifeTracker.Infrastructure.Repositories;

public class TodoRepository(AppDbContext context)
    : BaseRepository<Todo>(context), ITodoRepository;
