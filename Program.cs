using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        // Interface for all engines
        public interface IEngine
        {
            void Start();
        }

        // Common properties for engines
        public interface IEngineProperties
        {
            string Power { get; }
            string FuelType { get; }
        }

        // Common vehicle interface for all vehicles that can move
        public interface IMovable
        {
            void Move();
            int SeatingCapacity { get; }
        }

        // Common vehicle interface for vehicles that can fly
        public interface IFly : IMovable
        {
        }

        // Common vehicle interface for vehicles that move on road
        public interface IRoadVehicle : IMovable
        {
        }

        // Common vehicle interface for vehicles that can move on water
        public interface IWaterVehicle : IMovable
        {
        }

        // Load vehicle interface
        public interface ILoadVehicle
        {
            int Load { get; }
        }

        // Passenger vehicle interface
        public interface IPassengerVehicle
        {
            int PassengerCapacity { get; }
        }

        // Base engine class with common properties and methods
        public abstract class EngineBase : IEngine, IEngineProperties
        {
            public abstract string Power { get; }
            public abstract string FuelType { get; }
            public abstract void Start();
        }

        // Regular engine implementation
        public class RegularEngine : EngineBase
        {
            public override string Power => "Regular";
            public override string FuelType => "Petrol";

            public override void Start()
            {
                Console.WriteLine("Engine starts.");
            }
        }

        // Jet engine implementation
        public class JetEngine : EngineBase
        {
            public override string Power => "Jet";
            public override string FuelType => "Jet Fuel";

            public override void Start()
            {
                Console.WriteLine("Jet Engine starts.");
            }
        }

        // Steam engine implementation
        public class SteamEngine : EngineBase
        {
            public override string Power => "Steam";
            public override string FuelType => "Coal";

            public override void Start()
            {
                Console.WriteLine("Steam Engine starts.");
            }
        }

        // Common vehicle interface for all vehicles that have engines
        public interface IVehicle : IMovable, IEngineProperties
        {
        }

        // Base vehicle class with common properties and methods
        public abstract class VehicleBase : IVehicle
        {
            public int SeatingCapacity { get; protected set; }
            public abstract void Move();

            // Explicit implementation of IEngineProperties interface to access the engine properties
            string IEngineProperties.Power => (Engine as IEngineProperties)?.Power;
            string IEngineProperties.FuelType => (Engine as IEngineProperties)?.FuelType;

            protected IEngine Engine;

            protected VehicleBase(IEngine engine)
            {
                Engine = engine;
            }
        }

        // Boat class
        public class Boat : IWaterVehicle, ILoadVehicle
        {
            private readonly IEngine _engine;

            public Boat(int seatingCapacity, int load)
            {
                SeatingCapacity = seatingCapacity;
                Load = load;
                _engine = new RegularEngine();
            }

            public int SeatingCapacity { get; private set; }
            public int Load { get; private set; }

            public void Move()
            {
                Console.WriteLine($"Boat is moving on water.");
            }
        }

        // Aeroplan class
        public class Aeroplan : IFly, IPassengerVehicle
        {
            private readonly IEngine _engine;

            public Aeroplan(int seatingCapacity, int passengerCapacity)
            {
                SeatingCapacity = seatingCapacity;
                PassengerCapacity = passengerCapacity;
                _engine = new JetEngine();
            }

            public int SeatingCapacity { get; private set; }
            public int PassengerCapacity { get; private set; }

            public void Move()
            {
                Console.WriteLine($"Aeroplan is flying.");
            }
        }

        // Train class
        public class Train : IRoadVehicle, ILoadVehicle
        {
            private readonly IEngine _engine;

            public Train(int seatingCapacity, int load)
            {
                SeatingCapacity = seatingCapacity;
                Load = load;
                _engine = new SteamEngine();
            }

            public int SeatingCapacity { get; private set; }
            public int Load { get; private set; }

            public void Move()
            {
                Console.WriteLine($"Train is moving on the road.");
            }
        }

        // Bike class
        public class Bike : IRoadVehicle
        {
            private readonly IEngine _engine;

            public Bike(int seatingCapacity)
            {
                SeatingCapacity = seatingCapacity;
                _engine = new RegularEngine();
            }

            public int SeatingCapacity { get; private set; }

            public void Move()
            {
                Console.WriteLine($"Bike is moving on the road.");
            }
        }

        // Glider class
        public class Glider : IFly
        {
            public int SeatingCapacity { get; private set; }

            public Glider(int seatingCapacity)
            {
                SeatingCapacity = seatingCapacity;
            }

            public void Move()
            {
                Console.WriteLine("Glider is flying without an engine.");
            }
        }

        // Paddle Boat class
        public class PaddleBoat : IWaterVehicle
        {
            public int SeatingCapacity { get; private set; }

            public PaddleBoat(int seatingCapacity)
            {
                SeatingCapacity = seatingCapacity;
            }

            public void Move()
            {
                Console.WriteLine("Paddle boat is moving on water without an engine.");
            }
        }

        // UAV (Unmanned Aerial Vehicle) class
        public class UAV : IFly
        {
            private readonly IEngine _engine;

            public UAV()
            {
                SeatingCapacity = 0;
                _engine = new JetEngine();
            }

            public int SeatingCapacity { get; private set; }

            public void Move()
            {
                Console.WriteLine($"UAV is flying.");
            }
        }




        static void Main(string[] args)
        {
            List<IFly> flyingVehicles = new List<IFly>
        {
            new Aeroplan(150, 200),
            new Aeroplan(2, 1),
            new Glider(1),
            new UAV()
        };

            List<IRoadVehicle> roadVehicles = new List<IRoadVehicle>
        {
            new Train(300, 0),
            new Bike(2)
        };

            List<IWaterVehicle> waterVehicles = new List<IWaterVehicle>
        {
            new Boat(5, 100),
            new PaddleBoat(2)
        };

            foreach (var vehicle in flyingVehicles)
            {
                vehicle.Move();
                PrintEngineInfo(vehicle);
            }

            foreach (var vehicle in roadVehicles)
            {
                vehicle.Move();
                PrintEngineInfo(vehicle);
            }

            foreach (var vehicle in waterVehicles)
            {
                vehicle.Move();
                PrintEngineInfo(vehicle);
            }
        }

        static void PrintEngineInfo(IMovable vehicle)
        {
            if (vehicle is IEngineProperties vehicleWithEngine)
            {
                Console.WriteLine($"Engine Power: {vehicleWithEngine.Power}, Fuel Type: {vehicleWithEngine.FuelType}");
            }

        }
    }
}
