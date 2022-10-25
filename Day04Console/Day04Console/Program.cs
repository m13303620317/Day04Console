using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day04Console
{
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    // Class that publishes an event
    class Publisher
    {
        public Publisher(int ticketNumber)
        {
            TicketNumber = ticketNumber;
        }

        public int TicketNumber { get; set; }

        // Declare the event using EventHandler<T>
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public void Init()
        {
            Console.WriteLine($"There is(are) {TicketNumber} ticket(s) left.");
        }

        public void DoSomething()
        {
            // Write some code that does something useful here
            // then raise the event. You can also raise an event
            // before you execute a block of code.
            OnRaiseCustomEvent(new CustomEventArgs("Ticket purchased"));
        }

        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        protected virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<CustomEventArgs> raiseEvent = RaiseCustomEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                // Format the string to send inside the CustomEventArgs parameter
                e.Message += $" at {DateTime.Now}";

                // Call to raise the event.
                raiseEvent(this, e);
            }
        }
    }

    //Class that subscribes to an event
    class Subscriber
    {
        private readonly string _id;

        public Subscriber(string id, Publisher pub)
        {
            _id = id;


            // Subscribe to the event
            pub.RaiseCustomEvent += HandleCustomEvent;

            pub.TicketNumber--;
            if (pub.TicketNumber < 0)
            {
                pub.RaiseCustomEvent -= HandleCustomEvent;
                Console.WriteLine($"There are no tickets for {_id}\nThere are only 0 tickets left.");
            }
            else
            {
                Console.WriteLine($"There is a ticket for {_id}\nThere is(are) {pub.TicketNumber} ticket(s) left.");
            }
        }

        // Define what actions to take when the event is raised.
        void HandleCustomEvent(object sender, CustomEventArgs e)
        {
            Console.WriteLine($"{_id} successfully purchased the ticket: {e.Message}");
        }
    }

    class Program
    {
        static async Task Main()
        {
            var pub = new Publisher(1);
            pub.Init();

            var sub1Task = BuyTicket("sub1", pub);
            var sub2Task = BuyTicket("sub2", pub);

            //var sub1 = await sub1Task;
            //var sub2 = await sub2Task;

            // Call the method that raises the event.
            pub.DoSomething();

            // Keep the console window open
            //Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private static async Task<Subscriber> BuyTicket(string id, Publisher pub)
        {
            Console.WriteLine("Start purchasing...");
            await Task.Delay(3000);
            return new Subscriber(id, pub);
        }
    }
}
