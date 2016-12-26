using NodaTime;

namespace cafe.Test.Server
{
    public class FakeClock : IClock
    {
        public Instant CurrentInstant { get; set; } = Instant.FromUtc(2017, 1, 1, 9, 0);

        public Instant GetCurrentInstant()
        {
            return CurrentInstant;
        }

        public void AddToCurrentInstant(Duration duration)
        {
            CurrentInstant = CurrentInstant.Plus(duration);
        }
    }
}