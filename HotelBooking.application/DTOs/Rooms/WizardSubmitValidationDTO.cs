public class WizardSubmitValidationDTO
{
    public bool CanSubmit { get; set; } = true;
    public List<string> Errors { get; set; } = new();

    public void AddError(string error)
    {
        Errors.Add(error);
        CanSubmit = false;
    }
}