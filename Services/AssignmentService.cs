using TraineeManagementApi.Data;

namespace TraineeManagementApi.Services;


public class AssignmentService : IAssignmentService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AssignmentService(AppDbContext context, ILogger<AuthService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
