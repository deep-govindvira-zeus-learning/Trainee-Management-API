using TraineeManagementApi.Models;

namespace TraineeManagementApi.DTOs;

public static class TraineeConverter
{
    public static TraineeResponse ToTraineeResponse(Trainee trainee)
    {
        return new TraineeResponse
        {
            Id = trainee.Id,
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status,
            CreatedDate = trainee.CreatedDate,
            UpdatedDate = trainee.UpdatedDate
        };
    }

    public static Trainee ToTrainee(CreateTraineeRequest request)
    {
        return new Trainee
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            TechStack = request.TechStack,
            Status = request.Status.ToString(),
        };
    }


    public static Trainee ToTrainee(UpdateTraineeRequest request)
    {
        return new Trainee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            TechStack = request.TechStack,
            Status = request.Status,
        };
    }

    public static List<TraineeResponse> ToTraineeResponseList(List<Trainee> trainees)
    {
        List<TraineeResponse> traineeResponses = new();

        foreach (var trainee in trainees) traineeResponses.Add(ToTraineeResponse(trainee));

        return traineeResponses;

    }
}

