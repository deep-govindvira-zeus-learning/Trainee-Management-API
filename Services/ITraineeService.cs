using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public interface ITraineeService
{
    List<TraineeResponse> GetAllTrainees();
    Trainee? GetTraineeById(string id);
    TraineeResponse? GetTraineeResponseById(string id);
    TraineeResponse CreateTrainee(CreateTraineeRequest request);
    TraineeResponse? UpdateTrainee(string id, UpdateTraineeRequest request);
    bool DeleteTraineeById(string id);

}
