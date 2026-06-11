using TraineeManagementApi.Models;

namespace TraineeManagementApi.DTOs;

public static class MentorConverter
{
    public static MentorResponse ToMentorResponse(Mentor mentor)
    {
        return new MentorResponse
        {
            Id = mentor.Id,
            FirstName = mentor.FirstName,
            LastName = mentor.LastName,
            Email = mentor.Email,
            Expertise = mentor.Expertise,
            Status = mentor.Status,
            CreatedDate = mentor.CreatedDate,
            UpdatedDate = mentor.UpdatedDate
        };
    }

    public static List<MentorResponse> ToMentorResponseList(List<Mentor> mentors)
    {
        List<MentorResponse> responses = new();

        foreach (var mentor in mentors) responses.Add(ToMentorResponse(mentor));

        return responses;
    }
}

