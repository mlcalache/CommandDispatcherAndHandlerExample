using System;
using System.Linq;
using System.Reflection;

namespace CommandDispatcherAndHandlerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dispatcher = new Dispatcher();
            dispatcher.Dispatch(new SomeCommand { MyProperty = "test" });
        }
    }

    public interface ICommandHandler<TCommand> where TCommand : class
    {
        void Handle(TCommand command);
    }

    public class SomeCommandHandler : ICommandHandler<SomeCommand>
    {
        public void Handle(SomeCommand command)
        {
            Console.WriteLine($"SomeCommandHandler.Value: {command.MyProperty}");
        }
    }

    public class SomeCommandHandler2 : ICommandHandler<SomeCommand>
    {
        public void Handle(SomeCommand command)
        {
            Console.WriteLine($"SomeCommandHandler2.Value: {command.MyProperty}");
        }
    }

    public class SomeCommand
    {
        public string MyProperty { get; set; }
    }

    public class Dispatcher
    {
        public void Dispatch<TCommand>(TCommand command) where TCommand : class
        {
            Type handler = typeof(ICommandHandler<>);
            
            Type handlerType = handler.MakeGenericType(command.GetType());

            Type[] concreteTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.GetInterfaces().Contains(handlerType)).ToArray();

            if (!concreteTypes.Any()) return;

            foreach (var type in concreteTypes)
            {
                var concreteHandler = Activator.CreateInstance(type) as ICommandHandler<TCommand>;

                concreteHandler?.Handle(command);
            }
        }
    }
}
