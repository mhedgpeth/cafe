using System;

namespace cafe.CommandLine
{
    public class Result
    {
        public ResultStatus Status { get; set; }

        public bool IsSuccess => Status == ResultStatus.Success;

        public string FailureDescription { get; set; }
        public bool IsFailed => !IsSuccess;
        public bool IsInconclusive => Status == ResultStatus.Inconclusive;

        public static Result Failure(string description)
        {
            return new Result {Status = ResultStatus.Failure, FailureDescription = description};
        }

        public static Result Inconclusive(string description)
        {
            return new Result() {Status = ResultStatus.Inconclusive, FailureDescription = description};
        }

        public static Result Successful()
        {
            return new Result() {Status = ResultStatus.Success};
        }

        public Result TranslateIfFailed(string translatedDescription)
        {
            return IsSuccess ? this : Failure(translatedDescription);
        }

        public string StatusDescription
        {
            get
            {
                switch (Status)
                {
                    case ResultStatus.Failure:
                        return "Failed";
                    case ResultStatus.Success:
                        return "Successful";
                    case ResultStatus.Inconclusive:
                        return "Inconclusive";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string ToString()
        {
            return IsSuccess ? StatusDescription : $"{StatusDescription}: {FailureDescription}";
        }

        protected bool Equals(Result other)
        {
            return Status == other.Status && string.Equals(FailureDescription, other.FailureDescription);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Result) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Status * 397) ^ (FailureDescription?.GetHashCode() ?? 0);
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