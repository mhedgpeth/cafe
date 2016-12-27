namespace cafe.Shared
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string FailureDescription { get; set; }

        public static Result Failure(string description)
        {
            return new Result() { IsSuccess = false, FailureDescription = description};
        }

        public static Result Successful()
        {
            return new Result() { IsSuccess = true };
        }

        public Result TranslateIfFailed(string translatedDescription)
        {
            return IsSuccess ? this : Failure(translatedDescription);
        }

        public override string ToString()
        {
            return IsSuccess ? "Successful" : $"Failed: {FailureDescription}";
        }

        protected bool Equals(Result other)
        {
            return IsSuccess == other.IsSuccess && string.Equals(FailureDescription, other.FailureDescription);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Result) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IsSuccess.GetHashCode() * 397) ^ (FailureDescription != null ? FailureDescription.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Result left, Result right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Result left, Result right)
        {
            return !Equals(left, right);
        }
    }
}