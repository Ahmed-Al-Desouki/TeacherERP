namespace TeacherERP.Domain.ValueObjects;

public enum EducationStage
{
    Unspecified = 0,
    Primary = 1,
    Preparatory = 2,
    Secondary = 3
}

/// <summary>A structured Egyptian school stage and grade number.</summary>
public sealed record SchoolGrade(EducationStage Stage, int Number)
{
    public bool IsValid()
    {
        return Stage switch
        {
            EducationStage.Primary => Number is >= 1 and <= 6,
            EducationStage.Preparatory or EducationStage.Secondary => Number is >= 1 and <= 3,
            _ => false
        };
    }

    public string ToDisplayName() => $"{Stage} {Number}";
}
