namespace TennisMatch.Events
{
    internal class ValidationError
    {
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
    }
}