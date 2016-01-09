using SkiManager.Engine;

namespace SkiManager.App
{
    public static class Reasons
    {
        public static class Loaded
        {
            public static readonly Reason General = new Reason { Name = nameof(Loaded) };
            public static readonly Reason IntoCar = new Reason { Name = "Loaded into car" };
        }

        public static class Unloaded
        {
            public static readonly Reason General = new Reason { Name = nameof(Unloaded) };
            public static readonly Reason FromCar = new Reason { Name = "Unloaded from car" };
        }

        public static class NoSpace
        {
            public static readonly Reason InCarParkingLot = new Reason { Name = "No space in car parking lot" };
        }

        public static readonly Reason NotAllowed = new Reason { Name = "NotAllowed" };
        public static readonly Reason ProcessingUnsuccessful = new Reason { Name = "Processing not succesful" };
        public static readonly Reason ProcessingFinished = new Reason { Name = "Processing finished" };
        public static readonly Reason DoesNotHaveRequiredItem = new Reason { Name = "Required item is missing" };
    }
}
