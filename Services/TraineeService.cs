using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public class TraineeService : ITraineeService
{
    private static readonly List<Trainee> trainees = new();

    // private readonly AppDbContext _context;

    // public TraineeService(AppDbContext context)
    // {
    //     _context = context;
    // }

    public List<TraineeResponse> GetAllTrainees()
    {
        return TraineeConverter.ToTraineeResponseList(trainees);
    }

    public TraineeResponse? GetTraineeResponseById(string traineeId)
    {
        foreach(var trainee in trainees)
            if (trainee.Id.Equals(traineeId))
                return TraineeConverter.ToTraineeResponse(trainee);

        return null;
        // return _context.Trainees.Find(traineeId);
    }

    public Trainee? GetTraineeById(string traineeId)
    {
        foreach(var trainee in trainees) if (trainee.Id.Equals(traineeId)) return trainee;

        return null;
        // return _context.Trainees.Find(traineeId);
    }

    public TraineeResponse CreateTrainee(CreateTraineeRequest createTraineeRequest)
    {
        Trainee trainee = TraineeConverter.ToTrainee(createTraineeRequest);
        trainee.Id = Guid.NewGuid().ToString();
        trainee.CreatedDate = DateTime.UtcNow;
        trainee.UpdatedDate = DateTime.UtcNow;

        trainees.Add(trainee);

        // EF Core pattern:
        // _context.Trainees.Add(trainee);
        // _context.SaveChanges();

        return TraineeConverter.ToTraineeResponse(trainee);
    }

    public TraineeResponse? UpdateTrainee(string id, UpdateTraineeRequest request)
    {
        Trainee updateTrainee = TraineeConverter.ToTrainee(request);

        foreach (var trainee in trainees)
        {
            if (trainee.Id.Equals(id))
            {
                trainee.FirstName = updateTrainee.FirstName;
                trainee.LastName = updateTrainee.LastName;
                trainee.Status = updateTrainee.Status;
                trainee.TechStack = updateTrainee.TechStack;
                trainee.Email = updateTrainee.Email;
                trainee.UpdatedDate = DateTime.UtcNow;
            }

            return TraineeConverter.ToTraineeResponse(trainee);
        }

        return null;
    }

    public bool DeleteTraineeById(string traineeId)
    {
        return trainees.Remove(GetTraineeById(traineeId));
    }
}

