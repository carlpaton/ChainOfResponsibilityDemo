namespace ChainOfResponsibilityDemo
{
    internal class ValidateObjectInMultipleSteps
    {
        public static void DoWork() 
        {
            var person = new Person()
            {
                Name = "Carl Brown Paton",
                Age = 84
            };

            var request = new Request() { Data = person };

            var maxNameLengthHandler = new MaxNameLengthHandler();
            var maxAgeHandler = new MaxAgeHandler();

            maxNameLengthHandler.SetNextHandler(maxAgeHandler);
            maxNameLengthHandler.Process(request);

            foreach (var message in request.Messages)
            {
                Console.WriteLine(message);
            }
        }
    }

    public class Request
    {
        public Request() => Messages = new List<string>();

        public object Data { get; set; }
        public List<string> Messages { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public interface IHandler
    {
        /// <summary>
        /// Used to propagate the request to the next handler
        /// </summary>
        /// <param name="handler"></param>
        void SetNextHandler(IHandler handler);

        /// <summary>
        /// Process the request
        /// </summary>
        /// <param name="request"></param>
        void Process(Request request);
    }

    public class BaseHandler : IHandler
    {
        protected IHandler? _nextHandler;

        public void SetNextHandler(IHandler handler) => _nextHandler = handler;

        public virtual void Process(Request request) => throw new NotImplementedException();
    }

    public class MaxNameLengthHandler : BaseHandler
    {
        public override void Process(Request request)
        {
            if (request.Data is Person person)
            {
                if (person.Name.Length > 10)
                    request.Messages.Add("Invalid Name Length");

                if (_nextHandler != null)
                    _nextHandler.Process(request);

                return;
            }

            throw new ArgumentException("Request was not of type Person");
        }
    }

    public class MaxAgeHandler : BaseHandler
    {
        public override void Process(Request request)
        {
            if (request.Data is Person person)
            {
                if (person.Age > 55)
                    request.Messages.Add("Invalid Age");

                if (_nextHandler != null)
                    _nextHandler.Process(request);

                return;
            }

            throw new ArgumentException("Request was not of type Person");
        }
    }
}
