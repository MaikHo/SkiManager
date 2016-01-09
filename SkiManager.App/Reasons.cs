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
            public static readonly Reason General = new Reason { Name = nameof(NoSpace) };
            public static readonly Reason InCarParkingLot = new Reason { Name = "No space in car parking lot" };
            public static readonly Reason InWaitingQueue = new Reason { Name = "No space in waiting queue" };
        }

        public static class Processing
        {
            public static readonly Reason General = new Reason { Name = nameof(Processing) };
            public static readonly Reason Started = new Reason { Name = "Processing started" };
            public static readonly Reason Unsuccessful = new Reason { Name = "Processing not succesful" };
            public static readonly Reason Finished = new Reason { Name = "Processing finished" };
        }

        public static class Subgraph
        {
            public static readonly Reason Entering = new Reason { Name = "Entering subgraph" };
            public static readonly Reason Leaving = new Reason { Name = "Leaving subgraph" };
        }

        public static readonly Reason NotAllowed = new Reason { Name = "NotAllowed" };
        public static readonly Reason DoesNotHaveRequiredItem = new Reason { Name = "Required item is missing" };
        public static readonly Reason TargetReached = new Reason { Name = "Target reached" };
        public static readonly Reason MovingStarted = new Reason { Name = "Moving started" };
        public static readonly Reason TemplateCreation = new Reason { Name = "Template creation" };
    }
}
